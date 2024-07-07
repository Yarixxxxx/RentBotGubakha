using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using System.Collections.Generic;
using System;

namespace Namespace
{
    class MainClass : Buttons
    {
        static int photo = 0;
        static int start = 0;

        static async Task Main(string[] args)
        {
            var botToken = "6664930541:AAEzv45U0hCY3U_uuGIJssUe7V1VWKDfAcQ";
            var client = new TelegramBotClient(botToken);
            var offset = 0;
            bool isOK = true;

            Console.WriteLine("Bot started...");

            Dictionary<long, Stack<string>> userActions = new Dictionary<long, Stack<string>>();

            while (isOK)
            {
                var updates = await client.GetUpdatesAsync(offset);
                foreach (var update in updates)
                {
                    Console.WriteLine("Processing update...");

                    var message = update.Message;
                    var callbackQuery = update.CallbackQuery;

                    if (photo == 1)
                    {
                        await client.DeleteMessageAsync(callbackQuery.Message.Chat.Id, callbackQuery.Message.MessageId - 1);
                        photo = 0;
                    }

                    if (callbackQuery != null)
                    {
                        var chatId = callbackQuery.Message.Chat.Id;

                        if (!userActions.ContainsKey(chatId) && message == null)
                            userActions[chatId] = new Stack<string>();

                        if (callbackQuery.Data == "go_back" && userActions[chatId].Count > 0)
                        {
                            userActions[chatId].Pop();
                            if (userActions[chatId].Count > 0)
                            {
                                string previousAction = userActions[chatId].Peek();
                                await HandleAction(previousAction, callbackQuery, client, chatId.ToString());
                            }
                            else
                            {
                                await SendMainMenu(callbackQuery.Message, client);
                            }
                        }
                        else
                        {
                            await HandleAction(callbackQuery.Data, callbackQuery, client, chatId.ToString());
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

        static async Task HandleAction(string action, CallbackQuery callbackQuery, TelegramBotClient client, string chatId)
        {
            switch (action)
            {
                case "description":
                    photo = 1;
                    Message message = await client.SendPhotoAsync(
                        chatId: chatId,
                        photo: InputFile.FromUri("https://github.com/Yarixxxxx/Photos/blob/main/house.jpg?raw=true"));
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
                    await SendRooms(callbackQuery, client);
                    break;
                case "features_photos":
                    await SendFeatures(callbackQuery, client);
                    break;
                case "exterior_photos":
                    await SendExterior(callbackQuery, client);
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
                    photo = 1;
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
                    photo = 1;
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
                    photo = 1;
                    await SendApartmentTelegram(callbackQuery, client);
                    break;
                case "apartment_rent":
                    await SendApartmentRent(callbackQuery, client);
                    break;
            }
        }
    }
}
