using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace Namespace
{
    class Buttons
    {
        public static async Task<Message> SendMainMenu(Message message, TelegramBotClient client)
        {
            string description = "Очаровательный дом в Губахе около склона на 10 человек. Прекрасное место для отдыха!";
            var keyboard = new InlineKeyboardMarkup(new[]
            {
                new []
                {
                    InlineKeyboardButton.WithCallbackData("Описание дома", "description"),
                    InlineKeyboardButton.WithCallbackData("Свободные даты", "schedule")
                },
                new []
                {
                    InlineKeyboardButton.WithCallbackData("Фото и видео", "photos_and_videos"),
                    InlineKeyboardButton.WithCallbackData("Геолокация", "location"),
                    InlineKeyboardButton.WithCallbackData("FAQ", "faq")
                },
                new []
                {
                    InlineKeyboardButton.WithCallbackData("Связь с арендодателем", "contact_landlord"),
                    InlineKeyboardButton.WithCallbackData("Другие варианты аренды", "other_options")
                }
            });

            return await client.SendTextMessageAsync(message.Chat.Id, description, replyMarkup: keyboard);
        }

        public static async Task<Message> SendDescriptionMenu(CallbackQuery callbackQuery, TelegramBotClient client)
        {
            string fullDescription = "Описывание дома";
            var keyboard = new InlineKeyboardMarkup(new[]
            {
                new []
                {
                    InlineKeyboardButton.WithCallbackData("Фото и видео", "photos_and_videos"),
                    InlineKeyboardButton.WithCallbackData("FAQ", "faq")
                },
                new []
                {
                    InlineKeyboardButton.WithCallbackData("Связь с арендодателем", "contact_landlord")
                },
                new []
                {
                    InlineKeyboardButton.WithCallbackData("Назад", "go_back")
                }
            });

            return await client.SendTextMessageAsync(callbackQuery.Message.Chat.Id, fullDescription, replyMarkup: keyboard);
        }

        public static async Task<Message> SendFAQ(CallbackQuery callbackQuery, TelegramBotClient client)
        {
            string link = "https://docs.google.com/document/d/1ZuA6ER-39P7vX5Vxt3c6SEX6tHtZPgsp1ZyELsZxW2s/edit?usp=sharing";
            string text_with_link = "Вы можете ознакомиться с часто задаваемыми вопросами " + link;
            var keyboard = new InlineKeyboardMarkup(new[]
            {
                new []
                {
                    InlineKeyboardButton.WithCallbackData("Назад", "go_back")
                }
            });

            return await client.SendTextMessageAsync(callbackQuery.Message.Chat.Id, text_with_link, replyMarkup: keyboard);
        }

        public static async Task<Message> PhotosAndVideos(CallbackQuery callbackQuery, TelegramBotClient client)
        {
            string text = "Выберите вариант фото или видео";
            var keyboard = new InlineKeyboardMarkup(new[]
            {
                new []
                {
                    InlineKeyboardButton.WithCallbackData("Фото", "only_photos"),
                    InlineKeyboardButton.WithCallbackData("Видео", "only_videos")
                },
                new []
                {
                    InlineKeyboardButton.WithCallbackData("Назад", "go_back")
                }
            });

            return await client.SendTextMessageAsync(callbackQuery.Message.Chat.Id, text, replyMarkup: keyboard);
        }

        public static async Task<Message> SendOnlyPhotos(CallbackQuery callbackQuery, TelegramBotClient client)
        {
            string text = "Это фото, выберите вид: ";
            var keyboard = new InlineKeyboardMarkup(new[]
            {
                new []
                {
                    InlineKeyboardButton.WithCallbackData("Фото комнат", "rooms_photos"),
                    InlineKeyboardButton.WithCallbackData("Фото развлечений", "features_photos"),
                    InlineKeyboardButton.WithCallbackData("Фото с улицы", "exterior_photos")
                },
                new []
                {
                    InlineKeyboardButton.WithCallbackData("Назад", "go_back")
                }
            });

            return await client.SendTextMessageAsync(callbackQuery.Message.Chat.Id, text, replyMarkup: keyboard);
        }

        public static async Task<Message> SendOnlyVideos(CallbackQuery callbackQuery, TelegramBotClient client)
        {
            string text = "Это видео ";
            var keyboard = new InlineKeyboardMarkup(new[]
            {
                new []
                {
                    InlineKeyboardButton.WithCallbackData("Назад", "go_back")
                }
            });

            return await client.SendTextMessageAsync(callbackQuery.Message.Chat.Id, text, replyMarkup: keyboard);
        }

        public static async Task SendRooms(CallbackQuery callbackQuery, TelegramBotClient client)
        {

            string[] photoUrls = new[]
            {
        "https://sun9-7.userapi.com/impg/HWrxyYZsRMXhCBEZPcPYug_sipMsHk1m_ftfng/1HSqaT9ew3M.jpg?size=1620x2160&quality=95&sign=b48f00a13ab3c98f3a4b8bd55fd26989&type=album",
        "https://sun9-37.userapi.com/impg/1UkgIpp_5v0oGTDdjwgE3INl-cMdgjqR6h-gwA/aNQqlp9gk0E.jpg?size=1620x2160&quality=95&sign=d3f26b2ed967ba206c053def0e4e70a4&type=album",
        "https://sun9-36.userapi.com/impg/f-rZT-Ge9IoX1hG9ysKv1juahU3plzT64OhKrg/9fmAPCjTIx0.jpg?size=1620x2160&quality=95&sign=22109c97c0c31ea3a01e2919b2997b54&type=album"
    };

            var mediaGroup = photoUrls.Select(url => new InputMediaPhoto(InputFile.FromUri(url))).ToList<IAlbumInputMedia>();


            var keyboard = new InlineKeyboardMarkup(new[]
            {
        new[]
        {
            InlineKeyboardButton.WithCallbackData("Назад", "go_back")
        }
    });

            await client.SendMediaGroupAsync(
                chatId: callbackQuery.Message.Chat.Id,
                media: mediaGroup
            );

            await client.SendTextMessageAsync(
                chatId: callbackQuery.Message.Chat.Id,
                text: "Выберите опцию:",
                replyMarkup: keyboard
            );
        }

        public static async Task<Message> SendFeatures(CallbackQuery callbackQuery, TelegramBotClient client)
        {
            string text = "Это развлечения ";
            var keyboard = new InlineKeyboardMarkup(new[]
            {
                new []
                {
                    InlineKeyboardButton.WithCallbackData("Назад", "go_back")
                }
            });

            return await client.SendTextMessageAsync(callbackQuery.Message.Chat.Id, text, replyMarkup: keyboard);
        }

        public static async Task<Message> SendExterior(CallbackQuery callbackQuery, TelegramBotClient client)
        {
            string text = "Это внешний облик ";
            var keyboard = new InlineKeyboardMarkup(new[]
            {
                new []
                {
                    InlineKeyboardButton.WithCallbackData("Назад", "go_back")
                }
            });

            return await client.SendTextMessageAsync(callbackQuery.Message.Chat.Id, text, replyMarkup: keyboard);
        }

        public static async Task<Message> SendLandlord(CallbackQuery callbackQuery, TelegramBotClient client)
        {
            string text = "Связаться с владельцем: ";
            var keyboard = new InlineKeyboardMarkup(new[]
            {
                new []
                {
                    InlineKeyboardButton.WithCallbackData("По телефону", "telephone"),
                    InlineKeyboardButton.WithCallbackData("По телеграму", "telegram")
                },
                new []
                {
                    InlineKeyboardButton.WithCallbackData("Снять", "rent")
                },
                new []
                {
                    InlineKeyboardButton.WithCallbackData("Назад", "go_back")
                }
            });

            return await client.SendTextMessageAsync(callbackQuery.Message.Chat.Id, text, replyMarkup: keyboard);
        }

        public static async Task<Message> SendTelephone(CallbackQuery callbackQuery, TelegramBotClient client)
        {
            string text = "8999999999999";
            var keyboard = new InlineKeyboardMarkup(new[]
            {
                new []
                {
                    InlineKeyboardButton.WithCallbackData("Назад", "go_back")
                }
            });

            return await client.SendTextMessageAsync(callbackQuery.Message.Chat.Id, text, replyMarkup: keyboard);
        }

        public static async Task<Message> SendSchedule(CallbackQuery callbackQuery, TelegramBotClient client)
        {
            string text = "Посмотрите расписание, найдите подходящий для себя день и договоритесь с владельцем - 899999999999 \n https://calendar.google.com/calendar/u/0/r/week/2024/2/27?pli=1";
            var keyboard = new InlineKeyboardMarkup(new[]
            {
                new []
                {
                    InlineKeyboardButton.WithCallbackData("Назад", "go_back")
                }
            });

            return await client.SendTextMessageAsync(callbackQuery.Message.Chat.Id, text, replyMarkup: keyboard);
        }

        public static async Task<Message> SendTelegram(CallbackQuery callbackQuery, TelegramBotClient client)
        {
            string text = "Вот контакт арендодателя";
            var keyboard = new InlineKeyboardMarkup(new[]
            {
                new []
                {
                    InlineKeyboardButton.WithCallbackData("Назад", "go_back")
                }
            });
            await client.SendContactAsync(callbackQuery.Message.Chat.Id, "89922210955", "Yaroslav");
            return await client.SendTextMessageAsync(callbackQuery.Message.Chat.Id, text, replyMarkup: keyboard);
        }

        public static async Task<Message> SendLocation(CallbackQuery callbackQuery, TelegramBotClient client)
        {
            var keyboard = new InlineKeyboardMarkup(new[]
            {
                new []
                {
                    InlineKeyboardButton.WithCallbackData("Назад", "go_back")
                }
            });
            return await client.SendLocationAsync(callbackQuery.Message.Chat.Id, 16.45, 34.76, replyMarkup: keyboard);
        }

        public static async Task<Message> SendOtherOptions(CallbackQuery callbackQuery, TelegramBotClient client)
        {
            string text = "Также, вы можете рассмотреть следующие варианты для аренды";
            var keyboard = new InlineKeyboardMarkup(new[]
            {
                new []
                {
                    InlineKeyboardButton.WithCallbackData("Дом 2", "house2"),
                    InlineKeyboardButton.WithCallbackData("Квартира", "apartment")
                },
                new []
                {
                    InlineKeyboardButton.WithCallbackData("Назад", "go_back")
                }
            });

            return await client.SendTextMessageAsync(callbackQuery.Message.Chat.Id, text, replyMarkup: keyboard);
        }

        public static async Task<Message> SendHouse2(CallbackQuery callbackQuery, TelegramBotClient client)
        {
            string text = "Дом 2";
            var keyboard = new InlineKeyboardMarkup(new[]
            {
                new []
                {
                    InlineKeyboardButton.WithCallbackData("Описание дома", "house2_description"),
                    InlineKeyboardButton.WithCallbackData("Свободные даты", "house2_schedule")
                },
                new []
                {
                    InlineKeyboardButton.WithCallbackData("Фото и видео", "house2_photos_and_videos"),
                    InlineKeyboardButton.WithCallbackData("Геолокация", "house2_location"),
                    InlineKeyboardButton.WithCallbackData("FAQ", "house2_faq")
                },
                new []
                {
                    InlineKeyboardButton.WithCallbackData("Связь с арендодателем", "house2_contact_landlord"),
                    InlineKeyboardButton.WithCallbackData("Другие варианты аренды", "house2_other_options")
                }
            });

            return await client.SendTextMessageAsync(callbackQuery.Message.Chat.Id, text, replyMarkup: keyboard);
        }

        public static async Task<Message> SendHouse2Description(CallbackQuery callbackQuery, TelegramBotClient client)
        {
            string fullDescription = "Описание дома 2";
            var keyboard = new InlineKeyboardMarkup(new[]
            {
                new []
                {
                    InlineKeyboardButton.WithCallbackData("Фото и видео", "house2_photos_and_videos"),
                    InlineKeyboardButton.WithCallbackData("FAQ", "house2_faq")
                },
                new []
                {
                    InlineKeyboardButton.WithCallbackData("Связь с арендодателем", "house2_contact_landlord")
                },
                new []
                {
                    InlineKeyboardButton.WithCallbackData("Назад", "go_back")
                }
            });

            return await client.SendTextMessageAsync(callbackQuery.Message.Chat.Id, fullDescription, replyMarkup: keyboard);
        }

        public static async Task<Message> SendHouse2Schedule(CallbackQuery callbackQuery, TelegramBotClient client)
        {
            string text = "Свободные даты дома 2";
            var keyboard = new InlineKeyboardMarkup(new[]
            {
                new []
                {
                    InlineKeyboardButton.WithCallbackData("Назад", "go_back")
                }
            });

            return await client.SendTextMessageAsync(callbackQuery.Message.Chat.Id, text, replyMarkup: keyboard);
        }

        public static async Task<Message> SendHouse2PhotosAndVideos(CallbackQuery callbackQuery, TelegramBotClient client)
        {
            string text = "Фото и видео дома 2";
            var keyboard = new InlineKeyboardMarkup(new[]
            {
                new []
                {
                    InlineKeyboardButton.WithCallbackData("Фото", "house2_only_photos"),
                    InlineKeyboardButton.WithCallbackData("Видео", "house2_only_videos")
                },
                new []
                {
                    InlineKeyboardButton.WithCallbackData("Назад", "go_back")
                }
            });

            return await client.SendTextMessageAsync(callbackQuery.Message.Chat.Id, text, replyMarkup: keyboard);
        }

        public static async Task<Message> SendHouse2OnlyPhotos(CallbackQuery callbackQuery, TelegramBotClient client)
        {
            string text = "Это фото дома 2: ";
            var keyboard = new InlineKeyboardMarkup(new[]
            {
                new []
                {
                    InlineKeyboardButton.WithCallbackData("Фото комнат", "house2_rooms_photos"),
                    InlineKeyboardButton.WithCallbackData("Фото развлечений", "house2_features_photos"),
                    InlineKeyboardButton.WithCallbackData("Фото с улицы", "house2_exterior_photos")
                },
                new []
                {
                    InlineKeyboardButton.WithCallbackData("Назад", "go_back")
                }
            });

            return await client.SendTextMessageAsync(callbackQuery.Message.Chat.Id, text, replyMarkup: keyboard);
        }

        public static async Task<Message> SendHouse2OnlyVideos(CallbackQuery callbackQuery, TelegramBotClient client)
        {
            string text = "Это видео дома 2";
            var keyboard = new InlineKeyboardMarkup(new[]
            {
                new []
                {
                    InlineKeyboardButton.WithCallbackData("Назад", "go_back")
                }
            });

            return await client.SendTextMessageAsync(callbackQuery.Message.Chat.Id, text, replyMarkup: keyboard);
        }

        

        public static async Task<Message> SendHouse2Location(CallbackQuery callbackQuery, TelegramBotClient client)
        {
            var keyboard = new InlineKeyboardMarkup(new[]
            {
                new []
                {
                    InlineKeyboardButton.WithCallbackData("Назад", "go_back")
                }
            });

            return await client.SendLocationAsync(callbackQuery.Message.Chat.Id, 16.45, 34.76, replyMarkup: keyboard);
        }

        public static async Task<Message> SendHouse2FAQ(CallbackQuery callbackQuery, TelegramBotClient client)
        {
            string link = "https://docs.google.com/document/d/1ZuA6ER-39P7vX5Vxt3c6SEX6tHtZPgsp1ZyELsZxW2s/edit?usp=sharing";
            string text_with_link = "Вы можете ознакомиться с часто задаваемыми вопросами по дому 2 " + link;
            var keyboard = new InlineKeyboardMarkup(new[]
            {
                new []
                {
                    InlineKeyboardButton.WithCallbackData("Назад", "go_back")
                }
            });

            return await client.SendTextMessageAsync(callbackQuery.Message.Chat.Id, text_with_link, replyMarkup: keyboard);
        }

        public static async Task<Message> SendHouse2Landlord(CallbackQuery callbackQuery, TelegramBotClient client)
        {
            string text = "Связаться с владельцем дома 2: ";
            var keyboard = new InlineKeyboardMarkup(new[]
            {
                new []
                {
                    InlineKeyboardButton.WithCallbackData("По телефону", "house2_telephone"),
                    InlineKeyboardButton.WithCallbackData("По телеграму", "house2_telegram")
                },
                new []
                {
                    InlineKeyboardButton.WithCallbackData("Снять", "house2_rent")
                },
                new []
                {
                    InlineKeyboardButton.WithCallbackData("Назад", "go_back")
                }
            });

            return await client.SendTextMessageAsync(callbackQuery.Message.Chat.Id, text, replyMarkup: keyboard);
        }

        public static async Task<Message> SendHouse2OtherOptions(CallbackQuery callbackQuery, TelegramBotClient client)
        {
            string text = "Другие варианты аренды для дома 2";
            var keyboard = new InlineKeyboardMarkup(new[]
            {
                new []
                {
                    InlineKeyboardButton.WithCallbackData("Дом 1", "house"),
                    InlineKeyboardButton.WithCallbackData("Квартира", "apartment")
                },
                new []
                {
                    InlineKeyboardButton.WithCallbackData("Назад", "go_back")
                }
            });

            return await client.SendTextMessageAsync(callbackQuery.Message.Chat.Id, text, replyMarkup: keyboard);
        }

        public static async Task<Message> SendHouse2Telephone(CallbackQuery callbackQuery, TelegramBotClient client)
        {
            string text = "8999999999999";
            var keyboard = new InlineKeyboardMarkup(new[]
            {
                new []
                {
                    InlineKeyboardButton.WithCallbackData("Назад", "go_back")
                }
            });

            return await client.SendTextMessageAsync(callbackQuery.Message.Chat.Id, text, replyMarkup: keyboard);
        }

        public static async Task<Message> SendHouse2Telegram(CallbackQuery callbackQuery, TelegramBotClient client)
        {
            string text = "Вот контакт арендодателя для дома 2";
            var keyboard = new InlineKeyboardMarkup(new[]
            {
                new []
                {
                    InlineKeyboardButton.WithCallbackData("Назад", "go_back")
                }
            });
            await client.SendContactAsync(callbackQuery.Message.Chat.Id, "89922210955", "Yaroslav");
            return await client.SendTextMessageAsync(callbackQuery.Message.Chat.Id, text, replyMarkup: keyboard);
        }

        public static async Task<Message> SendHouse2Rent(CallbackQuery callbackQuery, TelegramBotClient client)
        {
            string text = "Посмотрите расписание, найдите подходящий для себя день и договоритесь с владельцем дома 2 - 899999999999 \n https://calendar.google.com/calendar/u/0/r/week/2024/2/27?pli=1";
            var keyboard = new InlineKeyboardMarkup(new[]
            {
                new []
                {
                    InlineKeyboardButton.WithCallbackData("Назад", "go_back")
                }
            });

            return await client.SendTextMessageAsync(callbackQuery.Message.Chat.Id, text, replyMarkup: keyboard);
        }

        // New methods for "квартира"
        public static async Task<Message> SendApartment(CallbackQuery callbackQuery, TelegramBotClient client)
        {
            string text = "Замечательная квартира в Губахе!";
            var keyboard = new InlineKeyboardMarkup(new[]
            {
                new []
                {
                    InlineKeyboardButton.WithCallbackData("Описание квартиры", "apartment_description"),
                    InlineKeyboardButton.WithCallbackData("Свободные даты", "apartment_schedule")
                },
                new []
                {
                    InlineKeyboardButton.WithCallbackData("Фото и видео", "apartment_photos_and_videos"),
                    InlineKeyboardButton.WithCallbackData("Геолокация", "apartment_location"),
                    InlineKeyboardButton.WithCallbackData("FAQ", "apartment_faq")
                },
                new []
                {
                    InlineKeyboardButton.WithCallbackData("Связь с арендодателем", "apartment_contact_landlord"),
                    InlineKeyboardButton.WithCallbackData("Другие варианты аренды", "apartment_other_options")
                }
            });

            return await client.SendTextMessageAsync(callbackQuery.Message.Chat.Id, text, replyMarkup: keyboard);
        }

        public static async Task<Message> SendApartmentDescription(CallbackQuery callbackQuery, TelegramBotClient client)
        {
            string fullDescription = "Описание квартиры";
            var keyboard = new InlineKeyboardMarkup(new[]
            {
                new []
                {
                    InlineKeyboardButton.WithCallbackData("Фото и видео", "apartment_photos_and_videos"),
                    InlineKeyboardButton.WithCallbackData("FAQ", "apartment_faq")
                },
                new []
                {
                    InlineKeyboardButton.WithCallbackData("Связь с арендодателем", "apartment_contact_landlord")
                },
                new []
                {
                    InlineKeyboardButton.WithCallbackData("Назад", "go_back")
                }
            });

            return await client.SendTextMessageAsync(callbackQuery.Message.Chat.Id, fullDescription, replyMarkup: keyboard);
        }

        public static async Task<Message> SendApartmentSchedule(CallbackQuery callbackQuery, TelegramBotClient client)
        {
            string text = "Свободные даты квартиры";
            var keyboard = new InlineKeyboardMarkup(new[]
            {
                new []
                {
                    InlineKeyboardButton.WithCallbackData("Назад", "go_back")
                }
            });

            return await client.SendTextMessageAsync(callbackQuery.Message.Chat.Id, text, replyMarkup: keyboard);
        }

        public static async Task<Message> SendApartmentPhotosAndVideos(CallbackQuery callbackQuery, TelegramBotClient client)
        {
            string text = "Фото и видео квартиры";
            var keyboard = new InlineKeyboardMarkup(new[]
            {
                new []
                {
                    InlineKeyboardButton.WithCallbackData("Фото", "apartment_only_photos"),
                    InlineKeyboardButton.WithCallbackData("Видео", "apartment_only_videos")
                },
                new []
                {
                    InlineKeyboardButton.WithCallbackData("Назад", "go_back")
                }
            });

            return await client.SendTextMessageAsync(callbackQuery.Message.Chat.Id, text, replyMarkup: keyboard);
        }

        public static async Task<Message> SendApartmentOnlyPhotos(CallbackQuery callbackQuery, TelegramBotClient client)
        {
            string text = "Это фото квартиры: ";
            var keyboard = new InlineKeyboardMarkup(new[]
            {
                new []
                {
                    InlineKeyboardButton.WithCallbackData("Фото комнат", "apartment_rooms_photos"),
                    InlineKeyboardButton.WithCallbackData("Фото развлечений", "apartment_features_photos"),
                    InlineKeyboardButton.WithCallbackData("Фото с улицы", "apartment_exterior_photos")
                },
                new []
                {
                    InlineKeyboardButton.WithCallbackData("Назад", "go_back")
                }
            });

            return await client.SendTextMessageAsync(callbackQuery.Message.Chat.Id, text, replyMarkup: keyboard);
        }

        public static async Task<Message> SendApartmentOnlyVideos(CallbackQuery callbackQuery, TelegramBotClient client)
        {
            string text = "Это видео квартиры";
            var keyboard = new InlineKeyboardMarkup(new[]
            {
                new []
                {
                    InlineKeyboardButton.WithCallbackData("Назад", "go_back")
                }
            });

            return await client.SendTextMessageAsync(callbackQuery.Message.Chat.Id, text, replyMarkup: keyboard);
        }

        public static async Task<Message> SendApartmentRooms(CallbackQuery callbackQuery, TelegramBotClient client)
        {
            string text = "Это комнаты квартиры";
            var keyboard = new InlineKeyboardMarkup(new[]
            {
                new []
                {
                    InlineKeyboardButton.WithCallbackData("Назад", "go_back")
                }
            });

            return await client.SendTextMessageAsync(callbackQuery.Message.Chat.Id, text, replyMarkup: keyboard);
        }

        public static async Task<Message> SendApartmentFeatures(CallbackQuery callbackQuery, TelegramBotClient client)
        {
            string text = "Это развлечения квартиры";
            var keyboard = new InlineKeyboardMarkup(new[]
            {
                new []
                {
                    InlineKeyboardButton.WithCallbackData("Назад", "go_back")
                }
            });

            return await client.SendTextMessageAsync(callbackQuery.Message.Chat.Id, text, replyMarkup: keyboard);
        }

        public static async Task<Message> SendApartmentExterior(CallbackQuery callbackQuery, TelegramBotClient client)
        {
            string text = "Это внешний облик квартиры";
            var keyboard = new InlineKeyboardMarkup(new[]
            {
                new []
                {
                    InlineKeyboardButton.WithCallbackData("Назад", "go_back")
                }
            });

            return await client.SendTextMessageAsync(callbackQuery.Message.Chat.Id, text, replyMarkup: keyboard);
        }

        public static async Task<Message> SendApartmentLocation(CallbackQuery callbackQuery, TelegramBotClient client)
        {
            var keyboard = new InlineKeyboardMarkup(new[]
            {
                new []
                {
                    InlineKeyboardButton.WithCallbackData("Назад", "go_back")
                }
            });

            return await client.SendLocationAsync(callbackQuery.Message.Chat.Id, 16.45, 34.76, replyMarkup: keyboard);
        }

        public static async Task<Message> SendApartmentFAQ(CallbackQuery callbackQuery, TelegramBotClient client)
        {
            string link = "https://docs.google.com/document/d/1ZuA6ER-39P7vX5Vxt3c6SEX6tHtZPgsp1ZyELsZxW2s/edit?usp=sharing";
            string text_with_link = "Вы можете ознакомиться с часто задаваемыми вопросами по квартире " + link;
            var keyboard = new InlineKeyboardMarkup(new[]
            {
                new []
                {
                    InlineKeyboardButton.WithCallbackData("Назад", "go_back")
                }
            });

            return await client.SendTextMessageAsync(callbackQuery.Message.Chat.Id, text_with_link, replyMarkup: keyboard);
        }

        public static async Task<Message> SendApartmentLandlord(CallbackQuery callbackQuery, TelegramBotClient client)
        {
            string text = "Связаться с владельцем квартиры: ";
            var keyboard = new InlineKeyboardMarkup(new[]
            {
                new []
                {
                    InlineKeyboardButton.WithCallbackData("По телефону", "apartment_telephone"),
                    InlineKeyboardButton.WithCallbackData("По телеграму", "apartment_telegram")
                },
                new []
                {
                    InlineKeyboardButton.WithCallbackData("Снять", "apartment_rent")
                },
                new []
                {
                    InlineKeyboardButton.WithCallbackData("Назад", "go_back")
                }
            });

            return await client.SendTextMessageAsync(callbackQuery.Message.Chat.Id, text, replyMarkup: keyboard);
        }

        public static async Task<Message> SendApartmentOtherOptions(CallbackQuery callbackQuery, TelegramBotClient client)
        {
            string text = "Другие варианты аренды для квартиры";
            var keyboard = new InlineKeyboardMarkup(new[]
            {
                new []
                {
                    InlineKeyboardButton.WithCallbackData("Дом 1", "house"),
                    InlineKeyboardButton.WithCallbackData("Дом 2", "house2")
                },
                new []
                {
                    InlineKeyboardButton.WithCallbackData("Назад", "go_back")
                }
            });

            return await client.SendTextMessageAsync(callbackQuery.Message.Chat.Id, text, replyMarkup: keyboard);
        }

        public static async Task<Message> SendApartmentTelephone(CallbackQuery callbackQuery, TelegramBotClient client)
        {
            string text = "8999999999999";
            var keyboard = new InlineKeyboardMarkup(new[]
            {
                new []
                {
                    InlineKeyboardButton.WithCallbackData("Назад", "go_back")
                }
            });

            return await client.SendTextMessageAsync(callbackQuery.Message.Chat.Id, text, replyMarkup: keyboard);
        }

        public static async Task<Message> SendApartmentTelegram(CallbackQuery callbackQuery, TelegramBotClient client)
        {
            string text = "Вот контакт арендодателя для квартиры";
            var keyboard = new InlineKeyboardMarkup(new[]
            {
                new []
                {
                    InlineKeyboardButton.WithCallbackData("Назад", "go_back")
                }
            });
            await client.SendContactAsync(callbackQuery.Message.Chat.Id, "89922210955", "Yaroslav");
            return await client.SendTextMessageAsync(callbackQuery.Message.Chat.Id, text, replyMarkup: keyboard);
        }

        public static async Task<Message> SendApartmentRent(CallbackQuery callbackQuery, TelegramBotClient client)
        {
            string text = "Посмотрите расписание, найдите подходящий для себя день и договоритесь с владельцем квартиры - 899999999999 \n https://calendar.google.com/calendar/u/0/r/week/2024/2/27?pli=1";
            var keyboard = new InlineKeyboardMarkup(new[]
            {
                new []
                {
                    InlineKeyboardButton.WithCallbackData("Назад", "go_back")
                }
            });

            return await client.SendTextMessageAsync(callbackQuery.Message.Chat.Id, text, replyMarkup: keyboard);
        }
    }
}