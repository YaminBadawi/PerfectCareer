using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PerfectCareer.Web.Authorization;
using PerfectCareer.Web.Data;

namespace PerfectCareer.Web.Controllers;

[Authorize(Roles = AppRoles.Candidate)]
[Route("CandidateProfile")]
public sealed class CandidateProfileAutoSaveController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<IdentityUser> _userManager;

    public CandidateProfileAutoSaveController(
        ApplicationDbContext context,
        UserManager<IdentityUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    [HttpPost("AutoSave")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Save(
        [FromBody] SaveCandidateProfileRequest? request,
        CancellationToken cancellationToken)
    {
        var userId = _userManager.GetUserId(User);

        if (userId is null)
        {
            return Challenge();
        }

        if (request is null)
        {
            return BadRequest(new
            {
                message = "The profile data is missing."
            });
        }

        if (!TryDecodeRowVersion(
                request.RowVersion,
                out var originalRowVersion))
        {
            return BadRequest(new
            {
                message =
                    "The profile version is invalid. Reload the page."
            });
        }

        var firstName =
            (request.FirstName ?? string.Empty).Trim();

        var lastName =
            (request.LastName ?? string.Empty).Trim();

        var location =
            (request.Location ?? string.Empty).Trim();

        var validationMessage = Validate(
            firstName,
            lastName,
            location);

        if (validationMessage is not null)
        {
            return BadRequest(new
            {
                message = validationMessage
            });
        }

        var profile = await _context.CandidateProfiles
            .SingleOrDefaultAsync(
                item => item.UserId == userId,
                cancellationToken);

        if (profile is null)
        {
            return NotFound(new
            {
                message =
                    "Save the profile once before using auto-save."
            });
        }

        _context.Entry(profile)
            .Property(item => item.RowVersion)
            .OriginalValue = originalRowVersion;

        profile.FirstName = firstName;
        profile.LastName = lastName;
        profile.Location = location;

        try
        {
            await _context.SaveChangesAsync(
                cancellationToken);
        }
        catch (DbUpdateConcurrencyException)
        {
            var currentProfile =
                await _context.CandidateProfiles
                    .AsNoTracking()
                    .Where(item => item.UserId == userId)
                    .Select(item => new
                    {
                        item.FirstName,
                        item.LastName,
                        item.Location,
                        item.RowVersion
                    })
                    .SingleOrDefaultAsync(
                        cancellationToken);

            if (currentProfile is null)
            {
                return Conflict(new
                {
                    message =
                        "The profile was deleted in another session. Reload the page."
                });
            }

            return Conflict(new
            {
                message =
                    "This profile was changed in another session. Reload before saving again.",

                rowVersion = Convert.ToBase64String(
                    currentProfile.RowVersion),

                firstName = currentProfile.FirstName,
                lastName = currentProfile.LastName,
                location = currentProfile.Location
            });
        }

        return Ok(new
        {
            rowVersion = Convert.ToBase64String(
                profile.RowVersion)
        });
    }

    private static string? Validate(
        string firstName,
        string lastName,
        string location)
    {
        if (string.IsNullOrWhiteSpace(firstName) ||
            string.IsNullOrWhiteSpace(lastName) ||
            string.IsNullOrWhiteSpace(location))
        {
            return "First name, last name, and location are required.";
        }

        if (firstName.Length > 100 ||
            lastName.Length > 100)
        {
            return "First name and last name must not exceed 100 characters.";
        }

        return location.Length > 150
            ? "Location must not exceed 150 characters."
            : null;
    }

    private static bool TryDecodeRowVersion(
        string? encodedRowVersion,
        out byte[] rowVersion)
    {
        rowVersion = Array.Empty<byte>();

        if (string.IsNullOrWhiteSpace(encodedRowVersion))
        {
            return false;
        }

        try
        {
            rowVersion = Convert.FromBase64String(
                encodedRowVersion);

            return rowVersion.Length == 8;
        }
        catch (FormatException)
        {
            return false;
        }
    }

    public sealed class SaveCandidateProfileRequest
    {
        public string FirstName { get; init; } = string.Empty;

        public string LastName { get; init; } = string.Empty;

        public string Location { get; init; } = string.Empty;

        public string RowVersion { get; init; } = string.Empty;
    }
}