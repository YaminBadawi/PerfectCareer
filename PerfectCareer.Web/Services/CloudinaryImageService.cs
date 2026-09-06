using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;

namespace PerfectCareer.Web.Services;

public sealed class CloudinaryImageService
{
    private const long MaxImageSizeInBytes =
        5 * 1024 * 1024;

    private static readonly HashSet<string> AllowedContentTypes =
        new(StringComparer.OrdinalIgnoreCase)
        {
            "image/jpeg",
            "image/png",
            "image/webp"
        };

    private static readonly HashSet<string> AllowedExtensions =
        new(StringComparer.OrdinalIgnoreCase)
        {
            ".jpg",
            ".jpeg",
            ".png",
            ".webp"
        };

    private readonly string? _cloudName;
    private readonly string? _apiKey;
    private readonly string? _apiSecret;

    public CloudinaryImageService(
        IConfiguration configuration)
    {
        _cloudName =
            configuration["Cloudinary:CloudName"];

        _apiKey =
            configuration["Cloudinary:ApiKey"];

        _apiSecret =
            configuration["Cloudinary:ApiSecret"];
    }

    public async Task<CloudinaryImageResult> UploadAsync(
        IFormFile image,
        string folder)
    {
        ValidateImage(image);

        if (string.IsNullOrWhiteSpace(folder))
        {
            throw new ArgumentException(
                "The Cloudinary folder is required.",
                nameof(folder));
        }

        var cloudinary =
            CreateCloudinary();

        await using var stream =
            image.OpenReadStream();

        var uploadParameters =
            new ImageUploadParams
            {
                File = new FileDescription(
                    Path.GetFileName(image.FileName),
                    stream),

                Folder = folder.Trim().Trim('/'),
                UseFilename = false,
                UniqueFilename = true,
                Overwrite = false
            };

        var uploadResult =
            await cloudinary.UploadAsync(
                uploadParameters);

        if (uploadResult.Error is not null)
        {
            throw new InvalidOperationException(
                uploadResult.Error.Message);
        }

        var secureUrl =
            uploadResult.SecureUrl?.ToString();

        if (string.IsNullOrWhiteSpace(secureUrl) ||
            string.IsNullOrWhiteSpace(
                uploadResult.PublicId))
        {
            throw new InvalidOperationException(
                "Cloudinary did not return the uploaded image information.");
        }

        return new CloudinaryImageResult(
            secureUrl,
            uploadResult.PublicId);
    }

    public async Task DeleteAsync(
        string? publicId)
    {
        if (string.IsNullOrWhiteSpace(publicId))
        {
            return;
        }

        var cloudinary =
            CreateCloudinary();

        var deleteParameters =
            new DeletionParams(publicId)
            {
                Invalidate = true
            };

        var deleteResult =
            await cloudinary.DestroyAsync(
                deleteParameters);

        if (deleteResult.Error is not null)
        {
            throw new InvalidOperationException(
                deleteResult.Error.Message);
        }
    }

    private Cloudinary CreateCloudinary()
    {
        if (string.IsNullOrWhiteSpace(_cloudName) ||
            string.IsNullOrWhiteSpace(_apiKey) ||
            string.IsNullOrWhiteSpace(_apiSecret))
        {
            throw new InvalidOperationException(
                "Cloudinary configuration is missing.");
        }

        var account =
            new Account(
                _cloudName,
                _apiKey,
                _apiSecret);

        var cloudinary =
            new Cloudinary(account);

        cloudinary.Api.Secure = true;

        return cloudinary;
    }

    private static void ValidateImage(
        IFormFile image)
    {
        if (image is null ||
            image.Length == 0)
        {
            throw new ArgumentException(
                "Select an image to upload.",
                nameof(image));
        }

        if (image.Length >
            MaxImageSizeInBytes)
        {
            throw new ArgumentException(
                "The image must not exceed 5 MB.",
                nameof(image));
        }

        if (!AllowedContentTypes.Contains(
                image.ContentType))
        {
            throw new ArgumentException(
                "Only JPG, PNG, and WEBP images are allowed.",
                nameof(image));
        }

        var extension =
            Path.GetExtension(image.FileName);

        if (!AllowedExtensions.Contains(
                extension))
        {
            throw new ArgumentException(
                "The image extension is not supported.",
                nameof(image));
        }
    }
}

public sealed record CloudinaryImageResult(
    string SecureUrl,
    string PublicId);