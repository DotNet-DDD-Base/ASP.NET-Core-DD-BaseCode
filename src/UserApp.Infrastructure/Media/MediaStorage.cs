using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;

namespace UserApp.Infrastructure.Media;

public class MediaStorage
{
    private readonly string _rootPath;

    public MediaStorage(IWebHostEnvironment env)
    {
        _rootPath = Path.Combine(env.WebRootPath, "uploads");
    }

    public async Task<string> SaveAsync(IFormFile file)
    {
        var fileName = Guid.NewGuid() + Path.GetExtension(file.FileName);

        Directory.CreateDirectory(_rootPath);

        var fullPath = Path.Combine(_rootPath, fileName);

        using var stream = new FileStream(fullPath, FileMode.Create);
        await file.CopyToAsync(stream);

        // IMPORTANT: return URL, NOT physical path
        return "/uploads/" + fileName;
    }
}