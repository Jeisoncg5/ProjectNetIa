using System.Globalization;
using System.Text;
using Pgvector;

namespace ProjectNetIa.Infrastructure.Search;

internal static class ProductEmbeddingGenerator
{
    public const int Dimension = 256;

    public static Vector CreateForProduct(
        string categoryName,
        string productName,
        string? description)
    {
        var content = string.Join(
            " ",
            new[]
            {
                categoryName,
                productName,
                description ?? string.Empty
            }.Where(value => !string.IsNullOrWhiteSpace(value)));

        return CreateVector(content);
    }

    public static Vector CreateForQuery(string query)
    {
        return CreateVector(query);
    }

    private static Vector CreateVector(string input)
    {
        var normalized = Normalize(input);
        var components = new float[Dimension];

        foreach (var token in Tokenize(normalized))
        {
            AccumulateToken(components, token, 1.25f);
        }

        foreach (var trigram in GetCharacterNGrams(normalized, 3))
        {
            AccumulateToken(components, trigram, 0.35f);
        }

        NormalizeMagnitude(components);
        return new Vector(components);
    }

    private static string Normalize(string input)
    {
        if (string.IsNullOrWhiteSpace(input))
        {
            return string.Empty;
        }

        var decomposed = input.Trim().ToLowerInvariant().Normalize(NormalizationForm.FormD);
        var builder = new StringBuilder(decomposed.Length);

        foreach (var character in decomposed)
        {
            var category = CharUnicodeInfo.GetUnicodeCategory(character);
            if (category == UnicodeCategory.NonSpacingMark)
            {
                continue;
            }

            builder.Append(char.IsLetterOrDigit(character) ? character : ' ');
        }

        return builder.ToString().Normalize(NormalizationForm.FormC);
    }

    private static IEnumerable<string> Tokenize(string input)
    {
        return input
            .Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .Distinct(StringComparer.Ordinal);
    }

    private static IEnumerable<string> GetCharacterNGrams(string input, int n)
    {
        var compact = input.Replace(" ", string.Empty, StringComparison.Ordinal);
        if (compact.Length == 0)
        {
            yield break;
        }

        if (compact.Length <= n)
        {
            yield return compact;
            yield break;
        }

        for (var index = 0; index <= compact.Length - n; index++)
        {
            yield return compact.Substring(index, n);
        }
    }

    private static void AccumulateToken(float[] components, string token, float weight)
    {
        unchecked
        {
            uint hash = 2166136261;
            foreach (var character in token)
            {
                hash ^= character;
                hash *= 16777619;
            }

            var firstIndex = (int)(hash % Dimension);
            var secondIndex = (int)(((hash >> 16) ^ hash) % Dimension);

            components[firstIndex] += weight;
            components[secondIndex] += weight * 0.5f;
        }
    }

    private static void NormalizeMagnitude(float[] components)
    {
        double magnitudeSquared = 0;
        foreach (var component in components)
        {
            magnitudeSquared += component * component;
        }

        if (magnitudeSquared <= 0)
        {
            return;
        }

        var magnitude = (float)Math.Sqrt(magnitudeSquared);
        for (var index = 0; index < components.Length; index++)
        {
            components[index] /= magnitude;
        }
    }
}
