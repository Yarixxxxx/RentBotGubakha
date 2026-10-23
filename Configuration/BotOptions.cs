namespace RentGubakhaBot.Configuration;

internal sealed record BotOptions(
    string TelegramBotToken,
    string? GoogleSheetsSpreadsheetId,
    string GoogleSheetsWorksheet,
    string? GoogleCredentialsJson)
{
    private const string DefaultWorksheet = "Лист1";

    public bool IsStatisticsEnabled => !string.IsNullOrWhiteSpace(GoogleSheetsSpreadsheetId);

    public static BotOptions FromEnvironment() => Load(Environment.GetEnvironmentVariable);

    internal static BotOptions Load(Func<string, string?> readVariable)
    {
        var token = readVariable("TELEGRAM_BOT_TOKEN")?.Trim();
        if (string.IsNullOrWhiteSpace(token) ||
            token.Equals("replace-me", StringComparison.OrdinalIgnoreCase) ||
            !token.Contains(':', StringComparison.Ordinal))
        {
            throw new ConfigurationException(
                "Переменная TELEGRAM_BOT_TOKEN не задана или имеет неверный формат.");
        }

        var spreadsheetId = NullIfWhiteSpace(readVariable("GOOGLE_SHEETS_SPREADSHEET_ID"));
        var worksheet = NullIfWhiteSpace(readVariable("GOOGLE_SHEETS_WORKSHEET")) ?? DefaultWorksheet;
        var credentialsJson = NullIfWhiteSpace(readVariable("GOOGLE_CREDENTIALS_JSON"));

        return new BotOptions(token, spreadsheetId, worksheet, credentialsJson);
    }

    private static string? NullIfWhiteSpace(string? value) =>
        string.IsNullOrWhiteSpace(value) ? null : value.Trim();
}

internal sealed class ConfigurationException(string message) : Exception(message);
