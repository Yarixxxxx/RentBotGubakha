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
