using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace PerfectCareer.Web.Models.Attributes;

[Index(nameof(AttributeDefinitionId), nameof(Label), IsUnique = true)]
public sealed class AttributeOption
{
    public int Id { get; set; }

    public int AttributeDefinitionId { get; set; }

    [Required]
    [MaxLength(150)]
    public string Label { get; set; } = string.Empty;

    [Timestamp]
    public byte[] RowVersion { get; set; } = Array.Empty<byte>();

    public AttributeDefinition AttributeDefinition { get; set; } = null!;
}