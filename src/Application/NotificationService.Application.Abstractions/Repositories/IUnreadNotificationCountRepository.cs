namespace NotificationService.Application.Abstractions.Repositories;

public interface IUnreadNotificationCountRepository
{
    public Task<int?> GetCountAsync(Guid userId, CancellationToken cancellationToken);

    public Task UpdateCountAsync(Guid userId, int count, CancellationToken cancellationToken);

    public Task IncrementCountAsync(Guid userId, CancellationToken cancellationToken);

    public Task DecrementCountAsync(Guid userId, CancellationToken cancellationToken);

    public Task ResetCountAsync(Guid userId, CancellationToken cancellationToken);
}