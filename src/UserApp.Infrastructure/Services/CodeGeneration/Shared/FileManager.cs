using System;
using System.IO;

namespace UserApp.Infrastructure.Services.CodeGeneration.Shared;

public class FileManager
{
    public void EnsureDirectory(string path)
    {
        if (string.IsNullOrWhiteSpace(path))
            throw new ArgumentException("Directory path is required.", nameof(path));

        Directory.CreateDirectory(path);
    }

    public void WriteFile(string filePath, string content)
    {
        var directory = Path.GetDirectoryName(filePath);
        if (directory is not null)
            EnsureDirectory(directory);

        File.WriteAllText(filePath, content);
    }

    public string ReadFile(string filePath)
    {
        return File.ReadAllText(filePath);
    }

    public bool FileContains(string filePath, string searchText)
    {
        if (!File.Exists(filePath))
            return false;

        return File.ReadAllText(filePath).Contains(searchText);
    }
}
