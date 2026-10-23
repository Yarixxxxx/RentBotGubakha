using System.Text;
using RentGubakhaBot;
using RentGubakhaBot.Configuration;
using RentGubakhaBot.Services;
using Telegram.Bot;

Console.OutputEncoding = Encoding.UTF8;

using var shutdown = new CancellationTokenSource();
Console.CancelKeyPress += (_, eventArgs) =>
{
    eventArgs.Cancel = true;
    shutdown.Cancel();
};

try
{
    var options = BotOptions.FromEnvironment();
    using var statistics = await GoogleSheetsStatisticsService.CreateAsync(
        options,
        shutdown.Token);
    await statistics.InitializeAsync(shutdown.Token);

    var clientOptions = new TelegramBotClientOptions(options.TelegramBotToken)
    {
        RetryCount = 3,
        RetryThreshold = 60
    };
    var client = new TelegramBotClient(clientOptions, cancellationToken: shutdown.Token);
    var handler = new BotUpdateHandler(client, statistics);
    var application = new BotApplication(client, handler);

    await application.RunAsync(shutdown.Token);
    return 0;
}
catch (ConfigurationException exception)
{
    Console.Error.WriteLine($"Ошибка конфигурации: {exception.Message}");
    return 2;
}
catch (OperationCanceledException) when (shutdown.IsCancellationRequested)
{
    return 0;
}
catch (Exception exception)
{
    Console.Error.WriteLine($"Не удалось запустить бота: {exception}");
    return 1;
}
