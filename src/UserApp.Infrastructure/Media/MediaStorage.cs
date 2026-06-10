using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Webp;
using SixLabors.ImageSharp.Processing;

namespace UserApp.Infrastructure.Media;

public class MediaStorage
{
    private const long MaxFileSizeBytes = 5 * 1024 * 1024; // 5 MB
    private static readonly string[] AllowedExtensions = [".jpg", ".jpeg", ".png", ".webp"];
    private static readonly string[] AllowedMimeTypes = ["image/jpeg", "image/png", "image/webp"];

    private readonly string _rootPath;

    public MediaStorage(IWebHostEnvironment env)
    {
        _rootPath = Path.Combine(env.WebRootPath, "uploads");
    }

    public async Task<string> SaveAsync(IFormFile file)
    {
        Validate(file.Length, file.FileName, file.ContentType);

        Directory.CreateDirectory(_rootPath);

        var safeName = BuildSafeFileName(file.FileName);
        var fullPath = Path.Combine(_rootPath, safeName);

        await using var stream = file.OpenReadStream();
        using var image = await Image.LoadAsync(stream);

        image.Mutate(x => x.Resize(new ResizeOptions
        {
            Mode = ResizeMode.Max,
            Size = new Size(1600, 1600)
        }));

        await image.SaveAsWebpAsync(fullPath, new WebpEncoder
        {
            Quality = 80
        });

        return "/uploads/" + safeName;
    }

    public async Task<string> SaveBytesAsync(byte[] data, string fileName, string contentType)
    {
        Validate(data.LongLength, fileName, contentType);

        Directory.CreateDirectory(_rootPath);

        var safeName = BuildSafeFileName(fileName);
        var fullPath = Path.Combine(_rootPath, safeName);

        await using var stream = new MemoryStream(data);
        using var image = await Image.LoadAsync(stream);

        image.Mutate(x => x.Resize(new ResizeOptions
        {
            Mode = ResizeMode.Max,
            Size = new Size(1600, 1600)
        }));

        await image.SaveAsWebpAsync(fullPath, new WebpEncoder { Quality = 80 });

        return "/uploads/" + safeName;
    }

    private static void Validate(long length, string fileName, string contentType)
    {
        if (length <= 0)
            throw new InvalidOperationException("No file was uploaded.");

        if (length > MaxFileSizeBytes)
            throw new InvalidOperationException("Image size must be 5 MB or smaller.");

        var extension = Path.GetExtension(fileName).ToLowerInvariant();
        var mime = (contentType ?? string.Empty).ToLowerInvariant();

        if (!AllowedExtensions.Contains(extension))
            throw new InvalidOperationException("Only JPG, PNG, and WEBP files are allowed.");

        if (!AllowedMimeTypes.Contains(mime))
            throw new InvalidOperationException("Invalid image content type.");
    }

    private static string BuildSafeFileName(string fileName)
    {
        var extension = Path.GetExtension(fileName).ToLowerInvariant();
        var safeBase = Path.GetFileNameWithoutExtension(fileName)
            .Replace(" ", "-")
            .ToLowerInvariant();

        var sanitized = new string(safeBase.Where(c => char.IsLetterOrDigit(c) || c == '-' || c == '_').ToArray());
        if (string.IsNullOrWhiteSpace(sanitized))
            sanitized = "image";

        return $"{sanitized}-{Guid.NewGuid():N}.webp";
    }
}