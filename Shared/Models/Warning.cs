namespace ZDC.Shared.Models;

public class Warning
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public User? User { get; set; }
    public int SubmitterId { get; set; }
    public User? Submitter { get; set; }
    public int Month { get; set; }
    public int Year { get; set; }
}