namespace RentGubakhaBot.Services;

internal interface IStatisticsService : IDisposable
{
    Task InitializeAsync(CancellationToken cancellationToken);

    Task TrackAsync(
        long userId,
        string? username,
        string action,
        CancellationToken cancellationToken);
}
