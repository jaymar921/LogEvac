namespace LogEvac.Contexts;

using Microsoft.EntityFrameworkCore;

public abstract  class BaseDatabaseContext<T> : DbContext where T : DbContext
{
    public BaseDatabaseContext(DbContextOptions<T> options) : base(options)
    {
    }
}
