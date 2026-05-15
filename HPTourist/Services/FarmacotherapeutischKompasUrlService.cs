using System.Globalization;
using System.Text;

namespace HPTourist.Services;

public sealed class FarmacotherapeutischKompasUrlService 
    : IFarmacotherapeutischKompasUrlService
{
    private const string BaseUrl =
        "https://www.farmacotherapeutischkompas.nl/bladeren/preparaatteksten";

    public string CreatePreparationtextUrl(string activeSubstance)
    {
        if (string.IsNullOrWhiteSpace(activeSubstance))
            throw new ArgumentException("Werkzame stof mag niet leeg zijn.", nameof(activeSubstance));

        var slug = ToSlug(activeSubstance);
        return $"{BaseUrl}/{slug[0]}/{slug}";
    }

    private static string ToSlug(string value)
    {
        var normalized = value
            .Trim()
            .ToLowerInvariant()
            .Normalize(NormalizationForm.FormD);

        var builder = new StringBuilder();

        foreach (var c in normalized)
        {
            var category = CharUnicodeInfo.GetUnicodeCategory(c);

            if (category == UnicodeCategory.NonSpacingMark)
                continue;

            if (char.IsLetterOrDigit(c))
                builder.Append(c);
            else if (char.IsWhiteSpace(c) || c is '-' or '/')
                builder.Append('_');
        }

        return builder.ToString().Trim('_');
    }
}