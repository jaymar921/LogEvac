namespace LogEvac.Services;

using Chronos.Log.Cleaner.Services;
using LogEvac.Contexts;
using LogEvac.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

public class SerilogCleanupService : BaseCleanupService, ICleanupService
{
    public SerilogCleanupService(IOptions<LogEvacSettings> detail, SerilogDatabaseContext serilogDatabaseContext, ILogger<SerilogCleanupService> logger) : base(detail)
    {
        _serilogDatabaseContext = serilogDatabaseContext;
        _logger = logger;
    }

    private readonly SerilogDatabaseContext _serilogDatabaseContext;
    private readonly ILogger<SerilogCleanupService> _logger;

    public async Task ExecuteCleanupAsync()
    {
        try
        {
            DateTime cutoffDate;

            if (Detail.CleanUpDataOlderThanWeeksValue.HasValue && Detail.CleanUpDataOlderThanWeeksValue > 0)
            {
                cutoffDate = DateTime.Now.AddDays((-7 * Detail.CleanUpDataOlderThanWeeksValue.Value));
            }
            else if (Detail.CleanUpDataOlderThanDaysValue.HasValue && Detail.CleanUpDataOlderThanDaysValue > 0)
            {
                cutoffDate = DateTime.Now.AddDays((-1 * Detail.CleanUpDataOlderThanDaysValue.Value));
            }
            else if (Detail.CleanUpDataOlderThanMinutesValue.HasValue && Detail.CleanUpDataOlderThanMinutesValue > 0)
            {
                cutoffDate = DateTime.Now.AddMinutes((-1 * Detail.CleanUpDataOlderThanMinutesValue.Value));
            }
            else
            {
                cutoffDate = DateTime.Now.AddMonths((-1 * Detail.CleanUpDataOlderThanMonthsValue));
            }

            var filteredItems = await _serilogDatabaseContext.LogEvents.Where(
                    l => l.ApplicationName == Detail.ApplicationName &&
                    l.Environment == Detail.Environment &&
                    l.TimeStamp <= cutoffDate
            ).ToListAsync();

            _serilogDatabaseContext.LogEvents.RemoveRange(filteredItems);
            await _serilogDatabaseContext.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"[ERROR][SerilogCleanupService.ExecuteCleanupAsync] encountered an error.");
            // log detailed error
            _logger.LogError($"[ERROR][SerilogCleanupService.ExecuteCleanupAsync] Error Message: {ex.Message}");
            _logger.LogError($"[ERROR][SerilogCleanupService.ExecuteCleanupAsync] Stack Trace: {ex.StackTrace}");
            throw;
        }
    }
}
