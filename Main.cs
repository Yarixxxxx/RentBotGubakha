using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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
                        start = 1;
                    }
                    offset = update.Id + 1;
                }
                await Task.Delay(700);
            }
        }

        static async Task SendWelcomeMessage(Message message, TelegramBotClient client)
        {
            var chatId = message.Chat.Id;
            var welcomeText = "Добро пожаловать на курорт Губаха! Выберите дом, с которым находитесь рядом...\n\n1 дом - Краснооктябрьская, 12\n2 дом - Краснооктябрьская, 6\n\nДома отмечены на карте ниже";
            var mapImageUrl = "https://sun9-22.userapi.com/impg/Janx-X0domWG_sKW4lQWO79EwTgCO7cDTvLpuA/l5f_FxS-yIE.jpg?size=796x575&quality=96&sign=2f3663ce856a900b1677bae15629282b&type=album"; 
            await client.SendPhotoAsync(
                chatId: chatId,
                photo: InputFile.FromUri(mapImageUrl),
                caption: welcomeText,
                replyMarkup: new InlineKeyboardMarkup(new[]
                {
                    new[]
                    {
                        InlineKeyboardButton.WithCallbackData("Дом 1", "description"),
                        InlineKeyboardButton.WithCallbackData("Дом 2", "house2_description")
                    }
                })
            );
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
                "https://sun9-32.userapi.com/impg/NplEbSLzs1bIPaLmJTvRoLUatBySuYlugINzZA/K_eilLgWPMQ.jpg?size=1620x2160&quality=95&sign=431a2418e7ded8d0ca879209abab17cf&type=album",
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
                text: "Фото комнат",
                replyMarkup: keyboard
            );
        }

        static async Task HandleAction(string action, CallbackQuery callbackQuery, TelegramBotClient client, string chatId, Dictionary<long, List<int>> photoMessages)
        {
            switch (action)
            {
                case "description":
                    var descriptionMessage = await client.SendPhotoAsync(
                        chatId: chatId,
                        photo: InputFile.FromUri("https://sun9-66.userapi.com/impg/E6gWP5zmvj4YIk3V22h7wEW4dNYgzbF2GKiOVA/OK_g5M6mB9E.jpg?size=1620x2160&quality=95&sign=8fd820e21070f17f693c3548418f465e&type=album"));
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
                    var descriptionMessage2 = await client.SendPhotoAsync(
                       chatId: chatId,
                       photo: InputFile.FromUri("https://sun9-57.userapi.com/impg/4USiQbZMyAi7y3PY0IgrMsIVAh92O9AE2I0sNA/6Hq-jZmj0nU.jpg?size=1620x2160&quality=95&sign=9c17c508574b4be7d232af1cfafdce04&type=album"));
                    AddToPhotoMessages(photoMessages, callbackQuery.Message.Chat.Id, new[] { descriptionMessage2.MessageId });
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
                    break;

                case "house2_telegram":
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
                    await SendApartmentTelegram(callbackQuery, client);
                    break;

                case "apartment_rent":
                    await SendApartmentRent(callbackQuery, client);
                    break;
            }
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

        static async Task SendHouse2Rooms(CallbackQuery callbackQuery, TelegramBotClient client, Dictionary<long, List<int>> photoMessages)
        {
            string[] photoUrls = new[]
            {
                "https://sun9-61.userapi.com/impg/RbTV_Obt0mAN7HdV2j3NTtdChI5KSb7jSU-jVw/PE72BddtILg.jpg?size=1620x2160&quality=95&sign=cb412488ddabd158a57eef0d2c2aa152&type=album",
                "https://sun9-27.userapi.com/impg/QFtX7vMSVp7JItyoEu5IufZ_RlB_TEmk1Bsp4w/QyVbBV5th2o.jpg?size=1620x2160&quality=95&sign=8415bb53dd8408995fcba6927f1895d1&type=album",
                "https://sun9-68.userapi.com/impg/oxSw_K8T1MHtfm1Pllkb_J7X7ortypYUsO_WVw/gQrzD1BVVfA.jpg?size=1620x2160&quality=95&sign=87dc2d81acbf105b9ca73df074a419b7&type=album",
                "https://sun9-36.userapi.com/impg/0dna961s9I15WvVukB7to7AZVzeD57twiVXFfw/4utHRLRogWc.jpg?size=1620x2160&quality=95&sign=bbf5f87f356449a74dca19c591d8945b&type=album",
                "https://sun9-27.userapi.com/impg/5F5ykUjos5SXRIhZ_vIMAR18RnVCNAS-ACzFMA/l0ZFWEP6Q5I.jpg?size=1620x2160&quality=95&sign=8431af46df3d6f74fa9befcee0ec1244&type=album",
                "https://sun9-36.userapi.com/impg/YFLOwvasKU8g3GI6DXYHtwyFac6r2S1OD_v0hQ/c7yqpnyA6yM.jpg?size=1620x2160&quality=95&sign=088ac6af2df800c9f7fdee9fe8d2ba21&type=album",
                "https://sun9-30.userapi.com/impg/4J66aMCbo0_FyN0GBHAoWjG_yDuHFm9mNNDhzA/u3BbggLKtyk.jpg?size=1620x2160&quality=95&sign=8c4ae8fe5df8ff7ad9747a90133b9602&type=album",
                "https://sun9-15.userapi.com/impg/GW51mmZpFE3DQJx_7qUw4x2shx1lzpMFVuYpGw/Dr53X4oDPxg.jpg?size=1620x2160&quality=95&sign=e4a47aae94003aef28d66c1a64a80c16&type=album",
                "https://sun9-39.userapi.com/impg/J_nfloHymUaUnphpNoJOVNr_LPX3iTs0Jh66dQ/wjrKOr6Gu4Q.jpg?size=2560x1920&quality=95&sign=c31cc5ba8cc0e3f95f6d3c773337fa12&type=album",
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
                    InlineKeyboardButton.WithCallbackData("Назад", "go_back")
                }
            });

            await client.SendTextMessageAsync(
                chatId: callbackQuery.Message.Chat.Id,
                text: "Фото развлечений",
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
