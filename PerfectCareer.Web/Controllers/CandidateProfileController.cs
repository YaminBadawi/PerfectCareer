using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using PerfectCareer.Web.Authorization;
using PerfectCareer.Web.Data;
using PerfectCareer.Web.Models.Attributes;
using PerfectCareer.Web.Models.Profiles;
using PerfectCareer.Web.Services;
using PerfectCareer.Web.ViewModels.Profiles;

namespace PerfectCareer.Web.Controllers;

[Authorize(Roles = AppRoles.Candidate)]
public sealed class CandidateProfileController : Controller
{
    private const string ProfilePhotoFolder =
        "perfect-career/profile-photos";

    private readonly ApplicationDbContext _context;
    private readonly UserManager<IdentityUser> _userManager;
    private readonly CloudinaryImageService _imageService;
    private readonly ILogger<CandidateProfileController> _logger;

    public CandidateProfileController(
        ApplicationDbContext context,
        UserManager<IdentityUser> userManager,
        CloudinaryImageService imageService,
        ILogger<CandidateProfileController> logger)
    {
        _context = context;
        _userManager = userManager;
        _imageService = imageService;
        _logger = logger;
    }

    [HttpGet]
    public Task<IActionResult> Index(
        CancellationToken cancellationToken) =>
        RenderProfileAsync(
            nameof(Index),
            loadAttributes: true,
            cancellationToken);

    [HttpGet]
    public Task<IActionResult> Info(
        CancellationToken cancellationToken) =>
        RenderProfileAsync(
            nameof(Info),
            loadAttributes: true,
            cancellationToken);

    private async Task<IActionResult> RenderProfileAsync(
        string viewName,
        bool loadAttributes,
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

        IReadOnlyList<AttributeLibraryItemViewModel> attributes =
            Array.Empty<AttributeLibraryItemViewModel>();

        if (loadAttributes)
        {
            var values = profile is null
                ? Array.Empty<CandidateAttributeValue>()
                : await _context.CandidateAttributeValues
                    .AsNoTracking()
                    .Where(item =>
                        item.CandidateProfileId == profile.Id)
                    .ToArrayAsync(cancellationToken);

            var valuesByDefinitionId = values
                .ToDictionary(
                    item => item.AttributeDefinitionId);

            var usageTimesByDefinitionId =
                profile is null
                    ? new Dictionary<int, DateTimeOffset>()
                    : await _context.CandidateAttributeUsages
                        .AsNoTracking()
                        .Where(item =>
                            item.CandidateProfileId == profile.Id)
                        .ToDictionaryAsync(
                            item => item.AttributeDefinitionId,
                            item => item.LastUsedAtUtc,
                            cancellationToken);

            var definitions = await _context.AttributeDefinitions
                .AsNoTracking()
                .OrderBy(item => item.Category)
                .ThenBy(item => item.Name)
                .ToArrayAsync(cancellationToken);

            var optionGroups = (await _context.AttributeOptions
                    .AsNoTracking()
                    .OrderBy(item => item.Id)
                    .ToArrayAsync(cancellationToken))
                .GroupBy(item =>
                    item.AttributeDefinitionId)
                .ToDictionary(
                    group => group.Key,
                    group => group
                        .Select(option =>
                            new AttributeOptionItemViewModel
                            {
                                Id = option.Id,
                                Label = option.Label
                            })
                        .ToArray());

            attributes = definitions
                .Select(definition =>
                {
                    valuesByDefinitionId.TryGetValue(
                        definition.Id,
                        out var value);

                    optionGroups.TryGetValue(
                        definition.Id,
                        out var options);

                    return new AttributeLibraryItemViewModel
                    {
                        Id = definition.Id,
                        Name = definition.Name,
                        Description = definition.Description,
                        Category = definition.Category,
                        DataType = definition.DataType,
                        IsBuiltIn = definition.IsBuiltIn,
                        IsSelected =
                            definition.IsBuiltIn ||
                            value is not null,
                        LastUsedAtUtc =
                            usageTimesByDefinitionId
                                .GetValueOrDefault(definition.Id),
                        TextValue = value?.TextValue,
                        NumberValue = value?.NumberValue,
                        DateValue = value?.DateValue,
                        PeriodStart = value?.PeriodStart,
                        PeriodEnd = value?.PeriodEnd,
                        BooleanValue = value?.BooleanValue,
                        SelectedOptionId =
                            value?.SelectedOptionId,
                        ValueRowVersion =
                            value is null
                                ? string.Empty
                                : Convert.ToBase64String(
                                    value.RowVersion),
                        Options =
                            options ??
                            Array.Empty<AttributeOptionItemViewModel>()
                    };
                })
                .ToArray();
        }

        var model = new CandidateProfileDetailsViewModel
        {
            HasProfile = profile is not null,
            FirstName =
                profile?.FirstName ?? string.Empty,
            LastName =
                profile?.LastName ?? string.Empty,
            Location =
                profile?.Location ?? string.Empty,
            PersonalPhotoUrl =
                profile?.PersonalPhotoUrl ?? string.Empty,
            CompletionPercentage =
                profile is null ? 0 : 100,
            Attributes = attributes
        };

        return View(viewName, model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AddAttributes(
        int[]? attributeIds,
        CancellationToken cancellationToken)
    {
        var userId = _userManager.GetUserId(User);

        if (userId is null)
        {
            return Challenge();
        }

        var profileId = await _context.CandidateProfiles
            .Where(item => item.UserId == userId)
            .Select(item => (int?)item.Id)
            .SingleOrDefaultAsync(cancellationToken);

        if (profileId is null)
        {
            return RedirectToAction(nameof(Edit));
        }

        var requestedIds =
            attributeIds?
                .Distinct()
                .ToArray() ??
            Array.Empty<int>();

        if (requestedIds.Length == 0)
        {
            return RedirectToAction(nameof(Info));
        }

        var validIds = await _context.AttributeDefinitions
            .AsNoTracking()
            .Where(item =>
                requestedIds.Contains(item.Id) &&
                !item.IsBuiltIn)
            .Select(item => item.Id)
            .ToArrayAsync(cancellationToken);

        if (validIds.Length == 0)
        {
            return RedirectToAction(nameof(Info));
        }

        var existingIds = await _context.CandidateAttributeValues
            .AsNoTracking()
            .Where(item =>
                item.CandidateProfileId == profileId.Value &&
                validIds.Contains(item.AttributeDefinitionId))
            .Select(item =>
                item.AttributeDefinitionId)
            .ToArrayAsync(cancellationToken);

        var newValues = validIds
            .Except(existingIds)
            .Select(definitionId =>
                new CandidateAttributeValue
                {
                    CandidateProfileId =
                        profileId.Value,
                    AttributeDefinitionId =
                        definitionId
                })
            .ToArray();

        _context.CandidateAttributeValues
            .AddRange(newValues);

        await TouchAttributeUsagesAsync(
            profileId.Value,
            validIds,
            cancellationToken);

        try
        {
            await _context.SaveChangesAsync(
                cancellationToken);
        }
        catch (DbUpdateException exception) when (
            exception.InnerException is
                SqlException { Number: 2601 or 2627 })
        {
            TempData["InfoError"] =
                "The attributes were changed in another session. Refresh and try again.";
        }

        return RedirectToAction(nameof(Info));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> RemoveAttributes(
        int[]? attributeIds,
        CancellationToken cancellationToken)
    {
        var userId = _userManager.GetUserId(User);

        if (userId is null)
        {
            return Challenge();
        }

        var requestedIds =
            attributeIds?
                .Distinct()
                .ToArray() ??
            Array.Empty<int>();

        if (requestedIds.Length == 0)
        {
            return RedirectToAction(nameof(Info));
        }

        var profileId = await _context.CandidateProfiles
            .Where(item => item.UserId == userId)
            .Select(item => (int?)item.Id)
            .SingleOrDefaultAsync(cancellationToken);

        if (profileId is null)
        {
            return RedirectToAction(nameof(Info));
        }

        var values = await _context.CandidateAttributeValues
            .Include(item =>
                item.AttributeDefinition)
            .Where(item =>
                item.CandidateProfileId == profileId.Value &&
                requestedIds.Contains(
                    item.AttributeDefinitionId) &&
                !item.AttributeDefinition.IsBuiltIn)
            .ToArrayAsync(cancellationToken);

        if (values.Length == 0)
        {
            return RedirectToAction(nameof(Info));
        }

        _context.CandidateAttributeValues
            .RemoveRange(values);

        await TouchAttributeUsagesAsync(
            profileId.Value,
            values
                .Select(item =>
                    item.AttributeDefinitionId)
                .ToArray(),
            cancellationToken);

        try
        {
            await _context.SaveChangesAsync(
                cancellationToken);
        }
        catch (DbUpdateConcurrencyException)
        {
            TempData["InfoError"] =
                "One or more attributes changed in another session. Try again.";
        }
        catch (DbUpdateException exception) when (
            exception.InnerException is
                SqlException { Number: 2601 or 2627 })
        {
            TempData["InfoError"] =
                "The attributes were changed in another session. Refresh and try again.";
        }

        return RedirectToAction(nameof(Info));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> SaveAttributeValue(
        [FromBody] SaveAttributeValueRequest? request,
        CancellationToken cancellationToken)
    {
        if (request is null ||
            request.AttributeDefinitionId <= 0)
        {
            return BadRequest(new
            {
                message =
                    "The submitted attribute is invalid."
            });
        }

        if (!TryDecodeRowVersion(
                request.RowVersion,
                out var originalRowVersion))
        {
            return BadRequest(new
            {
                message =
                    "The attribute version is missing or invalid."
            });
        }

        var userId = _userManager.GetUserId(User);

        if (userId is null)
        {
            return Challenge();
        }

        var profileId = await _context.CandidateProfiles
            .Where(item => item.UserId == userId)
            .Select(item => (int?)item.Id)
            .SingleOrDefaultAsync(cancellationToken);

        if (profileId is null)
        {
            return BadRequest(new
            {
                message =
                    "Complete your profile before adding information."
            });
        }

        var value = await _context.CandidateAttributeValues
            .Include(item =>
                item.AttributeDefinition)
            .SingleOrDefaultAsync(
                item =>
                    item.CandidateProfileId ==
                        profileId.Value &&
                    item.AttributeDefinitionId ==
                        request.AttributeDefinitionId,
                cancellationToken);

        if (value is null ||
            value.AttributeDefinition.IsBuiltIn)
        {
            return NotFound(new
            {
                message =
                    "The selected attribute no longer exists."
            });
        }

        _context.Entry(value)
            .Property(item => item.RowVersion)
            .OriginalValue = originalRowVersion;

        value.TextValue = null;
        value.NumberValue = null;
        value.DateValue = null;
        value.PeriodStart = null;
        value.PeriodEnd = null;
        value.BooleanValue = null;
        value.SelectedOptionId = null;

        switch (value.AttributeDefinition.DataType)
        {
            case AttributeDataType.Text:
            case AttributeDataType.MarkdownText:
                value.TextValue =
                    NormalizeText(request.TextValue);
                break;

            case AttributeDataType.ExternalImage:
                {
                    var imageUrl =
                        NormalizeText(request.TextValue);

                    if (imageUrl is not null &&
                        !IsHttpUrl(imageUrl))
                    {
                        return BadRequest(new
                        {
                            message =
                                "Image value must be a valid HTTP or HTTPS URL."
                        });
                    }

                    value.TextValue = imageUrl;
                    break;
                }

            case AttributeDataType.Number:
                value.NumberValue =
                    request.NumberValue;
                break;

            case AttributeDataType.Date:
                value.DateValue =
                    request.DateValue;
                break;

            case AttributeDataType.Period:
                if (request.PeriodStart.HasValue &&
                    request.PeriodEnd.HasValue &&
                    request.PeriodEnd.Value <
                    request.PeriodStart.Value)
                {
                    return BadRequest(new
                    {
                        message =
                            "The period end date cannot be before the start date."
                    });
                }

                value.PeriodStart =
                    request.PeriodStart;

                value.PeriodEnd =
                    request.PeriodEnd;
                break;

            case AttributeDataType.Boolean:
                value.BooleanValue =
                    request.BooleanValue ?? false;
                break;

            case AttributeDataType.SingleChoice:
                if (request.SelectedOptionId.HasValue)
                {
                    var optionExists =
                        await _context.AttributeOptions
                            .AsNoTracking()
                            .AnyAsync(
                                option =>
                                    option.Id ==
                                        request.SelectedOptionId.Value &&
                                    option.AttributeDefinitionId ==
                                        request.AttributeDefinitionId,
                                cancellationToken);

                    if (!optionExists)
                    {
                        return BadRequest(new
                        {
                            message =
                                "The selected option is invalid."
                        });
                    }
                }

                value.SelectedOptionId =
                    request.SelectedOptionId;
                break;

            default:
                return BadRequest(new
                {
                    message =
                        "The attribute data type is unsupported."
                });
        }

        try
        {
            await _context.SaveChangesAsync(
                cancellationToken);
        }
        catch (DbUpdateConcurrencyException)
        {
            var currentRowVersion =
                await _context.CandidateAttributeValues
                    .AsNoTracking()
                    .Where(item =>
                        item.CandidateProfileId ==
                            profileId.Value &&
                        item.AttributeDefinitionId ==
                            request.AttributeDefinitionId)
                    .Select(item => item.RowVersion)
                    .SingleOrDefaultAsync(
                        cancellationToken);

            return Conflict(new
            {
                message =
                    "This value was changed in another session. Reload the page before editing it again.",
                rowVersion =
                    currentRowVersion is null
                        ? string.Empty
                        : Convert.ToBase64String(
                            currentRowVersion)
            });
        }

        return Json(new
        {
            rowVersion =
                Convert.ToBase64String(
                    value.RowVersion)
        });
    }

    [HttpGet]
    public async Task<IActionResult> Edit(
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

        var model =
            profile is null
                ? new CandidateProfileEditViewModel()
                : new CandidateProfileEditViewModel
                {
                    FirstName =
                        profile.FirstName,
                    LastName =
                        profile.LastName,
                    Location =
                        profile.Location,
                    PersonalPhotoUrl =
                        profile.PersonalPhotoUrl,
                    RowVersion =
                        profile.RowVersion
                };

        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(
        CandidateProfileEditViewModel model,
        CancellationToken cancellationToken)
    {
        var userId = _userManager.GetUserId(User);

        if (userId is null)
        {
            return Challenge();
        }

        var profile = await _context.CandidateProfiles
            .SingleOrDefaultAsync(
                item => item.UserId == userId,
                cancellationToken);

        var currentPhotoUrl =
            profile?.PersonalPhotoUrl ??
            string.Empty;

        var currentPhotoPublicId =
            profile?.PersonalPhotoPublicId;

        model.PersonalPhotoUrl =
            currentPhotoUrl;

        ModelState.Remove(
            nameof(
                CandidateProfileEditViewModel
                    .PersonalPhotoUrl));

        var hasNewPhoto =
            model.PersonalPhotoFile is
            { Length: > 0 };

        if (string.IsNullOrWhiteSpace(
                currentPhotoUrl) &&
            !hasNewPhoto)
        {
            ModelState.AddModelError(
                nameof(
                    CandidateProfileEditViewModel
                        .PersonalPhotoFile),
                "Personal photo is required.");
        }

        if (profile is not null)
        {
            if (model.RowVersion is null)
            {
                ModelState.AddModelError(
                    string.Empty,
                    "The profile version is missing. Reload the page and try again.");
            }
            else if (!profile.RowVersion.SequenceEqual(
                         model.RowVersion))
            {
                model.RowVersion =
                    profile.RowVersion;

                model.PersonalPhotoUrl =
                    profile.PersonalPhotoUrl;

                ModelState.AddModelError(
                    string.Empty,
                    "The profile was changed in another session. Review the saved information and try again.");
            }
        }

        if (!ModelState.IsValid)
        {
            return View(model);
        }

        CloudinaryImageResult? uploadedPhoto =
            null;

        if (hasNewPhoto)
        {
            try
            {
                uploadedPhoto =
                    await _imageService.UploadAsync(
                        model.PersonalPhotoFile!,
                        ProfilePhotoFolder);

                model.PersonalPhotoUrl =
                    uploadedPhoto.SecureUrl;
            }
            catch (ArgumentException exception)
            {
                ModelState.AddModelError(
                    nameof(
                        CandidateProfileEditViewModel
                            .PersonalPhotoFile),
                    exception.Message);

                return View(model);
            }
            catch (Exception exception) when (
                exception is not
                    OperationCanceledException)
            {
                _logger.LogError(
                    exception,
                    "Profile photo upload failed for user {UserId}.",
                    userId);

                ModelState.AddModelError(
                    nameof(
                        CandidateProfileEditViewModel
                            .PersonalPhotoFile),
                    "The image could not be uploaded. Try again.");

                return View(model);
            }
        }

        var photoUrl =
            uploadedPhoto?.SecureUrl ??
            currentPhotoUrl;

        var photoPublicId =
            uploadedPhoto?.PublicId ??
            currentPhotoPublicId;

        if (string.IsNullOrWhiteSpace(photoUrl))
        {
            ModelState.AddModelError(
                nameof(
                    CandidateProfileEditViewModel
                        .PersonalPhotoFile),
                "Personal photo is required.");

            return View(model);
        }

        if (profile is null)
        {
            profile = new CandidateProfile
            {
                UserId = userId,
                FirstName =
                    model.FirstName.Trim(),
                LastName =
                    model.LastName.Trim(),
                Location =
                    model.Location.Trim(),
                PersonalPhotoUrl =
                    photoUrl,
                PersonalPhotoPublicId =
                    photoPublicId
            };

            _context.CandidateProfiles.Add(profile);
        }
        else
        {
            _context.Entry(profile)
                .Property(item => item.RowVersion)
                .OriginalValue =
                    model.RowVersion!;

            profile.FirstName =
                model.FirstName.Trim();

            profile.LastName =
                model.LastName.Trim();

            profile.Location =
                model.Location.Trim();

            profile.PersonalPhotoUrl =
                photoUrl;

            profile.PersonalPhotoPublicId =
                photoPublicId;
        }

        var databaseSaved = false;

        try
        {
            await _context.SaveChangesAsync(
                cancellationToken);

            databaseSaved = true;
        }
        catch (DbUpdateConcurrencyException)
        {
            var currentProfile =
                await _context.CandidateProfiles
                    .AsNoTracking()
                    .SingleOrDefaultAsync(
                        item => item.UserId == userId,
                        cancellationToken);

            if (currentProfile is null)
            {
                model.RowVersion = null;
                model.PersonalPhotoUrl =
                    string.Empty;

                ModelState.AddModelError(
                    string.Empty,
                    "The profile was deleted in another session. Select the photo again and submit to recreate it.");
            }
            else
            {
                model.RowVersion =
                    currentProfile.RowVersion;

                model.PersonalPhotoUrl =
                    currentProfile.PersonalPhotoUrl;

                ModelState.AddModelError(
                    string.Empty,
                    "The profile was changed in another session. Review the saved information and select the new photo again if needed.");
            }

            ModelState.Remove(
                nameof(
                    CandidateProfileEditViewModel
                        .PersonalPhotoUrl));

            return View(model);
        }
        catch (DbUpdateException exception) when (
            _context.Entry(profile).State ==
                EntityState.Added &&
            exception.InnerException is
                SqlException { Number: 2601 or 2627 })
        {
            var currentProfile =
                await _context.CandidateProfiles
                    .AsNoTracking()
                    .SingleOrDefaultAsync(
                        item => item.UserId == userId,
                        cancellationToken);

            if (currentProfile is null)
            {
                throw;
            }

            model.RowVersion =
                currentProfile.RowVersion;

            model.PersonalPhotoUrl =
                currentProfile.PersonalPhotoUrl;

            ModelState.Remove(
                nameof(
                    CandidateProfileEditViewModel
                        .PersonalPhotoUrl));

            ModelState.AddModelError(
                string.Empty,
                "Your profile was created in another session. Your changes were not saved. Review the saved information and try again.");

            return View(model);
        }
        finally
        {
            if (!databaseSaved &&
                uploadedPhoto is not null)
            {
                await TryDeletePhotoAsync(
                    uploadedPhoto.PublicId,
                    userId);
            }
        }

        if (uploadedPhoto is not null &&
            !string.Equals(
                currentPhotoPublicId,
                uploadedPhoto.PublicId,
                StringComparison.Ordinal))
        {
            await TryDeletePhotoAsync(
                currentPhotoPublicId,
                userId);
        }

        TempData["ProfileMessage"] =
            "Profile saved successfully.";

        return RedirectToAction(nameof(Index));
    }

    private async Task TryDeletePhotoAsync(
        string? publicId,
        string userId)
    {
        if (string.IsNullOrWhiteSpace(publicId))
        {
            return;
        }

        try
        {
            await _imageService.DeleteAsync(
                publicId);
        }
        catch (Exception exception)
        {
            _logger.LogWarning(
                exception,
                "Cloudinary profile photo cleanup failed for user {UserId}. Public ID: {PublicId}.",
                userId,
                publicId);
        }
    }

    private async Task TouchAttributeUsagesAsync(
        int candidateProfileId,
        IReadOnlyCollection<int> attributeDefinitionIds,
        CancellationToken cancellationToken)
    {
        if (attributeDefinitionIds.Count == 0)
        {
            return;
        }

        var ids = attributeDefinitionIds
            .Distinct()
            .ToArray();

        var existingUsages =
            await _context.CandidateAttributeUsages
                .Where(item =>
                    item.CandidateProfileId ==
                        candidateProfileId &&
                    ids.Contains(
                        item.AttributeDefinitionId))
                .ToDictionaryAsync(
                    item => item.AttributeDefinitionId,
                    cancellationToken);

        var usedAtUtc =
            DateTimeOffset.UtcNow;

        foreach (var definitionId in ids)
        {
            if (existingUsages.TryGetValue(
                    definitionId,
                    out var usage))
            {
                usage.LastUsedAtUtc =
                    usedAtUtc;

                continue;
            }

            _context.CandidateAttributeUsages.Add(
                new CandidateAttributeUsage
                {
                    CandidateProfileId =
                        candidateProfileId,
                    AttributeDefinitionId =
                        definitionId,
                    LastUsedAtUtc =
                        usedAtUtc
                });
        }
    }

    private static bool IsHttpUrl(
        string value)
    {
        return Uri.TryCreate(
                   value,
                   UriKind.Absolute,
                   out var uri) &&
               (uri.Scheme ==
                    Uri.UriSchemeHttps ||
                uri.Scheme ==
                    Uri.UriSchemeHttp);
    }

    private static string? NormalizeText(
        string? value)
    {
        return string.IsNullOrWhiteSpace(value)
            ? null
            : value.Trim();
    }

    private static bool TryDecodeRowVersion(
        string? encodedRowVersion,
        out byte[] rowVersion)
    {
        rowVersion = Array.Empty<byte>();

        if (string.IsNullOrWhiteSpace(
                encodedRowVersion))
        {
            return false;
        }

        try
        {
            rowVersion =
                Convert.FromBase64String(
                    encodedRowVersion);

            return rowVersion.Length == 8;
        }
        catch (FormatException)
        {
            return false;
        }
    }

    public sealed class SaveAttributeValueRequest
    {
        public int AttributeDefinitionId
        {
            get;
            init;
        }

        public string RowVersion
        {
            get;
            init;
        } = string.Empty;

        public string? TextValue
        {
            get;
            init;
        }

        public decimal? NumberValue
        {
            get;
            init;
        }

        public DateOnly? DateValue
        {
            get;
            init;
        }

        public DateOnly? PeriodStart
        {
            get;
            init;
        }

        public DateOnly? PeriodEnd
        {
            get;
            init;
        }

        public bool? BooleanValue
        {
            get;
            init;
        }

        public int? SelectedOptionId
        {
            get;
            init;
        }
    }
}