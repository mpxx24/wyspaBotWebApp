using System;

namespace wyspaBotWebApp.Core {
    public class ApplicationSettingsHelper {
        public static string DateTimeFormat => "dd/MM/yyyy";

        public static TimeZoneInfo TimeZoneInfo => TimeZoneInfo.FindSystemTimeZoneById("Central European Standard Time");

        public static string PathToDbFile = "d:\\home\\site\\wwwroot\\dbFile.sqllite";

        public static string PathToLogFile = "d:\\home\\site\\wwwroot\\wyspaBotLog.txt";
    }
}