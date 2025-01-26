using Google.Apis.Sheets.v4.Data;
using Google.Apis.Sheets.v4;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Google.Apis.Util.Store;
using System.Data;

namespace Namespace
{
    class MainClass : Buttons
    {
        static Dictionary<long, int> rentButtonClicks = new Dictionary<long, int>();
        static HashSet<string> uniqueUsers = new HashSet<string>();
        static SheetsService sheetsService;

        static Dictionary<long, List<int>> videoMessageIds = new Dictionary<long, List<int>>();

        static async Task Main(string[] args)
        {
            InitializeGoogleSheets();
            var botToken = "6664930541:AAEzv45U0hCY3U_uuGIJssUe7V1VWKDfAcQ";
            var client = new TelegramBotClient(botToken);
            var offset = 0;
            bool isOK = true;

            Console.WriteLine("Bot started...");

            Dictionary<long, Stack<string>> userActions = new Dictionary<long, Stack<string>>();
            Dictionary<long, List<int>> photoMessages = new Dictionary<long, List<int>>();
            while (isOK)
            {
                var updates = await client.GetUpdatesAsync(offset);
                foreach (var update in updates)
                {
                    Console.WriteLine("Processing update...");

                    var message = update.Message;
                    var callbackQuery = update.CallbackQuery;

                    if (callbackQuery != null)
                    {
                        var chatId = callbackQuery.Message.Chat.Id;
                        var username = "@" + callbackQuery.From.Username;

                        if (photoMessages.ContainsKey(chatId) && photoMessages[chatId].Count > 0)
                        {
                            await DeleteAllPhotoMessages(client, chatId, photoMessages);
                        }

                        if (!userActions.ContainsKey(chatId) && message == null)
                            userActions[chatId] = new Stack<string>();

                        if (callbackQuery.Data == "go_back" && userActions[chatId].Count > 0)
                        {
                            userActions[chatId].Pop();
                            if (userActions[chatId].Count > 0)
                            {
                                string previousAction = userActions[chatId].Peek();
                                await HandleAction(previousAction, callbackQuery, client, chatId.ToString(), photoMessages);
                            }
                            else
                            {
                                await SendMainMenu(callbackQuery.Message, client);
                            }

                            if (videoMessageIds.ContainsKey(chatId))
                            {
                                foreach (var msgId in videoMessageIds[chatId])
                                {
                                    await DeleteMessageSafeAsync(client, chatId, msgId);
                                }
                                videoMessageIds[chatId].Clear();
                            }
                        }
                        else
                        {
                            await HandleAction(callbackQuery.Data, callbackQuery, client, chatId.ToString(), photoMessages);
                            userActions[chatId].Push(callbackQuery.Data);
                        }
                        await DeleteMessageSafeAsync(client, callbackQuery.Message.Chat.Id, callbackQuery.Message.MessageId);
                    }

                    if (message != null && message.Text == "/start")
                    {
                        await SendWelcomeMessage(message, client);
                    }
                    offset = update.Id + 1;
                }
                await Task.Delay(700);
            }
        }

        static async Task SendWelcomeMessage(Message message, TelegramBotClient client)
        {
            var chatId = message.Chat.Id;
            var welcomeText = "⛷️🏠🏂 Добро пожаловать в Губаху! ⛷️🏠🏂 \n\nДля начала выберите дом, расположенный рядом с вами:\n\n1 дом - желтый (Краснооктябрьская, 6)\n2 дом - коричневый (Краснооктябрьская, 12)\n\nНажмите на кнопку для выбора дома";
            var mapImageUrl = "https://sun9-70.userapi.com/impg/doJM9P55crDMijXqswRzmVBZMAwp22-5570ajQ/sDEl6SxsXSE.jpg?size=2094x1657&quality=96&sign=397a5a1593b85a7a4612b8ae8641a832&type=album";
            await client.SendPhotoAsync(
                chatId: chatId,
                photo: InputFile.FromUri(mapImageUrl),
                caption: welcomeText,
                replyMarkup: new InlineKeyboardMarkup(new[]
                {
                    new[]
                    {
                        InlineKeyboardButton.WithCallbackData("Дом 1", "house2"),
                        InlineKeyboardButton.WithCallbackData("Дом 2", "house")
                    }
                })
            );
            await DeleteMessageSafeAsync(client, chatId, message.MessageId);
        }
        

        static void InitializeGoogleSheets()
        {
            string[] Scopes = { SheetsService.Scope.Spreadsheets };
            string ApplicationName = "Telegram Bot Google Sheets Integration";

            UserCredential credential;

            // Читаем содержимое JSON из строки или переменной окружения
            string credentialsJson = Environment.GetEnvironmentVariable("GOOGLE_CREDENTIALS");

            if (string.IsNullOrEmpty(credentialsJson))
            {
                throw new Exception("Google credentials are missing. Make sure GOOGLE_CREDENTIALS is set.");
            }

            using (var stream = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(credentialsJson)))
            {
                string credPath = "token.json"; // Путь для хранения токена авторизации
                credential = GoogleWebAuthorizationBroker.AuthorizeAsync(
                    GoogleClientSecrets.Load(stream).Secrets,
                    Scopes,
                    "user",
                    CancellationToken.None,
                    new FileDataStore(credPath, true)).Result;

                Console.WriteLine("Credential file saved to: " + credPath);
            }

            // Инициализируем SheetsService
            sheetsService = new SheetsService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = ApplicationName,
            });

            InitializeSheetHeaders();
        }


        static async Task InitializeSheetHeaders()
        {
            var spreadsheetId = "1q7CAgj_Gp_wEcFHA_1Fde5jo-IXIZeKPgkjXicdfADU";
            var range = "Лист1!A1:С1";
            var headerValues = new List<IList<object>> {
        new List<object> { "Время", "Идентификатор", "Действие"}
    };

            var valueRange = new ValueRange { Values = headerValues };

            var updateRequest = sheetsService.Spreadsheets.Values.Update(valueRange, spreadsheetId, range);
            updateRequest.ValueInputOption = SpreadsheetsResource.ValuesResource.UpdateRequest.ValueInputOptionEnum.USERENTERED;

            await updateRequest.ExecuteAsync();
            Console.WriteLine("Sheet headers initialized.");
        }

        static void AddToPhotoMessages(Dictionary<long, List<int>> photoMessages, long chatId, int[] messageIds)
        {
            if (!photoMessages.ContainsKey(chatId))
            {
                photoMessages[chatId] = new List<int>();
            }
            photoMessages[chatId].AddRange(messageIds);
        }

        static async Task DeleteAllPhotoMessages(TelegramBotClient client, long chatId, Dictionary<long, List<int>> photoMessages)
        {
            if (photoMessages.ContainsKey(chatId))
            {
                foreach (var messageId in photoMessages[chatId])
                {
                    await DeleteMessageSafeAsync(client, chatId, messageId);
                }
                photoMessages[chatId].Clear();
            }
        }

        static async Task DeleteMessageSafeAsync(TelegramBotClient client, long chatId, int messageId)
        {
            try
            {
                await client.DeleteMessageAsync(chatId, messageId);
            }
            catch (Telegram.Bot.Exceptions.ApiRequestException ex) when (ex.Message.Contains("message to delete not found"))
            {
                Console.WriteLine($"Message {messageId} not found: {ex.Message}");
            }
            catch (Telegram.Bot.Exceptions.ApiRequestException ex) when (ex.Message.Contains("message can't be deleted for everyone"))
            {
                Console.WriteLine($"Message {messageId} can't be deleted for everyone: {ex.Message}");
            }
        }

        static async Task HandleAction(string action, CallbackQuery callbackQuery, TelegramBotClient client, string chatId, Dictionary<long, List<int>> photoMessages)
        {
            switch (action)
            {
                case "description":
                    var descriptionMessage = await client.SendPhotoAsync(
                        chatId: chatId,
                        photo: InputFile.FromUri("https://sun9-34.userapi.com/impg/tVuHuLHumvbmDQf6vCrji1-Xbz7Y8681dAwr2Q/IaqH5Kldkko.jpg?size=1620x2160&quality=95&sign=0f3d2ae876fac12cb208b4c764fd9659&type=album"));
                    AddToPhotoMessages(photoMessages, callbackQuery.Message.Chat.Id, new[] { descriptionMessage.MessageId });
                    await SendDescriptionMenu(callbackQuery, client);
                    break;

                case "faq":
                    await SendFAQ(callbackQuery, client);
                    break;

                case "photos_and_videos":
                    await PhotosAndVideos(callbackQuery, client);
                    break;

                case "only_videos":
                    await SendOnlyVideos(callbackQuery, client);
                    break;

                case "only_photos":
                    await SendOnlyPhotos(callbackQuery, client);
                    break;

                case "rooms_photos":
                    await SendRooms(callbackQuery, client, photoMessages);
                    break;

                case "features_photos":
                    await SendFeatures(callbackQuery, client, photoMessages);
                    break;

                case "exterior_photos":
                    await SendExterior(callbackQuery, client, photoMessages);
                    break;

                case "contact_landlord":
                    await SendLandlord(callbackQuery, client);
                    break;

                case "telephone":
                    await SendTelephone(callbackQuery, client);
                    IncrementButtonClick(callbackQuery.Message.Chat.Id, "telephone", callbackQuery.From.Username);
                    await DeleteMessageSafeAsync(client, callbackQuery.Message.Chat.Id, callbackQuery.Message.MessageId);
                    await UploadStatistics(callbackQuery.Message.Chat.Id, "telephone", callbackQuery.From.Username);
                    break;

                case "schedule":
                    await SendSchedule(callbackQuery, client);
                    IncrementButtonClick(callbackQuery.Message.Chat.Id, "schedule", callbackQuery.From.Username);
                    await DeleteMessageSafeAsync(client, callbackQuery.Message.Chat.Id, callbackQuery.Message.MessageId);
                    await UploadStatistics(callbackQuery.Message.Chat.Id, "schedule", callbackQuery.From.Username);
                    break;

                case "telegram":
                    await SendTelegram(callbackQuery, client);
                    IncrementButtonClick(callbackQuery.Message.Chat.Id, "telegram", callbackQuery.From.Username);
                    await DeleteMessageSafeAsync(client, callbackQuery.Message.Chat.Id, callbackQuery.Message.MessageId);
                    await UploadStatistics(callbackQuery.Message.Chat.Id, "telegram", callbackQuery.From.Username);
                    break;

                case "location":
                    await SendLocation(callbackQuery, client);
                    break;

                case "rent":
                    await SendSchedule(callbackQuery, client);
                    IncrementButtonClick(callbackQuery.Message.Chat.Id, "rent", callbackQuery.From.Username);
                    await DeleteMessageSafeAsync(client, callbackQuery.Message.Chat.Id, callbackQuery.Message.MessageId);
                    await UploadStatistics(callbackQuery.Message.Chat.Id, "rent", callbackQuery.From.Username);
                    break;

                case "other_options":
                    await SendOtherOptions(callbackQuery, client);
                    break;

                case "house":
                    await SendMainMenu(callbackQuery.Message, client);
                    break;

                case "house2":
                    await SendHouse2(callbackQuery, client);
                    break;

                case "house2_description":
                    var descriptionMessage2 = await client.SendPhotoAsync(
                       chatId: chatId,
                       photo: InputFile.FromUri("https://sun9-57.userapi.com/impg/4USiQbZMyAi7y3PY0IgrMsIVAh92O9AE2I0sNA/6Hq-jZmj0nU.jpg?size=1620x2160&quality=95&sign=9c17c508574b4be7d232af1cfafdce04&type=album"));
                    AddToPhotoMessages(photoMessages, callbackQuery.Message.Chat.Id, new[] { descriptionMessage2.MessageId });
                    await SendHouse2Description(callbackQuery, client);
                    break;

                case "house2_schedule":
                    await SendHouse2Schedule(callbackQuery, client);
                    IncrementButtonClick(callbackQuery.Message.Chat.Id, "house2_schedule", callbackQuery.From.Username);
                    await DeleteMessageSafeAsync(client, callbackQuery.Message.Chat.Id, callbackQuery.Message.MessageId);
                    await UploadStatistics(callbackQuery.Message.Chat.Id, "house2_schedule", callbackQuery.From.Username);
                    break;

                case "house2_photos_and_videos":
                    await SendHouse2PhotosAndVideos(callbackQuery, client);
                    break;

                case "house2_location":
                    await SendHouse2Location(callbackQuery, client);
                    break;

                case "house2_faq":
                    await SendHouse2FAQ(callbackQuery, client);
                    break;

                case "house2_contact_landlord":
                    await SendHouse2Landlord(callbackQuery, client);
                    break;

                case "house2_other_options":
                    await SendHouse2OtherOptions(callbackQuery, client);
                    break;

                case "house2_only_photos":
                    await SendHouse2OnlyPhotos(callbackQuery, client);
                    break;

                case "house2_only_videos":
                    await SendOnly2Videos(callbackQuery, client);
                    break;

                case "house2_rooms_photos":
                    await SendHouse2Rooms(callbackQuery, client, photoMessages);
                    break;

                case "house2_features_photos":
                    await SendHouse2Features(callbackQuery, client, photoMessages);
                    break;

                case "house2_exterior_photos":
                    await SendHouse2Exterior(callbackQuery, client, photoMessages);
                    break;

                case "house2_telephone":
                    await SendHouse2Telephone(callbackQuery, client);
                    IncrementButtonClick(callbackQuery.Message.Chat.Id, "house2_telephone", callbackQuery.From.Username);
                    await DeleteMessageSafeAsync(client, callbackQuery.Message.Chat.Id, callbackQuery.Message.MessageId);
                    await UploadStatistics(callbackQuery.Message.Chat.Id, "house2_telephone", callbackQuery.From.Username);
                    break;

                case "house2_telegram":
                    await SendHouse2Telegram(callbackQuery, client);
                    IncrementButtonClick(callbackQuery.Message.Chat.Id, "house2_telegram", callbackQuery.From.Username);
                    await DeleteMessageSafeAsync(client, callbackQuery.Message.Chat.Id, callbackQuery.Message.MessageId);
                    await UploadStatistics(callbackQuery.Message.Chat.Id, "house2_telegram", callbackQuery.From.Username);
                    break;

                case "house2_rent":
                    await SendHouse2Rent(callbackQuery, client);
                    IncrementButtonClick(callbackQuery.Message.Chat.Id, "house2_rent", callbackQuery.From.Username);
                    await DeleteMessageSafeAsync(client, callbackQuery.Message.Chat.Id, callbackQuery.Message.MessageId);
                    await UploadStatistics(callbackQuery.Message.Chat.Id, "house2_rent", callbackQuery.From.Username);
                    break;

                case "apartment":
                    await SendApartment(callbackQuery, client);
                    break;

                case "apartment_description":
                    await SendApartmentDescription(callbackQuery, client);
                    break;

                case "apartment_schedule":
                    await SendApartmentSchedule(callbackQuery, client);
                    break;

                case "apartment_photos_and_videos":
                    await SendApartmentPhotosAndVideos(callbackQuery, client);
                    break;

                case "apartment_location":
                    await SendApartmentLocation(callbackQuery, client);
                    break;

                case "apartment_faq":
                    await SendApartmentFAQ(callbackQuery, client);
                    break;

                case "apartment_contact_landlord":
                    await SendApartmentLandlord(callbackQuery, client);
                    break;

                case "apartment_other_options":
                    await SendApartmentOtherOptions(callbackQuery, client);
                    break;

                case "apartment_only_photos":
                    await SendApartmentOnlyPhotos(callbackQuery, client);
                    break;

                case "apartment_only_videos":
                    await SendApartmentOnlyVideos(callbackQuery, client);
                    break;

                case "apartment_telephone":
                    await SendApartmentTelephone(callbackQuery, client);
                    break;

                case "apartment_telegram":
                    await SendApartmentTelegram(callbackQuery, client);
                    break;

                case "apartment_rent":
                    await SendApartmentRent(callbackQuery, client);
                    break;

                case "help":
                    await SendHelp(callbackQuery, client);
                    break;

                case "house2_help":
                    await SendHelp(callbackQuery, client);
                    break;
            }
        }

        static async Task SendRooms(CallbackQuery callbackQuery, TelegramBotClient client, Dictionary<long, List<int>> photoMessages)
        {
            string[] photoUrls = new[]
            {
                "https://sun9-15.userapi.com/impg/nHCVFAyz7Ohm2ZEVZwk9Y-i6uFfBap6lPs-MVg/e3wC_1-vqU0.jpg?size=1620x2160&quality=95&sign=b246aafd8d6055523cadf6e846a3fe16&type=album",
                "https://sun9-79.userapi.com/impg/LCtiarO_XUr-fipY-k0FfTCLwKYpN0wLiiEcKg/TQyEeHxbdx4.jpg?size=1620x2160&quality=95&sign=1556ac5c559832ff3783c28830c9c4f7&type=album",
                "https://sun9-41.userapi.com/impg/R0rrOxY2LDWWABpbbFNpvHzlabxxoz7djD_aPw/xpXIHz5gJyY.jpg?size=1620x2160&quality=95&sign=92949eb9207d3c165ecc7a07784b9f1b&type=album",
                "https://sun9-63.userapi.com/impg/QOpsLnT9HRw_Wmd7QOsxCO_4y6bYmO3Q6DoK9g/8mTwkpkmNg0.jpg?size=1620x2160&quality=95&sign=30b591c02e35a8c7e4d33ddaa9d82be1&type=album",
                "https://sun9-49.userapi.com/impg/iRYbfsM6qzCitPRQkXrqPtU46JTnfDCcK0X_bA/8QmhM3yZFUo.jpg?size=1620x2160&quality=95&sign=e50eec5edd80a84504679e1f3623fd30&type=album",
                "https://sun9-23.userapi.com/impg/nI1SnLt0b_7uJNCZwG0fmBISGDET8QmqF4fyRw/DHe-2a7MlCo.jpg?size=1620x2160&quality=95&sign=dc2a58f1330f6cd9b524f04b7b06dac5&type=album",
                "https://sun9-41.userapi.com/impg/M_VE7yNNxGpccCRNLtbL3z5pEHbHf0JJJjfuIg/l1-eJyniIts.jpg?size=1620x2160&quality=95&sign=18d7c959342f8c966200b1a96bf0cdb8&type=album",
                "https://sun9-79.userapi.com/impg/r_6-3JTEa3vX-ABNB_BgpU8q1xsdVBf_BnMW7A/cMoHXC8Y85E.jpg?size=1620x2160&quality=95&sign=3ac95a13d255d1752d8d20fe79a58502&type=album",
                "https://sun9-22.userapi.com/impg/ukIC60d2hZd7mwUVCoYiXmZBDFd9KbeWnRUG2Q/-SQqHNDc77w.jpg?size=1620x2160&quality=95&sign=9ad48b5aec5df4e1cf255ad3ae9064b9&type=album",
                "https://sun9-7.userapi.com/impg/IokIxOfDo-B1zKuBOw-GIVGGXKZwoELImsR1-g/1AI5JcxjvvM.jpg?size=1620x2160&quality=95&sign=3ee566417dbcb1e6a68cce44cc2191d2&type=album",
            };

            string[] photoUrls2 = new[]
            {
                "https://sun9-2.userapi.com/impg/LcD9LsbxAghEzANz6p2FmWS_WhTlb0-rcNYg_Q/lsiripL6yDY.jpg?size=1620x2160&quality=95&sign=faf4235dd8309ee2c1f2bb02e0ed592c&type=album",
                "https://sun9-45.userapi.com/impg/hsfTNKutT-VVYadIFWt5yZzTZRCIMAD_BKEzFw/fTN_ydi5xHk.jpg?size=1620x2160&quality=95&sign=bcb64578404bbea62a1288057563c22f&type=album",
                "https://sun9-2.userapi.com/impg/mpz7CyE4R2lwzpnk0v-XORAXCGIZMBZEv8ax7Q/URkk_2fp3uA.jpg?size=1620x2160&quality=95&sign=5533ddf69c8c6d2cbc43399e1294e151&type=album",
                "https://sun9-40.userapi.com/impg/i_S05CiJnoN5u3SC45sE1VJWvpz8AbiS28bM2g/WteSwJs6mzs.jpg?size=1620x2160&quality=95&sign=11cbafc143b8b11a370a5e9fb5eac1fa&type=album",
                "https://sun9-65.userapi.com/impg/6Kkj7OBWwyXByy-wH9ubk_Rr0aAqFPVwKVnBkg/_OSytMpJ974.jpg?size=1620x2160&quality=95&sign=d498d7b09bf269d5d5ca0622ee82bea3&type=album",
                "https://sun9-70.userapi.com/impg/eLZ69_3Q1FsvpUpI2LXuysDQx3fvu5wmKHHo2Q/teoKSvyDKH4.jpg?size=1620x2160&quality=95&sign=a43649f5c7700eca740a45991b395696&type=album",
                "https://sun9-76.userapi.com/impg/1-wQ1by8UUmgQ56bETTMfoY9-lWQdKR5RtLqxw/SWLNi75i1I8.jpg?size=1620x2160&quality=95&sign=dd5253bef4a000e001f196ab95d6b315&type=album",
            };

            var mediaGroup = photoUrls.Select(url => new InputMediaPhoto(InputFile.FromUri(url))).ToList<IAlbumInputMedia>();

            var sentMessages = await client.SendMediaGroupAsync(
                chatId: callbackQuery.Message.Chat.Id,
                media: mediaGroup
            );

            var mediaGroup2 = photoUrls2.Select(url => new InputMediaPhoto(InputFile.FromUri(url))).ToList<IAlbumInputMedia>();

            var sentMessages2 = await client.SendMediaGroupAsync(
                chatId: callbackQuery.Message.Chat.Id,
                media: mediaGroup2
            );

            AddToPhotoMessages(photoMessages, callbackQuery.Message.Chat.Id, sentMessages.Select(msg => msg.MessageId).ToArray());
            AddToPhotoMessages(photoMessages, callbackQuery.Message.Chat.Id, sentMessages2.Select(msg => msg.MessageId).ToArray());

            var keyboard = new InlineKeyboardMarkup(new[]
            {
                new[]
                {
                    InlineKeyboardButton.WithCallbackData("Назад ⏪", "go_back")
                }
            });

            await client.SendTextMessageAsync(
                chatId: callbackQuery.Message.Chat.Id,
                text: "Фото комнат",
                replyMarkup: keyboard
            );
        }

        static async Task SendFeatures(CallbackQuery callbackQuery, TelegramBotClient client, Dictionary<long, List<int>> photoMessages)
        {
            string[] photoUrls = new[]
            {
                "https://sun9-65.userapi.com/impg/0AuAHSofgHV__Koaz9EoV8Bk0WbW4J1U_xIvLw/Ri_By7Z23AA.jpg?size=1620x2160&quality=95&sign=2225ca18b57bf761ef60d7366db4b825&type=album",
            };

            var mediaGroup = photoUrls.Select(url => new InputMediaPhoto(InputFile.FromUri(url))).ToList<IAlbumInputMedia>();

            var sentMessages = await client.SendMediaGroupAsync(
                chatId: callbackQuery.Message.Chat.Id,
                media: mediaGroup
            );

            AddToPhotoMessages(photoMessages, callbackQuery.Message.Chat.Id, sentMessages.Select(msg => msg.MessageId).ToArray());

            var keyboard = new InlineKeyboardMarkup(new[]
            {
                new[]
                {
                    InlineKeyboardButton.WithCallbackData("Назад ⏪", "go_back")
                }
            });

            await client.SendTextMessageAsync(
                chatId: callbackQuery.Message.Chat.Id,
                text: "Фото удобств",
                replyMarkup: keyboard
            );
        }

        static async Task SendExterior(CallbackQuery callbackQuery, TelegramBotClient client, Dictionary<long, List<int>> photoMessages)
        {
            string[] photoUrls = new[]
            {
                "https://sun9-64.userapi.com/impg/tSj9xCkXN1eyIlXxAphUBjU0klT28KiXj6Qnog/YSh2l7-P1fY.jpg?size=2560x1920&quality=95&sign=dbf49b425e134762f110ce3e748a32f3&type=album",
                "https://sun9-55.userapi.com/impg/KEJ7vHUTOyLU4Gh01hgpH7fbAm6Mm0zo9r5AHQ/tk4_Xzc5PdQ.jpg?size=2560x1920&quality=95&sign=1998bb05e748cd61e068f0b576ef78b6&type=album",
                "https://sun9-42.userapi.com/impg/NVv0c26XsDiuzhpK-tZbTn8OYEaXiGyRhmKnFA/xLT1qRE3jW0.jpg?size=1620x2160&quality=95&sign=e2239267a1759488b84bf1f5f4a3c785&type=album",
                "https://sun9-33.userapi.com/impg/wgA9a4Ux0AbHu1bKSeP3fv4oZkroHDlfqWBY8w/XY5Jv_UtGSM.jpg?size=1620x2160&quality=95&sign=1ec48f5e34ae8fdedf5b6de9a4d758c2&type=album",
                "https://sun9-75.userapi.com/impg/fo3JaZ0RIp0jsseLgHW-rmtDmE1KDjGQrw_W2g/R-gDhyaF1cw.jpg?size=1620x2160&quality=95&sign=06eac938c231faffa7ce604dc8076d10&type=album",
            };

            var mediaGroup = photoUrls.Select(url => new InputMediaPhoto(InputFile.FromUri(url))).ToList<IAlbumInputMedia>();

            var sentMessages = await client.SendMediaGroupAsync(
                chatId: callbackQuery.Message.Chat.Id,
                media: mediaGroup
            );

            AddToPhotoMessages(photoMessages, callbackQuery.Message.Chat.Id, sentMessages.Select(msg => msg.MessageId).ToArray());

            var keyboard = new InlineKeyboardMarkup(new[]
            {
                new[]
                {
                    InlineKeyboardButton.WithCallbackData("Назад ⏪", "go_back")
                }
            });

            await client.SendTextMessageAsync(
                chatId: callbackQuery.Message.Chat.Id,
                text: "Фото с улицы",
                replyMarkup: keyboard
            );
        }

        public static async Task SendOnlyVideos(CallbackQuery callbackQuery, TelegramBotClient client)
        {
            var keyboard = new InlineKeyboardMarkup(new[]
            {
                new[]
                {
                    InlineKeyboardButton.WithCallbackData("Назад ⏪", "go_back")
                }
            });

            var message1 = await client.SendTextMessageAsync(
                chatId: callbackQuery.Message.Chat.Id,
                text: "https://t.me/GubakhaVideo/4"
            );

            var message2 = await client.SendTextMessageAsync(
                chatId: callbackQuery.Message.Chat.Id,
                text: "https://t.me/GubakhaVideo/3",
                replyMarkup: keyboard
            );

            if (!videoMessageIds.ContainsKey(callbackQuery.Message.Chat.Id))
            {
                videoMessageIds[callbackQuery.Message.Chat.Id] = new List<int>();
            }

            videoMessageIds[callbackQuery.Message.Chat.Id].Add(message1.MessageId);
            videoMessageIds[callbackQuery.Message.Chat.Id].Add(message2.MessageId);
        }

        public static async Task SendOnly2Videos(CallbackQuery callbackQuery, TelegramBotClient client)
        {
            var keyboard = new InlineKeyboardMarkup(new[]
            {
                new[]
                {
                    InlineKeyboardButton.WithCallbackData("Назад ⏪", "go_back")
                }
            });

            var message1 = await client.SendTextMessageAsync(
                chatId: callbackQuery.Message.Chat.Id,
                text: "https://t.me/GubakhaVideo/5"
            );

            var message2 = await client.SendTextMessageAsync(
                chatId: callbackQuery.Message.Chat.Id,
                text: "https://t.me/GubakhaVideo/6",
                replyMarkup: keyboard
            );

            if (!videoMessageIds.ContainsKey(callbackQuery.Message.Chat.Id))
            {
                videoMessageIds[callbackQuery.Message.Chat.Id] = new List<int>();
            }

            videoMessageIds[callbackQuery.Message.Chat.Id].Add(message1.MessageId);
            videoMessageIds[callbackQuery.Message.Chat.Id].Add(message2.MessageId);
        }

        static async Task SendHouse2Rooms(CallbackQuery callbackQuery, TelegramBotClient client, Dictionary<long, List<int>> photoMessages)
        {
            string[] photoUrls = new[]
            {
                "https://sun9-24.userapi.com/impg/xeRwc_Gn-7nSwcXno6W8DCoUZVaA4q1VDNhaug/RYEmWz9qKiU.jpg?size=2560x1920&quality=95&sign=6b9640ebf5f85e038f14d8fe39d6f84a&type=album",
                "https://sun9-63.userapi.com/impg/Y5iUH_UvvZbWSMzRJGmJyDLSvowQNbCEQ6Zf8g/4mBSEDNwiyw.jpg?size=2560x1920&quality=95&sign=adbd78a9221509a5eb3ca9b8bb9afd29&type=album",
                "https://sun9-52.userapi.com/impg/g6o8NTRc_Smf_SkKMwjW39eH-UZwh9vdr7thKg/VggiN64sbm0.jpg?size=1620x2160&quality=95&sign=1d05cd71a503b2cc06f6f4d8cd850d65&type=album",
                "https://sun9-79.userapi.com/impg/GA0jKLz0trBwpBayGbrimP7oSDCXJS5MQuHeKQ/-tWr1oNzgRw.jpg?size=2560x1920&quality=95&sign=fbad44ec9c3cf701d0b26d5b4ccf3995&type=album",
                "https://sun9-18.userapi.com/impg/PdWUay4kYo5_SIHoYAQpAfJXpsP7pkNURedDxQ/TSFvyJ5WPl4.jpg?size=1620x2160&quality=95&sign=01114d102eaa5d4273d249a0cbabf034&type=album",
                "https://sun9-3.userapi.com/impg/v4cgt8Ewts5_MWW2HdRahr8bt5M1g-kxBLuzkA/8_QM-mlVCnY.jpg?size=1620x2160&quality=95&sign=362e42532ef96d60e5416206cb0c3ca1&type=album",
                "https://sun9-68.userapi.com/impg/bN5Qfb_XQaec0x3R9aFw5HkaHoV6bnIH3Fy9kw/pgZSyidojBU.jpg?size=1620x2160&quality=95&sign=c6cbbed69644a2285e9987769b5bbaa7&type=album",
                "https://sun9-80.userapi.com/impg/WnwfT-SISRkKJtuC_TUaSVT_SnJfI_qU3S8DFw/qRH8w7BPKnw.jpg?size=1620x2160&quality=95&sign=d967ed17d6e74c1a2d622f5921f4d32f&type=album",
                "https://sun9-23.userapi.com/impg/Qh9fgrCWPHTNJTQ3RoY6irAGRg6XPiLExFUVXw/INHl3saKL08.jpg?size=1620x2160&quality=95&sign=f9cdace87e3f406438d9d608c5e52dca&type=album",
                "https://sun9-78.userapi.com/impg/Gcf5jRZgPRebx5shArsNrcYQoT-f9ldWnPZjXQ/p5BraJqro2U.jpg?size=2560x1920&quality=95&sign=7f56d2c70b9ba193ecfbe01e3422a58d&type=album",
            };


            var mediaGroup = photoUrls.Select(url => new InputMediaPhoto(InputFile.FromUri(url))).ToList<IAlbumInputMedia>();

            var sentMessages = await client.SendMediaGroupAsync(
                chatId: callbackQuery.Message.Chat.Id,
                media: mediaGroup
            );

            AddToPhotoMessages(photoMessages, callbackQuery.Message.Chat.Id, sentMessages.Select(msg => msg.MessageId).ToArray());

            var keyboard = new InlineKeyboardMarkup(new[]
            {
                new[]
                {
                    InlineKeyboardButton.WithCallbackData("Назад ⏪", "go_back")
                }
            });

            await client.SendTextMessageAsync(
                chatId: callbackQuery.Message.Chat.Id,
                text: "Фото комнат",
                replyMarkup: keyboard
            );
        }

        static async Task SendHouse2Features(CallbackQuery callbackQuery, TelegramBotClient client, Dictionary<long, List<int>> photoMessages)
        {
            string[] photoUrls = new[]
            {
                "https://sun9-68.userapi.com/impg/7QT89P9-GgIo-TKyCfwmry94eS1NDOrNSj056w/Q2dE1Gfk-CY.jpg?size=1620x2160&quality=95&sign=b0a382787d5dc80d0712fd06d8aa833e&type=album",
            };

            var mediaGroup = photoUrls.Select(url => new InputMediaPhoto(InputFile.FromUri(url))).ToList<IAlbumInputMedia>();

            var sentMessages = await client.SendMediaGroupAsync(
                chatId: callbackQuery.Message.Chat.Id,
                media: mediaGroup
            );

            AddToPhotoMessages(photoMessages, callbackQuery.Message.Chat.Id, sentMessages.Select(msg => msg.MessageId).ToArray());

            var keyboard = new InlineKeyboardMarkup(new[]
            {
                new[]
                {
                    InlineKeyboardButton.WithCallbackData("Назад ⏪", "go_back")
                }
            });

            await client.SendTextMessageAsync(
                chatId: callbackQuery.Message.Chat.Id,
                text: "Фото удобств",
                replyMarkup: keyboard
            );
        }

        static async Task SendHouse2Exterior(CallbackQuery callbackQuery, TelegramBotClient client, Dictionary<long, List<int>> photoMessages)
        {
            string[] photoUrls = new[]
            {
                "https://sun9-12.userapi.com/impg/ZnooW0u89W13lBWv2Bm2ACLHHShsHlBrl-NwVw/SQUrSmDZcXI.jpg?size=1620x2160&quality=95&sign=d780f64724b30e10151110f377a7efd9&type=album",
                "https://sun9-57.userapi.com/impg/4USiQbZMyAi7y3PY0IgrMsIVAh92O9AE2I0sNA/6Hq-jZmj0nU.jpg?size=1620x2160&quality=95&sign=9c17c508574b4be7d232af1cfafdce04&type=album",
                "https://sun9-80.userapi.com/impg/0A_Eg80F5VmHZjDlt1GowDDvLWC_XDbQKz8oOw/UnPY7u1BdpE.jpg?size=1620x2160&quality=95&sign=ff938b2ff919688b629da161529f1ad1&type=album",
                "https://sun9-32.userapi.com/impg/ybmkuexo8YehtJIzVr_7eWm4YNt8wLSWxklBww/70DBqOjCoU8.jpg?size=1820x998&quality=96&sign=c9cf77b0c3fa5cca3194fbf45de75292&type=album"
            };

            var mediaGroup = photoUrls.Select(url => new InputMediaPhoto(InputFile.FromUri(url))).ToList<IAlbumInputMedia>();

            var sentMessages = await client.SendMediaGroupAsync(
                chatId: callbackQuery.Message.Chat.Id,
                media: mediaGroup
            );

            AddToPhotoMessages(photoMessages, callbackQuery.Message.Chat.Id, sentMessages.Select(msg => msg.MessageId).ToArray());

            var keyboard = new InlineKeyboardMarkup(new[]
            {
                new[]
                {
                    InlineKeyboardButton.WithCallbackData("Назад ⏪", "go_back")
                }
            });

            await client.SendTextMessageAsync(
                chatId: callbackQuery.Message.Chat.Id,
                text: "Фото с улицы",
                replyMarkup: keyboard
            );
        }

        static void IncrementButtonClick(long chatId, string action, string username)
        {
            if (!rentButtonClicks.ContainsKey(chatId))
            {
                rentButtonClicks[chatId] = 0;
            }
            rentButtonClicks[chatId]++;
            uniqueUsers.Add("@" + username);
        }

        static async Task UploadStatistics(long chatId, string action, string username)
        {
            var spreadsheetId = "1q7CAgj_Gp_wEcFHA_1Fde5jo-IXIZeKPgkjXicdfADU";
            var range = "Лист1!A:D";

            var requestBody = new ValueRange
            {
                Values = new List<IList<object>>
                {
                    new List<object> { DateTime.Now.ToString("s"), "@" + username, action }
                }
            };

            var appendRequest = sheetsService.Spreadsheets.Values.Append(requestBody, spreadsheetId, range);
            appendRequest.ValueInputOption = SpreadsheetsResource.ValuesResource.AppendRequest.ValueInputOptionEnum.USERENTERED;

            var appendResponse = await appendRequest.ExecuteAsync();
            Console.WriteLine($"Statistics uploaded successfully for username @{username} and action {action}.");

            await UpdateUserCount();
        }

        static async Task UpdateUserCount()
        {
            var spreadsheetId = "1q7CAgj_Gp_wEcFHA_1Fde5jo-IXIZeKPgkjXicdfADU";
            var userCountRange = "Лист1!E1:E2";

            var userCountBody = new ValueRange
            {
                Values = new List<IList<object>>
                {
                    new List<object> { "Кол-во пользователей" },
                    new List<object> { uniqueUsers.Count }
                }
            };

            var updateRequest = sheetsService.Spreadsheets.Values.Update(userCountBody, spreadsheetId, userCountRange);
            updateRequest.ValueInputOption = SpreadsheetsResource.ValuesResource.UpdateRequest.ValueInputOptionEnum.USERENTERED;

            await updateRequest.ExecuteAsync();
            Console.WriteLine("User count updated in F1 and F2.");
        }
    }
}
