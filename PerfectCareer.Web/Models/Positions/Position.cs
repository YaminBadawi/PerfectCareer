using System.ComponentModel.DataAnnotations;

namespace PerfectCareer.Web.Models.Positions;

public sealed class Position
{
    public int Id { get; set; }

    [Required]
    [MaxLength(160)]
    public string Title { get; set; } = string.Empty;

    [Required]
    [MaxLength(160)]
    public string Location { get; set; } = string.Empty;

    [Required]
    [MaxLength(60)]
    public string EmploymentType { get; set; } = string.Empty;

    [Required]
    [MaxLength(5000)]
    public string Description { get; set; } = string.Empty;

    public bool IsActive { get; set; } = true;

    public DateTimeOffset CreatedAtUtc { get; set; }

    public DateTimeOffset UpdatedAtUtc { get; set; }

    [Timestamp]
    public byte[] RowVersion { get; set; } = Array.Empty<byte>();

    public ICollection<PositionTemplateAttribute> TemplateAttributes { get; set; } =
        new List<PositionTemplateAttribute>();

    public ICollection<PositionTechnologyTag> PositionTechnologyTags { get; set; } =
        new List<PositionTechnologyTag>();
}