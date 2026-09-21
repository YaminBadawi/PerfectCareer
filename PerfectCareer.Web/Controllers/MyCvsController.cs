using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PerfectCareer.Web.Authorization;
using PerfectCareer.Web.Data;
using PerfectCareer.Web.Models.Profiles;
using PerfectCareer.Web.ViewModels.Cvs;
using PerfectCareer.Web.ViewModels.Profiles;

namespace PerfectCareer.Web.Controllers;

[Authorize(Roles = AppRoles.Candidate)]
public sealed class MyCvsController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<IdentityUser> _userManager;

    public MyCvsController(
        ApplicationDbContext context,
        UserManager<IdentityUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    [HttpGet]
    public async Task<IActionResult> Index(
        CancellationToken cancellationToken)
    {
        var userId = _userManager.GetUserId(User);

        if (userId is null)
        {
            return Challenge();
        }

        var profile = await _context.CandidateProfiles
            .AsNoTracking()
            .SingleOrDefaultAsync(
                item => item.UserId == userId,
                cancellationToken);

        if (profile is null)
        {
            return RedirectToAction(
                "Edit",
                "CandidateProfile");
        }

        var cvs = await _context.CandidateCvs
            .AsNoTracking()
            .Where(cv =>
                cv.CandidateProfileId == profile.Id)
            .OrderByDescending(cv => cv.UpdatedAtUtc)
            .Select(cv =>
                new CandidateCvListItemViewModel
                {
                    Id = cv.Id,
                    PositionTitle = cv.Position.Title,
                    PositionLocation = cv.Position.Location,
                    EmploymentType = cv.Position.EmploymentType,
                    Status = cv.Status,
                    UpdatedAtUtc = cv.UpdatedAtUtc,
                    PublishedAtUtc = cv.PublishedAtUtc,
                    SelectedProjectCount =
                        cv.SelectedProjects.Count
                })
            .ToArrayAsync(cancellationToken);

        var model = new CandidateCvsIndexViewModel
        {
            Profile = CreateProfileDetails(profile),
            Cvs = cvs
        };

        return View(model);
    }

    [HttpGet]
    public async Task<IActionResult> Delete(
        int id,
        CancellationToken cancellationToken)
    {
        var userId = _userManager.GetUserId(User);

        if (userId is null)
        {
            return Challenge();
        }

        var profile = await _context.CandidateProfiles
            .AsNoTracking()
            .SingleOrDefaultAsync(
                item => item.UserId == userId,
                cancellationToken);

        if (profile is null)
        {
            return RedirectToAction(
                "Edit",
                "CandidateProfile");
        }

        var cv = await _context.CandidateCvs
            .AsNoTracking()
            .Where(item =>
                item.Id == id &&
                item.CandidateProfileId == profile.Id)
            .Select(item =>
                new
                {
                    item.Id,
                    PositionTitle = item.Position.Title,
                    PositionLocation = item.Position.Location,
                    EmploymentType = item.Position.EmploymentType,
                    item.Status,
                    SelectedProjectCount =
                        item.SelectedProjects.Count,
                    item.RowVersion
                })
            .SingleOrDefaultAsync(cancellationToken);

        if (cv is null)
        {
            return NotFound();
        }

        var model = new CandidateCvDeleteViewModel
        {
            Profile = CreateProfileDetails(profile),
            Id = cv.Id,
            PositionTitle = cv.PositionTitle,
            PositionLocation = cv.PositionLocation,
            EmploymentType = cv.EmploymentType,
            Status = cv.Status,
            SelectedProjectCount = cv.SelectedProjectCount,
            RowVersion = Convert.ToBase64String(cv.RowVersion)
        };

        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(
        int id,
        string? rowVersion,
        CancellationToken cancellationToken)
    {
        var userId = _userManager.GetUserId(User);

        if (userId is null)
        {
            return Challenge();
        }

        if (!TryDecodeRowVersion(
                rowVersion,
                out var originalRowVersion))
        {
            TempData["CvError"] =
                "The CV version is invalid. Reload and try again.";

            return RedirectToAction(nameof(Index));
        }

        var cv = await _context.CandidateCvs
            .SingleOrDefaultAsync(
                item =>
                    item.Id == id &&
                    item.CandidateProfile.UserId == userId,
                cancellationToken);

        if (cv is null)
        {
            return NotFound();
        }

        _context.Entry(cv)
            .Property(item => item.RowVersion)
            .OriginalValue = originalRowVersion;

        _context.CandidateCvs.Remove(cv);

        try
        {
            await _context.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateConcurrencyException)
        {
            TempData["CvError"] =
                "This CV changed or was already deleted. " +
                "Review the latest list and try again.";

            return RedirectToAction(nameof(Index));
        }

        TempData["CvMessage"] =
            "CV deleted successfully.";

        return RedirectToAction(nameof(Index));
    }

    private static CandidateProfileDetailsViewModel CreateProfileDetails(
        CandidateProfile profile)
    {
        return new CandidateProfileDetailsViewModel
        {
            HasProfile = true,
            FirstName = profile.FirstName,
            LastName = profile.LastName,
            Location = profile.Location,
            PersonalPhotoUrl = profile.PersonalPhotoUrl,
            CompletionPercentage = 100
        };
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
}