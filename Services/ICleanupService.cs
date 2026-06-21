namespace Chronos.Log.Cleaner.Services;

using LogEvac.Models;

public interface ICleanupService
{
    public LogEvacSettings Detail { get; }
    public Task ExecuteCleanupAsync();
}