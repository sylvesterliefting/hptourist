using System.Data;
using HPTourist.Components;
using HPTourist.Data;
using HPTourist.Data.Models;
using HPTourist.Database;
using HPTourist.Features.ContraIndications;
using HPTourist.Features.PatientMedications;
using HPTourist.Features.RepeatPrescriptions;
using HPTourist.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace HPTourist;

public static class Application
{
    public static WebApplication Setup(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddRazorComponents()
            .AddInteractiveServerComponents();

        builder.Services.AddDbContextPool<DatabaseContext>(options =>
            options.UseNpgsql(builder.Configuration.GetConnectionString("DatabaseConnection")));

        builder.Services.AddLocalization(options => options.ResourcesPath = "Resources");

        builder.Services.Configure<RequestLocalizationOptions>(options =>
        {
            var supportedCultures = new[] { "nl", "en", "pl" };

            options.SetDefaultCulture("nl")
                .AddSupportedCultures(supportedCultures)
                .AddSupportedUICultures(supportedCultures);
        });

        builder.Services.AddControllers();
        builder.Services.AddHttpContextAccessor();
        builder.Services.AddScoped<ICurrentPatientService, CurrentPatientService>();
        builder.Services.AddScoped<IPatientAccountService, PatientAccountService>();
        builder.Services.AddScoped<IPatientMedicationService, PatientMedicationService>();
        builder.Services.AddScoped<IRepeatPrescriptionRequestService, RepeatPrescriptionRequestService>();
        builder.Services.AddScoped<IContraIndicationService, ContraIndicationService>();
        builder.Services.AddScoped<IFarmacotherapeutischKompasUrlService, FarmacotherapeutischKompasUrlService>();
        builder.Services.AddHttpClient<IFarmacotherapeutischKompasScraperService, FarmacotherapeutischKompasScraperService>();
        builder.Services.AddSingleton<IPasswordHasher<User>, PasswordHasher<User>>();
        builder.Services.AddSingleton(TimeProvider.System);

        builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
            .AddCookie(options =>
            {
                options.LoginPath = "/login";
                options.LogoutPath = "/logout";
                options.Cookie.Name = "HPTourist.Auth";
                options.ExpireTimeSpan = TimeSpan.FromHours(8);
                options.SlidingExpiration = true;
            });

        builder.Services.AddAuthorization();
        builder.Services.AddCascadingAuthenticationState();

        var app = builder.Build();

        var isTestRun = IsTestRun(args);

        if (app.Environment.IsDevelopment() || isTestRun)
        {
            PrepareDevelopmentDatabase(app, builder.Configuration);
        }

        app.MapControllers();
        app.UseRequestLocalization();
        app.UseStaticFiles();

        if (!app.Environment.IsDevelopment())
        {
            app.UseExceptionHandler("/Error", createScopeForErrors: true);
            app.UseHsts();
        }

        app.UseStatusCodePagesWithReExecute("/not-found", createScopeForStatusCodePages: true);
        app.UseHttpsRedirection();

        app.UseAuthentication();
        app.UseAuthorization();
        app.UseAntiforgery();

        if (!isTestRun)
        {
            app.MapStaticAssets();
        }

        app.MapRazorComponents<App>()
            .AddInteractiveServerRenderMode();

        SetDeleteProtection(app, enable: !app.Environment.IsDevelopment());

        return app;
    }

    private static void PrepareDevelopmentDatabase(WebApplication app, IConfiguration configuration)
    {
        using var scope = app.Services.CreateScope();

        var db = scope.ServiceProvider.GetRequiredService<DatabaseContext>();
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
        var passwordHasher = scope.ServiceProvider.GetRequiredService<IPasswordHasher<User>>();
        var connectionString = configuration.GetConnectionString("DatabaseConnection")
            ?? throw new InvalidOperationException("Missing DatabaseConnection connection string.");

        CreateDatabaseIfMissing(connectionString, logger);

        db.Database.Migrate();
        logger.LogInformation("Migrations applied");

        DevelopmentDataSeeder.SeedUserIfMissing(db, passwordHasher, logger);
    }

    private static void CreateDatabaseIfMissing(string connectionString, ILogger logger)
    {
        var targetDatabase = new NpgsqlConnectionStringBuilder(connectionString).Database;
        if (string.IsNullOrWhiteSpace(targetDatabase))
        {
            throw new InvalidOperationException("DatabaseConnection must include a database name.");
        }

        var maintenanceConnectionString = new NpgsqlConnectionStringBuilder(connectionString)
        {
            Database = "postgres",
        };

        using var connection = new NpgsqlConnection(maintenanceConnectionString.ConnectionString);
        connection.Open();

        using var command = connection.CreateCommand();
        command.CommandText = $"CREATE DATABASE {QuoteIdentifier(targetDatabase)} WITH TEMPLATE template0;";

        try
        {
            command.ExecuteNonQuery();
            logger.LogInformation("Database {DatabaseName} created", targetDatabase);
        }
        catch (PostgresException ex) when (ex.SqlState == PostgresErrorCodes.DuplicateDatabase)
        {
            logger.LogInformation("Database {DatabaseName} already exists", targetDatabase);
        }
    }

    private static void SetDeleteProtection(WebApplication app, bool enable)
    {
        using var scope = app.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<DatabaseContext>();
        var state = enable ? "ENABLE" : "DISABLE";

        SetTriggerState(db, "Patients", FunctionNames.PATIENT_CANT_BE_DELETED, state);
        SetTriggerState(db, "Practices", FunctionNames.PRACTICE_CANT_BE_DELETED, state);
        SetTriggerState(db, "PrescriptionRequests", FunctionNames.PRESCRIPTIONREQUEST_CANT_BE_DELETED, state);
        SetTriggerState(db, "Prescriptions", FunctionNames.PRESCRIPTION_CANT_BE_DELETED, state);
        SetTriggerState(db, "Employees", FunctionNames.EMPLOYEE_CANT_BE_DELETED, state);
        SetTriggerState(db, "EHICs", FunctionNames.EHIC_CANT_BE_DELETED, state);
        SetTriggerState(db, "Identificatios", FunctionNames.IDENTIFICATION_CANT_BE_DELETED, state);
    }

    private static void SetTriggerState(DatabaseContext db, string table, string trigger, string state)
    {
        var connection = db.Database.GetDbConnection();
        var shouldCloseConnection = connection.State != ConnectionState.Open;

        if (shouldCloseConnection)
        {
            connection.Open();
        }

        try
        {
            using var command = connection.CreateCommand();
            command.CommandText = $"ALTER TABLE {QuoteIdentifier(table)} {state} TRIGGER {QuoteIdentifier(trigger)};";
            command.ExecuteNonQuery();
        }
        finally
        {
            if (shouldCloseConnection)
            {
                connection.Close();
            }
        }
    }

    private static bool IsTestRun(string[] args)
    {
        return args.Contains("Test", StringComparer.OrdinalIgnoreCase);
    }

    private static string QuoteIdentifier(string identifier)
    {
        return "\"" + identifier.Replace("\"", "\"\"") + "\"";
    }
}
