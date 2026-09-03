using System.ComponentModel.DataAnnotations;

namespace PerfectCareer.Web.ViewModels.Profiles;

public sealed class CandidateProfileEditViewModel
{
    [Required(ErrorMessage = "First name is required.")]
    [StringLength(100)]
    [Display(Name = "First name")]
    public string FirstName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Last name is required.")]
    [StringLength(100)]
    [Display(Name = "Last name")]
    public string LastName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Location is required.")]
    [StringLength(150)]
    public string Location { get; set; } = string.Empty;

    [Required(ErrorMessage = "Personal photo URL is required.")]
    [StringLength(2048)]
    [Url(ErrorMessage = "Enter a valid photo URL.")]
    [Display(Name = "Personal photo URL")]
    public string PersonalPhotoUrl { get; set; } = string.Empty;

    public byte[]? RowVersion { get; set; }
}