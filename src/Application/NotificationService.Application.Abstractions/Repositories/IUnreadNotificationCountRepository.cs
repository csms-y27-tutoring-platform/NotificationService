namespace NotificationService.Application.Abstractions.Repositories;

public interface IUnreadNotificationCountRepository
{
    public Task<int?> GetCountAsync(string userId, CancellationToken cancellationToken);

    public Task UpdateCountAsync(string userId, int count, CancellationToken cancellationToken);

    public Task IncrementCountAsync(string userId, CancellationToken cancellationToken);

    public Task DecrementCountAsync(string userId, CancellationToken cancellationToken);

    public Task ResetCountAsync(string userId, CancellationToken cancellationToken);
}