namespace UserApp.Application.Permissions.DTOs;

public class PermissionDto
{
    public Guid Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; }
}