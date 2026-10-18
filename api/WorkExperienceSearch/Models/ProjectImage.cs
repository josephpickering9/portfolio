using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Work_Experience_Search.Types;

namespace Work_Experience_Search.Models;

public sealed class ProjectImage
{
    [Required] public ProjectImageId Id { get; set; } = ProjectImageId.New();

    [Required] public string Image { get; set; } = null!;

    [Required] public ImageType Type { get; set; }

    public int? Order { get; set; }

    public bool IsOptimised { get; set; } = false;

    public ProjectId ProjectId { get; set; }
    [Required] [JsonIgnore] public Project? Project { get; set; }
}

public enum ImageType
{
    [Description("Logo")] Logo,
    [Description("Banner")] Banner,
    [Description("Card")] Card,
    [Description("Desktop")] Desktop,
    [Description("Mobile")] Mobile
}
