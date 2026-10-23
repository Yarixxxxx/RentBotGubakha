using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Types.Enums;

namespace RentGubakhaBot;

internal sealed class BotApplication(
    TelegramBotClient client,
    BotUpdateHandler updateHandler)
{
    private static readonly UpdateType[] AllowedUpdates =
    [
        UpdateType.Message,
        UpdateType.CallbackQuery
    ];

    public async Task RunAsync(CancellationToken cancellationToken)
    {
        var bot = await client.GetMe(cancellationToken);
        Console.WriteLine($"@{bot.Username} запущен. Для остановки нажмите Ctrl+C.");

        int? offset = null;
        while (!cancellationToken.IsCancellationRequested)
        {
            try
            {
                var updates = await client.GetUpdates(
                    offset,
                    timeout: 30,
                    allowedUpdates: AllowedUpdates,
                    cancellationToken: cancellationToken);

                foreach (var update in updates)
                {
                    offset = update.Id + 1;

                    try
                    {
                        await updateHandler.HandleAsync(update, cancellationToken);
                    }
                    catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
                    {
                        return;
                    }
                    catch (Exception exception)
                    {
                        Console.Error.WriteLine(
                            $"Ошибка обработки update {update.Id}: {exception}");
                    }
                }
            }
            catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
            {
                return;
            }
            catch (ApiRequestException exception)
            {
                Console.Error.WriteLine($"Telegram API вернул ошибку: {exception.Message}");
                await Task.Delay(TimeSpan.FromSeconds(2), cancellationToken);
            }
            catch (RequestException exception)
            {
                Console.Error.WriteLine($"Ошибка соединения с Telegram: {exception.Message}");
                await Task.Delay(TimeSpan.FromSeconds(2), cancellationToken);
            }
        }
    }
}
