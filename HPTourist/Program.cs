using HPTourist.Components;
using HPTourist.Data.Models;
using HPTourist.Database;
using HPTourist.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using HPTourist.Data;

var app = Setup(args);

app.Run();

public partial class Program
{
   public static WebApplication Setup(string[] args)
   {
      var builder = WebApplication.CreateBuilder(args);

      // Add services to the container.
      builder.Services.AddRazorComponents()
          .AddInteractiveServerComponents();

      builder.Services.AddDbContextPool<DatabaseContext>(options => options.UseNpgsql(builder.Configuration.GetConnectionString("DatabaseConnection")));


      builder.Services.AddLocalization(options =>
      {
         options.ResourcesPath = "Resources";
      });

      builder.Services.Configure<RequestLocalizationOptions>(options =>
      {
         var supportedCultures = new[] { "en", "nl", "pl" };

         options.SetDefaultCulture("en")
                .AddSupportedCultures(supportedCultures)
                .AddSupportedUICultures(supportedCultures);
      });

      builder.Services.AddControllers();


      builder.Services.AddHttpContextAccessor();
      builder.Services.AddScoped<IPatientAccountService, PatientAccountService>();
      builder.Services.AddSingleton<IPasswordHasher<User>, PasswordHasher<User>>();

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

      app.MapControllers();


      app.UseRequestLocalization();


      app.UseStaticFiles();

      // Configure the HTTP request pipeline.
      if (!app.Environment.IsDevelopment())
      {
         app.UseExceptionHandler("/Error", createScopeForErrors: true);
         // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
         app.UseHsts();
      }
      app.UseStatusCodePagesWithReExecute("/not-found", createScopeForStatusCodePages: true);
      app.UseHttpsRedirection();

      app.UseAuthentication();
      app.UseAuthorization();
      app.UseAntiforgery();

      if (!args.Contains("Test"))
      {
         app.MapStaticAssets();
      }
      app.MapRazorComponents<App>()
          .AddInteractiveServerRenderMode();

      //Disable triggers that enforce non-deletion of records for development environment.
      //Should stay (optionally) until anonimization options have been put in place instead of delete, to prevent dev work from being dragged down
      if (app.Environment.IsDevelopment())
      {
         dbDeleteProtection(app, enable: false);
      }
      else
      {
         dbDeleteProtection(app, enable: true);
      }

      return app;
   }

   static void dbDeleteProtection(WebApplication app, bool enable)
   {
      string setState = enable ? "ENABLE" : "DISABLE";

      using (var scope = app.Services.CreateScope())
      {
         var db = scope.ServiceProvider.GetRequiredService<DatabaseContext>();
         string updateTrigger(string table, string triggerName) => $"ALTER TABLE \"{table}\" {setState} TRIGGER \"{triggerName}\";";

         db.Database.ExecuteSqlRaw(updateTrigger("Patients", FunctionNames.PATIENT_CANT_BE_DELETED));
         db.Database.ExecuteSqlRaw(updateTrigger("Practices", FunctionNames.PRACTICE_CANT_BE_DELETED));
         db.Database.ExecuteSqlRaw(updateTrigger("PrescriptionRequests", FunctionNames.PRESCRIPTIONREQUEST_CANT_BE_DELETED));
         db.Database.ExecuteSqlRaw(updateTrigger("Prescriptions", FunctionNames.PRESCRIPTION_CANT_BE_DELETED));
         db.Database.ExecuteSqlRaw(updateTrigger("Employees", FunctionNames.EMPLOYEE_CANT_BE_DELETED));
         db.Database.ExecuteSqlRaw(updateTrigger("EHICs", FunctionNames.EHIC_CANT_BE_DELETED));
         db.Database.ExecuteSqlRaw(updateTrigger("Identificatios", FunctionNames.IDENTIFICATION_CANT_BE_DELETED));
      }
   }

}
