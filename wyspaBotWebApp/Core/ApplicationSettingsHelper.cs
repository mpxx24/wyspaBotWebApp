using System;

namespace wyspaBotWebApp.Core {
    public class ApplicationSettingsHelper {
        public static string DateTimeFormat => "dd/MM/yyyy";

        public static TimeZoneInfo TimeZoneInfo => TimeZoneInfo.FindSystemTimeZoneById("Central European Standard Time");
    }
}