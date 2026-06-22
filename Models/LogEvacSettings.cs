namespace LogEvac.Models;

public class LogEvacSettings
{
    public string ApplicationName { get; set; } = string.Empty;
    public string Environment { get; set; } = string.Empty;
    public string LoggingTable { get; set; } = "LogEvents";
    public int CheckingFequencyInHours { get; set; } = 2;
    public int CheckingFrequencyInMinutes { get; set; } = 0;
    public int CleanUpDataOlderThanMonthsValue { get; set; } = 2;
    public int? CleanUpDataOlderThanWeeksValue { get; set; }
    public int? CleanUpDataOlderThanDaysValue { get; set; }
    public int? CleanUpDataOlderThanMinutesValue { get; set; }
}
