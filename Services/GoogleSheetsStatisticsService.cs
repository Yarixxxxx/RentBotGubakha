using System.Globalization;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Google.Apis.Sheets.v4;
using Google.Apis.Sheets.v4.Data;
using RentGubakhaBot.Configuration;

namespace RentGubakhaBot.Services;

internal sealed class GoogleSheetsStatisticsService : IStatisticsService
{
    private const string ApplicationName = "Rent Gubakha Bot";

    private readonly SheetsService _sheets;
    private readonly string _spreadsheetId;
    private readonly string _worksheet;

    private GoogleSheetsStatisticsService(
        SheetsService sheets,
        string spreadsheetId,
        string worksheet)
    {
        _sheets = sheets;
        _spreadsheetId = spreadsheetId;
        _worksheet = worksheet;
    }

    public static async Task<IStatisticsService> CreateAsync(
        BotOptions options,
        CancellationToken cancellationToken)
    {
        if (!options.IsStatisticsEnabled)
            return DisabledStatisticsService.Instance;

        GoogleCredential credential;
        if (options.GoogleCredentialsJson is { } credentialsJson)
        {
            credential = CredentialFactory
                .FromJson<ServiceAccountCredential>(credentialsJson)
                .ToGoogleCredential();
        }
        else
        {
            credential = await GoogleCredential.GetApplicationDefaultAsync(cancellationToken);
        }

        if (credential.IsCreateScopedRequired)
            credential = credential.CreateScoped(SheetsService.Scope.Spreadsheets);

        var sheets = new SheetsService(new BaseClientService.Initializer
        {
            ApplicationName = ApplicationName,
            HttpClientInitializer = credential
        });

        return new GoogleSheetsStatisticsService(
            sheets,
            options.GoogleSheetsSpreadsheetId!,
            options.GoogleSheetsWorksheet);
    }

    public async Task InitializeAsync(CancellationToken cancellationToken)
    {
        var header = new ValueRange
        {
            Values =
            [
                ["Время (UTC)", "Идентификатор", "Имя пользователя", "Действие"]
            ]
        };

        var updateHeader = _sheets.Spreadsheets.Values.Update(
            header,
            _spreadsheetId,
            Range("A1:D1"));
        updateHeader.ValueInputOption =
            SpreadsheetsResource.ValuesResource.UpdateRequest.ValueInputOptionEnum.RAW;
        await updateHeader.ExecuteAsync(cancellationToken);

        var uniqueUsers = new ValueRange
        {
            Values =
            [
                ["Кол-во пользователей"],
                ["=COUNTUNIQUE(B2:B)"]
            ]
        };

        var updateUniqueUsers = _sheets.Spreadsheets.Values.Update(
            uniqueUsers,
            _spreadsheetId,
            Range("F1:F2"));
        updateUniqueUsers.ValueInputOption =
            SpreadsheetsResource.ValuesResource.UpdateRequest.ValueInputOptionEnum.USERENTERED;
        await updateUniqueUsers.ExecuteAsync(cancellationToken);
    }

    public async Task TrackAsync(
        long userId,
        string? username,
        string action,
        CancellationToken cancellationToken)
    {
        var row = new ValueRange
        {
            Values =
            [
                [
                    DateTimeOffset.UtcNow.ToString("O", CultureInfo.InvariantCulture),
                    userId.ToString(CultureInfo.InvariantCulture),
                    string.IsNullOrWhiteSpace(username) ? string.Empty : $"@{username}",
                    action
                ]
            ]
        };

        var append = _sheets.Spreadsheets.Values.Append(
            row,
            _spreadsheetId,
            Range("A:D"));
        append.ValueInputOption =
            SpreadsheetsResource.ValuesResource.AppendRequest.ValueInputOptionEnum.RAW;
        append.InsertDataOption =
            SpreadsheetsResource.ValuesResource.AppendRequest.InsertDataOptionEnum.INSERTROWS;

        await append.ExecuteAsync(cancellationToken);
    }

    public void Dispose() => _sheets.Dispose();

    private string Range(string cells)
    {
        var escapedWorksheet = _worksheet.Replace("'", "''", StringComparison.Ordinal);
        return $"'{escapedWorksheet}'!{cells}";
    }
}
