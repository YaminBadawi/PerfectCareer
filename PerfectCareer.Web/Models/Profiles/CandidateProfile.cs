using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace PerfectCareer.Web.Models.Profiles;

public class CandidateProfile
{
    public int Id { get; set; }

    [Required]
    public string UserId { get; set; } = string.Empty;

    [Required]
    [StringLength(100)]
    public string FirstName { get; set; } = string.Empty;

    [Required]
    [StringLength(100)]
    public string LastName { get; set; } = string.Empty;

    [Required]
    [StringLength(150)]
    public string Location { get; set; } = string.Empty;

    [Required]
    [Url]
    [StringLength(2048)]
    public string PersonalPhotoUrl { get; set; } = string.Empty;

    [Timestamp]
    public byte[] RowVersion { get; set; } = Array.Empty<byte>();

    public IdentityUser User { get; set; } = null!;
}