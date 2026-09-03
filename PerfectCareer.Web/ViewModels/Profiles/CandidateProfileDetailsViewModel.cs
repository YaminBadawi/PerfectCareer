namespace PerfectCareer.Web.ViewModels.Profiles;

public sealed class CandidateProfileDetailsViewModel
{
    public bool HasProfile { get; init; }

    public string FirstName { get; init; } = string.Empty;

    public string LastName { get; init; } = string.Empty;

    public string Location { get; init; } = string.Empty;

    public string PersonalPhotoUrl { get; init; } = string.Empty;

    public int CompletionPercentage { get; init; }

    public string FullName =>
        HasProfile
            ? $"{FirstName} {LastName}".Trim()
            : "Complete your profile";

    public string Initials
    {
        get
        {
            var firstInitial = string.IsNullOrWhiteSpace(FirstName)
                ? string.Empty
                : FirstName[..1];

            var lastInitial = string.IsNullOrWhiteSpace(LastName)
                ? string.Empty
                : LastName[..1];

            var initials = firstInitial + lastInitial;

            return string.IsNullOrEmpty(initials)
                ? "PC"
                : initials.ToUpperInvariant();
        }
    }
}