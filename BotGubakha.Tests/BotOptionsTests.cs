using RentGubakhaBot.Configuration;
using Xunit;

namespace BotGubakha.Tests;

public sealed class BotOptionsTests
{
    [Fact]
    public void Load_Throws_WhenTelegramTokenIsMissing()
    {
        var exception = Assert.Throws<ConfigurationException>(
            () => BotOptions.Load(_ => null));

        Assert.Contains("TELEGRAM_BOT_TOKEN", exception.Message, StringComparison.Ordinal);
    }

    [Theory]
    [InlineData("")]
    [InlineData("  ")]
    [InlineData("replace-me")]
    [InlineData("token-without-colon")]
    public void Load_Throws_WhenTelegramTokenIsInvalid(string token)
    {
        var variables = new Dictionary<string, string?>
        {
            ["TELEGRAM_BOT_TOKEN"] = token
        };

        Assert.Throws<ConfigurationException>(() => BotOptions.Load(variables.GetValueOrDefault));
    }

    [Fact]
    public void Load_DisablesStatistics_WhenSpreadsheetIsMissing()
    {
        var variables = new Dictionary<string, string?>
        {
            ["TELEGRAM_BOT_TOKEN"] = "123456:new-token-value"
        };

        var options = BotOptions.Load(variables.GetValueOrDefault);

        Assert.False(options.IsStatisticsEnabled);
        Assert.Equal("Лист1", options.GoogleSheetsWorksheet);
    }

    [Fact]
    public void Load_TrimsConfiguredValues()
    {
        var variables = new Dictionary<string, string?>
        {
            ["TELEGRAM_BOT_TOKEN"] = " 123456:new-token-value ",
            ["GOOGLE_SHEETS_SPREADSHEET_ID"] = " sheet-id ",
            ["GOOGLE_SHEETS_WORKSHEET"] = " Аренда ",
            ["GOOGLE_CREDENTIALS_JSON"] = " {\"type\":\"service_account\"} "
        };

        var options = BotOptions.Load(variables.GetValueOrDefault);

        Assert.True(options.IsStatisticsEnabled);
        Assert.Equal("123456:new-token-value", options.TelegramBotToken);
        Assert.Equal("sheet-id", options.GoogleSheetsSpreadsheetId);
        Assert.Equal("Аренда", options.GoogleSheetsWorksheet);
        Assert.Equal("{\"type\":\"service_account\"}", options.GoogleCredentialsJson);
    }
}
