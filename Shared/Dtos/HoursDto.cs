namespace ZDC.Shared.Dtos;

public class HoursDto
{
    public int Month { get; set; }
    public int Year { get; set; }
    public TimeSpan LocalHours { get; set; }
    public string LocalHoursString => $"{LocalHours.Days * 24 + LocalHours.Hours}h {LocalHours.Minutes}m";
    public TimeSpan TraconHours { get; set; }
    public string TraconHoursString => $"{TraconHours.Days * 24 + TraconHours.Hours}h {TraconHours.Minutes}m";
    public TimeSpan CenterHours { get; set; }
    public string CenterHoursString => $"{CenterHours.Days * 24 + CenterHours.Hours}h {CenterHours.Minutes}m";
    public TimeSpan TotalHours => LocalHours + TraconHours + CenterHours;
    public string TotalHoursString => $"{TotalHours.Days * 24 + TotalHours.Hours}h {TotalHours.Minutes}m";
}
