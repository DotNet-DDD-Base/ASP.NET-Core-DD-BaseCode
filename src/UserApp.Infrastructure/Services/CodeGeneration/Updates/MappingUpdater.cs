using System;
using System.IO;
using System.Linq;
using UserApp.Infrastructure.Services.CodeGeneration.Shared;

namespace UserApp.Infrastructure.Services.CodeGeneration;

public class MappingUpdater
{
    private readonly FileManager _files;
    private readonly PathProvider _paths;

    public MappingUpdater(FileManager files, PathProvider paths)
    {
        _files = files;
        _paths = paths;
    }

    public void Update(string name)
    {
        var file = Path.Combine(_paths.SrcRoot, "UserApp.Web", "Mapping", "MappingProfile.cs");

        EnsureUsing(file, $"using UserApp.Domain.{name}s;");
        EnsureUsing(file, "using UserApp.Web.ViewModels;");

        var content = _files.ReadFile(file);
        if (content.Contains($"CreateMap<{name}, {name}ViewModel>"))
            return;

        InsertIntoBlock(file,
            "// <AUTO-MAPPINGS-START>",
            "// <AUTO-MAPPINGS-END>",
            $"CreateMap<{name}, {name}ViewModel>();\nCreateMap<{name}ViewModel, {name}>();");
    }

    private void EnsureUsing(string filePath, string usingLine)
    {
        var lines = File.ReadAllLines(filePath);
        if (Array.Exists(lines, x => x.Trim() == usingLine))
            return;

        var lastUsing = Array.FindLastIndex(lines, x => x.Trim().StartsWith("using"));
        if (lastUsing == -1)
            throw new InvalidOperationException("No using statement found in MappingProfile.cs");

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
            throw new InvalidOperationException("Block markers not found in MappingProfile.cs");

        if (lines.Skip(startIndex + 1).Take(endIndex - startIndex - 1).Any(x => x.Trim() == content.Trim()))
            return;

        lines.Insert(endIndex, content);
        File.WriteAllLines(filePath, lines);
    }
}
