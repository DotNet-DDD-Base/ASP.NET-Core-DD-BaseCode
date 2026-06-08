using System;
using System.IO;
using System.Linq;
using UserApp.Infrastructure.Services.CodeGeneration.Shared;

namespace UserApp.Infrastructure.Services.CodeGeneration;

public class ProgramUpdater
{
    private readonly FileManager _files;
    private readonly PathProvider _paths;

    public ProgramUpdater(FileManager files, PathProvider paths)
    {
        _files = files;
        _paths = paths;
    }

    public void Update(string name)
    {
        var file = Path.Combine(_paths.SrcRoot, "UserApp.Web", "Program.cs");

        EnsureUsing(file, $"using UserApp.Domain.{name}s;");
        EnsureUsing(file, $"using UserApp.Application.{name}s;");
        EnsureUsing(file, $"using UserApp.Application.{name}s.Interfaces;");

        var repoLine = $"builder.Services.AddScoped<I{name}Repository, {name}Repository>();";
        var serviceLine = $"builder.Services.AddScoped<I{name}Service, {name}Service>();";

        if (!FileContainsNormalized(file, repoLine))
        {
            InsertIntoBlock(file,
                "// <AUTO-REPOSITORIES-START>",
                "// <AUTO-REPOSITORIES-END>",
                repoLine);
        }

        if (!FileContainsNormalized(file, serviceLine))
        {
            InsertIntoBlock(file,
                "// <AUTO-SERVICES-START>",
                "// <AUTO-SERVICES-END>",
                serviceLine);
        }
    }

    private bool FileContainsNormalized(string filePath, string value)
    {
        var content = _files.ReadFile(filePath);
        return content.Replace(" ", string.Empty).Contains(value.Replace(" ", string.Empty));
    }

    private void EnsureUsing(string filePath, string usingLine)
    {
        var lines = File.ReadAllLines(filePath);
        if (Array.Exists(lines, x => x.Trim() == usingLine))
            return;

        var lastUsing = Array.FindLastIndex(lines, x => x.Trim().StartsWith("using"));
        if (lastUsing == -1)
            throw new InvalidOperationException("No using statement found in Program.cs");

        var updated = new string[lines.Length + 1];
        Array.Copy(lines, updated, lastUsing + 1);
        updated[lastUsing + 1] = usingLine;
        Array.Copy(lines, lastUsing + 1, updated, lastUsing + 2, lines.Length - lastUsing - 1);

        File.WriteAllLines(filePath, updated);
    }

    private void InsertIntoBlock(string filePath, string start, string end, string content)
    {
        var lines = File.ReadAllLines(filePath).ToList();

        var startIndex = lines.FindIndex(x => x.Contains(start));
        var endIndex = lines.FindIndex(x => x.Contains(end));

        if (startIndex == -1 || endIndex == -1)
            throw new InvalidOperationException("Block markers not found in Program.cs");

        if (lines.Skip(startIndex + 1).Take(endIndex - startIndex - 1).Any(x => x.Trim() == content.Trim()))
            return;

        lines.Insert(endIndex, content);
        File.WriteAllLines(filePath, lines);
    }
}
