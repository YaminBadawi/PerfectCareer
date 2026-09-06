using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

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

    [StringLength(2048)]
    [Url(ErrorMessage = "The saved photo URL is invalid.")]
    public string PersonalPhotoUrl { get; set; } = string.Empty;

    [Display(Name = "Personal photo")]
    public IFormFile? PersonalPhotoFile { get; set; }

    public byte[]? RowVersion { get; set; }
}