using AuthServer.Core.UnitOfWork;

namespace AuthServer.Data.UnitOfWork;

public class UnitOfWork(AppDbContext context) : IUnitOfWork
{

    public void SaveChanges()
    {
        context.SaveChanges();
    }

    public async Task SaveChangesAsync()
    {
        await context.SaveChangesAsync();
    }
}
