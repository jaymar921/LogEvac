namespace LogEvac.Models;

using System;

public class LogEvent
{
    public int Id { get; set; }
    public string? Message { get; set; }
    public string? MessageTemplate { get; set; }
    public string? Level { get; set; }
    public DateTime? TimeStamp { get; set; }
    public string? Exception { get; set; }
    public string? Properties { get; set; }
    public string? ApplicationName { get; set; }
    public string? Environment { get; set; }
}
