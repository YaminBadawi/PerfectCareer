using Microsoft.EntityFrameworkCore;
using PerfectCareer.Web.Models.Attributes;

namespace PerfectCareer.Web.Models.Profiles;

[Index(nameof(CandidateProfileId), nameof(LastUsedAtUtc))]
public sealed class CandidateAttributeUsage
{
    public int CandidateProfileId { get; set; }

    public int AttributeDefinitionId { get; set; }

    public DateTimeOffset LastUsedAtUtc { get; set; }

    public CandidateProfile CandidateProfile { get; set; } = null!;

    public AttributeDefinition AttributeDefinition { get; set; } = null!;
}