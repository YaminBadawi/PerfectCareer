using System.ComponentModel.DataAnnotations;
using PerfectCareer.Web.Models.Positions;
using PerfectCareer.Web.Models.Profiles;

namespace PerfectCareer.Web.Models.Cvs;

public sealed class CandidateCv
{
    public int Id { get; set; }

    public int CandidateProfileId { get; set; }

    public int PositionId { get; set; }

    [EnumDataType(typeof(CandidateCvStatus))]
    public CandidateCvStatus Status { get; set; } =
        CandidateCvStatus.Draft;

    public DateTimeOffset CreatedAtUtc { get; set; }

    public DateTimeOffset UpdatedAtUtc { get; set; }

    public DateTimeOffset? PublishedAtUtc { get; set; }

    [Timestamp]
    public byte[] RowVersion { get; set; } = Array.Empty<byte>();

    public CandidateProfile CandidateProfile { get; set; } = null!;

    public Position Position { get; set; } = null!;

    public ICollection<CandidateCvProject> SelectedProjects { get; set; } =
        new List<CandidateCvProject>();
}