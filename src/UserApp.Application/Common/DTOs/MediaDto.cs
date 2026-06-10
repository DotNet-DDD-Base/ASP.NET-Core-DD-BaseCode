namespace UserApp.Application.Media;

public class MediaDto
{
    public Guid Id { get; set; }
    public string Url { get; set; } = string.Empty;
    public string OriginalName { get; set; } = string.Empty;
}