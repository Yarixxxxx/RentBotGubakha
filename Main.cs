using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;


namespace Namespace
{
    class MainClass : Buttons
    {
        static int start = 0;

        static async Task Main(string[] args)
        {
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

                        // Delete all photo messages if any
                        if (photoMessages.ContainsKey(chatId) && photoMessages[chatId].Count > 0)
                        {
                            foreach (var messageId in photoMessages[chatId])
                            {
                                await client.DeleteMessageAsync(chatId, messageId);
                            }
                            photoMessages[chatId].Clear();
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
                        }
                        else
                        {
                            await HandleAction(callbackQuery.Data, callbackQuery, client, chatId.ToString(), photoMessages);
                            userActions[chatId].Push(callbackQuery.Data);
                        }
                        await client.DeleteMessageAsync(callbackQuery.Message.Chat.Id, callbackQuery.Message.MessageId);

                    }

                    if (message != null && message.Text == "/start" && start == 0)
                    {
                        await SendMainMenu(message, client);
                        start = 1;
                    }
                    offset = update.Id + 1;
                }
                await Task.Delay(700);
            }
        }

        static async Task HandleAction(string action, CallbackQuery callbackQuery, TelegramBotClient client, string chatId, Dictionary<long, List<int>> photoMessages)
        {
            switch (action)
            {
                case "description":
                    var descriptionMessage = await client.SendPhotoAsync(
                        chatId: chatId,
                        photo: InputFile.FromUri("https://github.com/Yarixxxxx/Photos/blob/main/house.jpg?raw=true"));
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
                    break;

                case "schedule":
                    await SendSchedule(callbackQuery, client);
                    break;

                case "telegram":
                    var telegramMessage = await client.SendPhotoAsync(
                        chatId: chatId,
                        photo: InputFile.FromUri("https://github.com/Yarixxxxx/Photos/blob/main/house.jpg?raw=true"));
                    AddToPhotoMessages(photoMessages, callbackQuery.Message.Chat.Id, new[] { telegramMessage.MessageId });
                    await SendTelegram(callbackQuery, client);
                    break;

                case "location":
                    await SendLocation(callbackQuery, client);
                    break;

                case "rent":
                    await SendSchedule(callbackQuery, client);
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
                    await SendHouse2Description(callbackQuery, client);
                    break;

                case "house2_schedule":
                    await SendHouse2Schedule(callbackQuery, client);
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
                    await SendHouse2OnlyVideos(callbackQuery, client);
                    break;

                case "house2_rooms_photos":
                    await SendHouse2Rooms(callbackQuery, client);
                    break;

                case "house2_features_photos":
                    await SendHouse2Features(callbackQuery, client);
                    break;

                case "house2_exterior_photos":
                    await SendHouse2Exterior(callbackQuery, client);
                    break;

                case "house2_telephone":
                    await SendHouse2Telephone(callbackQuery, client);
                    break;

                case "house2_telegram":
                    var house2TelegramMessage = await client.SendPhotoAsync(
                        chatId: chatId,
                        photo: InputFile.FromUri("https://github.com/Yarixxxxx/Photos/blob/main/house.jpg?raw=true"));
                    AddToPhotoMessages(photoMessages, callbackQuery.Message.Chat.Id, new[] { house2TelegramMessage.MessageId });
                    await SendHouse2Telegram(callbackQuery, client);
                    break;

                case "house2_rent":
                    await SendHouse2Rent(callbackQuery, client);
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

                case "apartment_rooms_photos":
                    await SendApartmentRooms(callbackQuery, client);
                    break;

                case "apartment_features_photos":
                    await SendApartmentFeatures(callbackQuery, client);
                    break;

                case "apartment_exterior_photos":
                    await SendApartmentExterior(callbackQuery, client);
                    break;

                case "apartment_telephone":
                    await SendApartmentTelephone(callbackQuery, client);
                    break;

                case "apartment_telegram":
                    var apartmentTelegramMessage = await client.SendPhotoAsync(
                        chatId: chatId,
                        photo: InputFile.FromUri("https://github.com/Yarixxxxx/Photos/blob/main/house.jpg?raw=true"));
                    AddToPhotoMessages(photoMessages, callbackQuery.Message.Chat.Id, new[] { apartmentTelegramMessage.MessageId });
                    await SendApartmentTelegram(callbackQuery, client);
                    break;

                case "apartment_rent":
                    await SendApartmentRent(callbackQuery, client);
                    break;
            }
        }

        static void AddToPhotoMessages(Dictionary<long, List<int>> photoMessages, long chatId, int[] messageIds)
        {
            if (!photoMessages.ContainsKey(chatId))
            {
                photoMessages[chatId] = new List<int>();
            }
            photoMessages[chatId].AddRange(messageIds);
        }

        static async Task SendRooms(CallbackQuery callbackQuery, TelegramBotClient client, Dictionary<long, List<int>> photoMessages)
        {
            string[] photoUrls = new[]
            {
                "https://sun9-7.userapi.com/impg/HWrxyYZsRMXhCBEZPcPYug_sipMsHk1m_ftfng/1HSqaT9ew3M.jpg?size=1620x2160&quality=95&sign=b48f00a13ab3c98f3a4b8bd55fd26989&type=album",
                "https://sun9-37.userapi.com/impg/1UkgIpp_5v0oGTDdjwgE3INl-cMdgjqR6h-gwA/aNQqlp9gk0E.jpg?size=1620x2160&quality=95&sign=d3f26b2ed967ba206c053def0e4e70a4&type=album",
                "https://sun9-36.userapi.com/impg/f-rZT-Ge9IoX1hG9ysKv1juahU3plzT64OhKrg/9fmAPCjTIx0.jpg?size=1620x2160&quality=95&sign=22109c97c0c31ea3a01e2919b2997b54&type=album",
                "https://sun9-8.userapi.com/impg/RZWJ44m9Sue7ifF8cwkePcIbWl2-g8oBlvHNXw/yCtQRSJYmSs.jpg?size=1620x2160&quality=95&sign=5f60b35e5045e46aee446496805ae683&type=album",
                "https://sun9-29.userapi.com/impg/dmEL81ATjmzidn3prUO26_y5fclR-G0K53VpSw/PY1CrM-XVu4.jpg?size=1620x2160&quality=95&sign=de0a66729a7f83a35dc2665d54c79154&type=album",
                "https://sun9-76.userapi.com/impg/hglk0DGil57DahXVYWd7a6Egg-5rEaLLqieM-A/twwG_hJOh2Y.jpg?size=1620x2160&quality=95&sign=9dc05fda902bc711da3059d72ccf586f&type=album",
            "https://sun9-63.userapi.com/impg/Y7ReHV7j27tPRWsXzdg4nK0LuhVs9QrZoTypBw/0wCag1WzJiI.jpg?size=1620x2160&quality=95&sign=ccd95962c3775f430d1c0d8834e87be9&type=album",
            "https://sun9-38.userapi.com/impg/RdLQC2h0czNrplosdUxVsMt7wYJFDpdN2vcDGg/JqNq2VTCdfo.jpg?size=1620x2160&quality=95&sign=0ed762707e802245c332c77fcaf117ce&type=album",
            "https://sun9-32.userapi.com/impg/NplEbSLzs1bIPaLmJTvRoLUatBySuYlugINzZA/K_eilLgWPMQ.jpg?size=1620x2160&quality=95&sign=431a2418e7ded8d0ca879209abab17cf&type=album"};

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
                    InlineKeyboardButton.WithCallbackData("Назад", "go_back")
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
                "https://sun9-1.userapi.com/impg/pVkZH7XsDx90SmKxRWle-9OM1K0g6WlsVbvJMw/yjp_RyrLJWY.jpg?size=1620x2160&quality=95&sign=df0b7450269f18949f4f70f5c118ed29&type=album",
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
                    InlineKeyboardButton.WithCallbackData("Назад", "go_back")
                }
            });

            await client.SendTextMessageAsync(
                chatId: callbackQuery.Message.Chat.Id,
                text: "Фото развлечений",
                replyMarkup: keyboard
            );
        }

        static async Task SendExterior(CallbackQuery callbackQuery, TelegramBotClient client, Dictionary<long, List<int>> photoMessages)
        {
            string[] photoUrls = new[]
            {
                "https://sun9-17.userapi.com/impg/a-romZ_kDpZyHnu7paswHYw7niXnIPpJn3NYkw/9zG7vOxg-Wc.jpg?size=1620x2160&quality=95&sign=b24b780bd2e939f2f12507f076a8f4ef&type=album",
                "https://sun9-66.userapi.com/impg/E6gWP5zmvj4YIk3V22h7wEW4dNYgzbF2GKiOVA/OK_g5M6mB9E.jpg?size=1620x2160&quality=95&sign=8fd820e21070f17f693c3548418f465e&type=album",
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
                    InlineKeyboardButton.WithCallbackData("Назад", "go_back")
                }
            });

            await client.SendTextMessageAsync(
                chatId: callbackQuery.Message.Chat.Id,
                text: "Фото с улицы",
                replyMarkup: keyboard
            );
        }
    }
}
