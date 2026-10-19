using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

namespace Portfolio.Api.Extensions;

public static partial class SlugExtensions
{
    private const int MaxNumberedSuffix = 20;
    private const int RandomSuffixLength = 8;

    public static string ToSlug(this string phrase)
    {
        var output = RemoveDiacritics(phrase.ToLowerInvariant());
        output = Whitespace().Replace(output, "-");
        output = DisallowedCharacters().Replace(output, "");
        output = output.Trim('-', '_');
        return RepeatedSeparators().Replace(output, "$1");
    }

    public static async Task<string> ToUniqueSlugAsync(
        this string phrase,
        string fallback,
        Func<string, CancellationToken, Task<bool>> isTakenAsync,
        CancellationToken cancellationToken = default)
    {
        var baseSlug = phrase.ToSlug() is { Length: > 0 } slug ? slug : fallback;
        if (!await isTakenAsync(baseSlug, cancellationToken)) return baseSlug;

        for (var suffix = 2; suffix <= MaxNumberedSuffix; suffix++)
        {
            var candidate = $"{baseSlug}-{suffix}";
            if (!await isTakenAsync(candidate, cancellationToken)) return candidate;
        }

        return $"{baseSlug}-{Guid.NewGuid().ToString("N")[..RandomSuffixLength]}";
    }

    private static string RemoveDiacritics(string text)
    {
        var normalizedString = text.Normalize(NormalizationForm.FormD);
        var stringBuilder = new StringBuilder();

        foreach (var c in normalizedString)
        {
            if (CharUnicodeInfo.GetUnicodeCategory(c) != UnicodeCategory.NonSpacingMark) stringBuilder.Append(c);
        }

        return stringBuilder.ToString().Normalize(NormalizationForm.FormC);
    }

    [GeneratedRegex(@"\s")]
    private static partial Regex Whitespace();

    [GeneratedRegex(@"[^a-z0-9_-]")]
    private static partial Regex DisallowedCharacters();

    [GeneratedRegex(@"([-_]){2,}")]
    private static partial Regex RepeatedSeparators();
}
