namespace Chronos.Log.Cleaner.Services;

using LogEvac.Models;
using Microsoft.Extensions.Options;

public abstract class BaseCleanupService(IOptions<LogEvacSettings> detail)
{
    public LogEvacSettings Detail => detail.Value;
}