using PerfectCareer.Web.Models.Cvs;
using PerfectCareer.Web.ViewModels.Profiles;

namespace PerfectCareer.Web.ViewModels.Cvs;

public sealed class CandidateCvDeleteViewModel
{
    public CandidateProfileDetailsViewModel Profile { get; init; } = new();

    public int Id { get; init; }

    public string PositionTitle { get; init; } = string.Empty;

    public string PositionLocation { get; init; } = string.Empty;

    public string EmploymentType { get; init; } = string.Empty;

    public CandidateCvStatus Status { get; init; }

    public int SelectedProjectCount { get; init; }

    public string RowVersion { get; init; } = string.Empty;
}