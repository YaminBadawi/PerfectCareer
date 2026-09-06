using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace PerfectCareer.Web.Models.Attributes;

[Index(nameof(Name), IsUnique = true)]
public sealed class AttributeDefinition
{
    public int Id { get; set; }

    [Required]
    [MaxLength(150)]
    public string Name { get; set; } = string.Empty;

    [Required]
    [MaxLength(2000)]
    public string Description { get; set; } = string.Empty;

    [EnumDataType(typeof(AttributeCategory))]
    public AttributeCategory Category { get; set; }

    [EnumDataType(typeof(AttributeDataType))]
    public AttributeDataType DataType { get; set; }

    public bool IsBuiltIn { get; set; }

    [Timestamp]
    public byte[] RowVersion { get; set; } = Array.Empty<byte>();
}
