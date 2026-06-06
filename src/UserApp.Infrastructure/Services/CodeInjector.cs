namespace UserApp.Infrastructure.Services;

public static class CodeInjector
{
    public static void InjectBetween(string filePath, string startTag, string endTag, string newCode)
    {
        var content = File.ReadAllText(filePath);

        if (!content.Contains(startTag) || !content.Contains(endTag))
            throw new Exception("Injection markers not found");

        // prevent duplicate injection
        if (content.Contains(newCode.Trim()))
            return;

        var startIndex = content.IndexOf(startTag) + startTag.Length;
        var endIndex = content.IndexOf(endTag);

        var before = content[..startIndex];
        var middle = "\n" + newCode + "\n";
        var after = content[endIndex..];

        File.WriteAllText(filePath, before + middle + after);
    }
}