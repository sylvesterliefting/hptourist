using System.ComponentModel.DataAnnotations;

namespace HPTourist.Models;

[AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
public sealed class AgeRangeAttribute : ValidationAttribute
{
    public int MinAge { get; }

    public int MaxAge { get; }

    public AgeRangeAttribute(int minAge, int maxAge)
    {
        MinAge = minAge;
        MaxAge = maxAge;
    }

    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        if (value is null)
        {
            return ValidationResult.Success;
        }

        DateOnly dob = value switch
        {
            DateOnly d => d,
            DateTime dt => DateOnly.FromDateTime(dt),
            _ => default,
        };

        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        if (dob > today)
        {
            return new ValidationResult("Date of birth cannot be in the future.", [validationContext.MemberName!]);
        }

        var age = today.Year - dob.Year;
        if (dob > today.AddYears(-age))
        {
            age--;
        }

        if (age < MinAge || age > MaxAge)
        {
            return new ValidationResult(
                $"Age must be between {MinAge} and {MaxAge} years.",
                [validationContext.MemberName!]);
        }

        return ValidationResult.Success;
    }
}
