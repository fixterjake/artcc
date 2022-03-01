namespace ZDC.Shared.Dtos;

public class NewsDto
{
    public int Id { get; set; }
    public string Title { get; set; }
    public string Content { get; set; }
    public int UserId { get; set; }
    public UserDto User { get; set; }
    public DateTime Updated { get; set; }
}