using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using QuestionService.Domain.Interfaces.Entity;

namespace QuestionService.DAL.Interceptors;

public class DateInterceptor : SaveChangesInterceptor
{
    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(DbContextEventData eventData,
        InterceptionResult<int> result,
        CancellationToken cancellationToken = new())
    {
        var dbContext = eventData.Context;
        if (dbContext == null) return base.SavingChangesAsync(eventData, result, cancellationToken);

        var entries = dbContext.ChangeTracker.Entries<IAuditable>();

        foreach (var entry in entries)
            switch (entry.State)
            {
                case EntityState.Added:
                    entry.Property(x => x.CreatedAt).CurrentValue = DateTime.UtcNow;
                    break;

                case EntityState.Modified:
                    entry.Property(x => x.LastModifiedAt).CurrentValue = DateTime.UtcNow;
                    break;
            }

        return base.SavingChangesAsync(eventData, result, cancellationToken);
    }

    public override InterceptionResult<int> SavingChanges(DbContextEventData eventData, InterceptionResult<int> result)
    {
        var dbContext = eventData.Context;
        if (dbContext == null) return base.SavingChanges(eventData, result);

        var entries = dbContext.ChangeTracker.Entries<IAuditable>();
        foreach (var entry in entries)
            switch (entry.State)
            {
                case EntityState.Added:
                    entry.Property(x => x.CreatedAt).CurrentValue = DateTime.UtcNow;
                    break;

                case EntityState.Modified:
                    entry.Property(x => x.LastModifiedAt).CurrentValue = DateTime.UtcNow;
                    break;
            }

        return base.SavingChanges(eventData, result);
    }
}