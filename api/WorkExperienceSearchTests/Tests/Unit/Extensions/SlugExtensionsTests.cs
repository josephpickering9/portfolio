using Work_Experience_Search.Extensions;
using Xunit;

namespace WorkExperienceSearchTests.Tests.Unit.Extensions;

public class SlugExtensionsTests
{
    private const string Fallback = "item";

    [Theory]
    [InlineData("Visit Northumberland", "visit-northumberland")]
    [InlineData("  ASP.NET   Core ", "aspnet-core")]
    [InlineData("Café Société", "cafe-societe")]
    [InlineData("C#", "c")]
    [InlineData("snake__case--slug", "snake_case-slug")]
    public void ToSlug_NormalisesPhrase(string phrase, string expected)
    {
        Assert.Equal(expected, phrase.ToSlug());
    }

    [Fact]
    public async Task ToUniqueSlugAsync_SlugFree_ReturnsBaseSlug()
    {
        var slug = await "Vue.js".ToUniqueSlugAsync(Fallback, Taken());

        Assert.Equal("vuejs", slug);
    }

    [Fact]
    public async Task ToUniqueSlugAsync_SlugTaken_AppendsFirstFreeNumber()
    {
        var slug = await "Vue.js".ToUniqueSlugAsync(Fallback, Taken("vuejs", "vuejs-2"));

        Assert.Equal("vuejs-3", slug);
    }

    [Fact]
    public async Task ToUniqueSlugAsync_PhraseHasNoSlugCharacters_UsesFallback()
    {
        var slug = await "!!!".ToUniqueSlugAsync(Fallback, Taken());

        Assert.Equal(Fallback, slug);
    }

    [Fact]
    public async Task ToUniqueSlugAsync_NumberedSuffixesExhausted_AppendsRandomSuffix()
    {
        var taken = new[] { "vuejs" }.Concat(Enumerable.Range(2, 19).Select(n => $"vuejs-{n}")).ToArray();

        var slug = await "Vue.js".ToUniqueSlugAsync(Fallback, Taken(taken));

        Assert.Matches("^vuejs-[0-9a-f]{8}$", slug);
    }

    private static Func<string, CancellationToken, Task<bool>> Taken(params string[] slugs) =>
        (slug, _) => Task.FromResult(slugs.Contains(slug));
}
