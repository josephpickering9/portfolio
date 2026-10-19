using Portfolio.Api.Models;
using Xunit;

namespace Portfolio.Api.Tests.Unit.Models;

public class ProjectImageUrlTests
{
    [Fact]
    public void CardImageUrl_PrefersCardThenBannerThenFirstDesktop()
    {
        Assert.Equal("card.png", ProjectWith(Image("card.png", ImageType.Card), Image("banner.png", ImageType.Banner)).CardImageUrl);
        Assert.Equal("banner.png", ProjectWith(Image("banner.png", ImageType.Banner), Image("desktop.png", ImageType.Desktop)).CardImageUrl);
        Assert.Equal("desktop.png", ProjectWith(Image("desktop.png", ImageType.Desktop)).CardImageUrl);
    }

    [Fact]
    public void HeroImageUrl_PrefersLowestOrderedDesktopThenBannerThenCard()
    {
        var project = ProjectWith(
            Image("desktop-2.png", ImageType.Desktop, 2),
            Image("desktop-1.png", ImageType.Desktop, 1),
            Image("banner.png", ImageType.Banner));

        Assert.Equal("desktop-1.png", project.HeroImageUrl);
        Assert.Equal("banner.png", ProjectWith(Image("banner.png", ImageType.Banner), Image("card.png", ImageType.Card)).HeroImageUrl);
        Assert.Equal("card.png", ProjectWith(Image("card.png", ImageType.Card)).HeroImageUrl);
    }

    [Fact]
    public void ImageUrls_AreNullWithoutImages()
    {
        var project = ProjectWith();

        Assert.Null(project.CardImageUrl);
        Assert.Null(project.HeroImageUrl);
    }

    private static Project ProjectWith(params ProjectImage[] images) => new()
    {
        Title = "Project",
        ShortDescription = "Short",
        Description = "Description",
        Images = [.. images]
    };

    private static ProjectImage Image(string path, ImageType type, int? order = null) =>
        new() { Image = path, Type = type, Order = order };
}
