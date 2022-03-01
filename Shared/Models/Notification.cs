﻿namespace ZDC.Shared.Models;

public class Notification
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public User User { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public bool Read { get; set; }
    public DateTime Timestamp { get; set; }

    public Notification()
    {
        Timestamp = DateTime.UtcNow;
    }
}