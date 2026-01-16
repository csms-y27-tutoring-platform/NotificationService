using NotificationService.Application.Domain.Entities;

namespace NotificationService.Application.Abstractions.Repositories;

public interface INotificationRepository
{
    public Task<Notification?> FindByIdAsync(Guid id, CancellationToken cancellationToken);

    public Task<IReadOnlyCollection<Notification>> FindByUserIdAsync(string userId, CancellationToken cancellationToken);

    public Task<IReadOnlyCollection<Notification>> FindUnreadByUserIdAsync(string userId, CancellationToken cancellationToken);

    public Task AddAsync(Notification notification, CancellationToken cancellationToken);

    public Task UpdateAsync(Notification notification, CancellationToken cancellationToken);

    public Task<int> CountUnreadByUserIdAsync(string userId, CancellationToken cancellationToken);
}