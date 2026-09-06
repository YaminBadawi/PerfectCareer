using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using PerfectCareer.Web.Models.Attributes;

namespace PerfectCareer.Web.Models.Profiles;

[Index(nameof(CandidateProfileId), nameof(AttributeDefinitionId), IsUnique = true)]
public sealed class CandidateAttributeValue
{
    public int Id { get; set; }

    public int CandidateProfileId { get; set; }

    public int AttributeDefinitionId { get; set; }

    public string? TextValue { get; set; }

    [Precision(18, 4)]
    public decimal? NumberValue { get; set; }

    public DateOnly? DateValue { get; set; }

    public DateOnly? PeriodStart { get; set; }

    public DateOnly? PeriodEnd { get; set; }

    public bool? BooleanValue { get; set; }

    public int? SelectedOptionId { get; set; }

    [Timestamp]
    public byte[] RowVersion { get; set; } = Array.Empty<byte>();

    public CandidateProfile CandidateProfile { get; set; } = null!;

    public AttributeDefinition AttributeDefinition { get; set; } = null!;

    public AttributeOption? SelectedOption { get; set; }
}