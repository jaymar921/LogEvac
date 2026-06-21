namespace LogEvac.Extensions;

using Chronos.Log.Cleaner.Services;
using Hangfire;
using LogEvac.Contexts;
using LogEvac.Models;
using LogEvac.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

public static class Initializer
{
    /// <summary>
    /// Default db connection string key for serilog.
    /// </summary>
    private static string LoggingDbKey = "LoggingDb";

    /// <summary>
    /// Configuration key that will be read from appsettings.json.
    /// </summary>
    private static string SettingsKey = "LogEvacSettings";

    public static void InitializeLogEvac(this IHostApplicationBuilder builder,
                string? loggingDbConnectiontionStringKey = null)
    {
        string? logConStr = string.IsNullOrEmpty(loggingDbConnectiontionStringKey) ?
            builder.Configuration.GetConnectionString(LoggingDbKey) :
            builder.Configuration.GetConnectionString(loggingDbConnectiontionStringKey);

        /// If nothing is configured, scheduler will not be initialized.
        if (string.IsNullOrEmpty(logConStr)) { return; }

        builder.Services.Configure<LogEvacSettings>(builder.Configuration.GetSection(SettingsKey));

        LogEvacSettings detail = new();
        builder.Configuration.GetSection(SettingsKey).Bind(detail);

        /// If the App name is not set, the service will not be initialized.
        if (string.IsNullOrEmpty(detail.ApplicationName)) { return; }

        // Serilog items.
        builder.Services.AddDbContext<SerilogDatabaseContext>(options => options.UseSqlServer(logConStr));
        builder.Services.AddScoped<ICleanupService, SerilogCleanupService>();
    }

    /// <summary>
    /// Initialize the job that starts the cleanup job.
    /// </summary>
    /// <param name="app"></param>
    public static void ScheduleLogEvacJob(this WebApplication app)
    {
        LogEvacSettings detail = new LogEvacSettings();
        app.Configuration.GetSection(SettingsKey).Bind(detail);
        RecurringJob.RemoveIfExists("LogEvac Data Cleanup");

        string cron = $"0 */{detail.CheckingFequencyInHours} * * *";

        if(detail.CheckingFrequencyInMinutes > 0)
        {
            cron = $"*/{detail.CheckingFrequencyInMinutes} * * * *";
        }

        RecurringJob.AddOrUpdate<ICleanupService>("LogEvac Data Cleanup", srv => srv.ExecuteCleanupAsync(), cron);
    }

}
