using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Webp;
using SixLabors.ImageSharp.Processing;
using UserApp.Application.Common.Interfaces;
using UserApp.Application.Common.Media;
using UserApp.Application.Media;
using UserApp.Domain.Media;

namespace UserApp.Infrastructure.Media;

public class MediaService : IMediaService
{
    private const long MaxFileSizeBytes = 5 * 1024 * 1024;
    private static readonly string[] AllowedExtensions = [".jpg", ".jpeg", ".png", ".webp"];
    private static readonly string[] AllowedMimeTypes = ["image/jpeg", "image/png", "image/webp"];

    private readonly IMediaRepository _repo;

    public MediaService(IMediaRepository repo)
    {
        _repo = repo;
    }

    public async Task<List<MediaDto>> GetAsync(string entityName, Guid entityId)
    {
        var media = await _repo.GetByEntityAsync(entityName, entityId);

        return media.Select(x => new MediaDto
        {
            Url = x.Url,
            OriginalName = x.OriginalName
        }).ToList();
    }

    public async Task UploadAsync(string entityName, Guid entityId, MediaFileInput file)
    {
        if (file == null || file.Data.Length == 0)
            return;

        Validate(file);

        var safeName = BuildSafeFileName(file.FileName);
        var path = $"/uploads/{safeName}";
        var fullPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", safeName);

        Directory.CreateDirectory(Path.GetDirectoryName(fullPath)!);

        await using var stream = new MemoryStream(file.Data);
        using var image = await Image.LoadAsync(stream);

        image.Mutate(x => x.Resize(new ResizeOptions
        {
            Mode = ResizeMode.Max,
            Size = new Size(1600, 1600)
        }));

        await image.SaveAsWebpAsync(fullPath, new WebpEncoder { Quality = 80 });

        await _repo.AddAsync(new MediaFile
        {
            EntityName = entityName,
            EntityId = entityId,
            Url = path,
            OriginalName = file.FileName,
            MimeType = file.ContentType
        });

        await _repo.SaveChangesAsync();
    }

    public async Task DeleteAsync(Guid mediaId)
    {
        var media = await _repo.GetByIdAsync(mediaId);
        if (media == null) return;

        _repo.Remove(media);
        await _repo.SaveChangesAsync();
    }

    public async Task<string?> GetLatestUrlAsync(string entityName, Guid entityId)
    {
        var media = await _repo.GetLatestByEntityAsync(entityName, entityId);
        return media?.Url;
    }

    private static void Validate(MediaFileInput file)
    {
        if (file == null || file.Data == null || file.Data.Length == 0)
            throw new InvalidOperationException("No file was uploaded.");

        if (file.Data.Length > MaxFileSizeBytes)
            throw new InvalidOperationException("Image size must be 5 MB or smaller.");

        var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
        var mime = (file.ContentType ?? string.Empty).ToLowerInvariant();

        if (!AllowedExtensions.Contains(extension))
            throw new InvalidOperationException("Only JPG, PNG, and WEBP files are allowed.");

        if (!AllowedMimeTypes.Contains(mime))
            throw new InvalidOperationException("Invalid image content type.");
    }

    private static string BuildSafeFileName(string fileName)
    {
        var baseName = Path.GetFileNameWithoutExtension(fileName)
            .Replace(" ", "-")
            .ToLowerInvariant();

        var sanitized = new string(baseName.Where(char.IsLetterOrDigit).ToArray());
        return $"{(string.IsNullOrWhiteSpace(sanitized) ? "image" : sanitized)}-{Guid.NewGuid():N}.webp";
    }
}