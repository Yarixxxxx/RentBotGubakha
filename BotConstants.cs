using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Google.Apis.Sheets.v4.Data;
using Google.Apis.Sheets.v4;
using Google.Apis.Util.Store;
using System.Collections.Generic;
using System.IO;

namespace Namespace
{
    public class BotDataConstants
    {
        public const string Token = "YOUR_BOT_TOKEN_HERE";
        public static readonly string PhotoFolder = "photos";
        public static readonly string MainMenuPhotoPath = Path.Combine(PhotoFolder, "main_menu_photo.jpg");

        public static Dictionary<int, object> UserLocation = new Dictionary<int, object>();
        public static Dictionary<int, object> UserLastRequest = new Dictionary<int, object>();

        public const string Point = "point";
    }
}



//static async Task UploadStatistics(long chatId, string action, string username)
//{
//    var spreadsheetId = "1q7CAgj_Gp_wEcFHA_1Fde5jo-IXIZeKPgkjXicdfADU";
//    var range = "Лист1!A:D";

//    var requestBody = new ValueRange
//    {
//        Values = new List<IList<object>>
//                {
//                    new List<object> { DateTime.Now.ToString("s"), "@" + username, action, rentButtonClicks[chatId] }
//                }
//    };

//    var appendRequest = sheetsService.Spreadsheets.Values.Append(requestBody, spreadsheetId, range);
//    appendRequest.ValueInputOption = SpreadsheetsResource.ValuesResource.AppendRequest.ValueInputOptionEnum.USERENTERED;

//    var appendResponse = await appendRequest.ExecuteAsync();
//    Console.WriteLine($"Statistics uploaded successfully for username @{username} and action {action}.");

//    await UpdateUserCount();
//}

//static async Task UpdateUserCount()
//{
//    var spreadsheetId = "1q7CAgj_Gp_wEcFHA_1Fde5jo-IXIZeKPgkjXicdfADU";
//    var userCountRange = "Лист1!F1:F2";

//    var userCountBody = new ValueRange
//    {
//        Values = new List<IList<object>>
//                {
//                    new List<object> { "Кол-во пользователей" },
//                    new List<object> { uniqueUsers.Count }
//                }
//    };

//    var updateRequest = sheetsService.Spreadsheets.Values.Update(userCountBody, spreadsheetId, userCountRange);
//    updateRequest.ValueInputOption = SpreadsheetsResource.ValuesResource.UpdateRequest.ValueInputOptionEnum.USERENTERED;

//    await updateRequest.ExecuteAsync();
//    Console.WriteLine("User count updated in F1 and F2.");
//}