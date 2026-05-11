using System.ComponentModel.DataAnnotations;

namespace HPTourist.Data.DTOs;

[AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
public sealed class FutureDateAttribute : ValidationAttribute
{
    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        if (value is null)
        {
            return ValidationResult.Success;
        }

        DateTime date = value switch
        {
            DateTime dt => dt,
            DateOnly d => d.ToDateTime(TimeOnly.MinValue),
            _ => default,
        };

        if (date.Date <= DateTime.UtcNow.Date)
        {
            return new ValidationResult("Date must be in the future.", [validationContext.MemberName!]);
        }

        return ValidationResult.Success;
    }
}
