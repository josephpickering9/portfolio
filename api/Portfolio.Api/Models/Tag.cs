using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Portfolio.Api.Types;

namespace Portfolio.Api.Models;

public sealed class Tag
{
    [Required] public TagId Id { get; set; } = TagId.New();

    [Required] public string Title { get; set; } = null!;

    [Required] public TagType Type { get; set; }

    public string? Icon { get; set; }

    public string? CustomColour { get; set; }

    [Required] public string Slug { get; set; } = Guid.NewGuid().ToString();

    [Required] [JsonIgnore] public List<Project> Projects { get; set; } = [];
}

public enum TagType
{
    [Description("Default")] Default,
    [Description("Backend")] Backend,
    [Description("Frontend")] Frontend,
    [Description("DevOps")] DevOps,
    [Description("Other")] Other,
    [Description("Data")] Data,
    [Description("CMS")] CMS,
    [Description("Mobile")] Mobile,
    [Description("AI")] AI
}
