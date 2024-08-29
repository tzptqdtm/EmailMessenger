using EmailMessenger.Models.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace EmailMessenger.Data.DataContexts;

public interface IDataSource : IDisposable
{
    DbSet<Message> Messages { get; }
    DbSet<Recipient> Recipients { get; }
    DbSet<MessageEvent> MessageEvents { get; }
    EntityEntry<TEntity> Entry<TEntity>(TEntity entity) where TEntity : class;
    int SaveChanges();
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default(CancellationToken));
}