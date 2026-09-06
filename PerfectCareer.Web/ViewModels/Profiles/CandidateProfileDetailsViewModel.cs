using PerfectCareer.Web.Models.Attributes;

namespace PerfectCareer.Web.ViewModels.Profiles;

public sealed class CandidateProfileDetailsViewModel
{
    public bool HasProfile { get; init; }

    public string FirstName { get; init; } = string.Empty;

    public string LastName { get; init; } = string.Empty;

    public string Location { get; init; } = string.Empty;

    public string PersonalPhotoUrl { get; init; } = string.Empty;

    public int CompletionPercentage { get; init; }

    public IReadOnlyList<AttributeLibraryItemViewModel> Attributes { get; init; } =
        Array.Empty<AttributeLibraryItemViewModel>();

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

public sealed class AttributeLibraryItemViewModel
{
    public int Id { get; init; }

    public string Name { get; init; } = string.Empty;

    public string Description { get; init; } = string.Empty;

    public AttributeCategory Category { get; init; }

    public AttributeDataType DataType { get; init; }

    public bool IsBuiltIn { get; init; }

    public bool IsSelected { get; init; }

    public string? TextValue { get; init; }

    public decimal? NumberValue { get; init; }

    public DateOnly? DateValue { get; init; }

    public DateOnly? PeriodStart { get; init; }

    public DateOnly? PeriodEnd { get; init; }

    public bool? BooleanValue { get; init; }

    public int? SelectedOptionId { get; init; }

    public string ValueRowVersion { get; init; } = string.Empty;

    public IReadOnlyList<AttributeOptionItemViewModel> Options { get; init; } =
        Array.Empty<AttributeOptionItemViewModel>();
}

public sealed class AttributeOptionItemViewModel
{
    public int Id { get; init; }

    public string Label { get; init; } = string.Empty;
}