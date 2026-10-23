namespace RentGubakhaBot.Services;

internal sealed class DisabledStatisticsService : IStatisticsService
{
    public static DisabledStatisticsService Instance { get; } = new();

    private DisabledStatisticsService()
    {
    }

    public Task InitializeAsync(CancellationToken cancellationToken) => Task.CompletedTask;

    public Task TrackAsync(
        long userId,
        string? username,
        string action,
        CancellationToken cancellationToken) => Task.CompletedTask;

    public void Dispose()
    {
    }
}
