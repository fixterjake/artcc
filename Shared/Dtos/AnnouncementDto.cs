namespace ZDC.Shared.Dtos;

public class AnnouncementDto
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public int UserId { get; set; }
    public UserDto? User { get; set; }
    public DateTimeOffset Updated { get; set; }
}