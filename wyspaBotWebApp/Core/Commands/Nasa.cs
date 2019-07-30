using System.Collections.Generic;

namespace wyspaBotWebApp.Core.Commands {
    public class NasaPictureOfTheDay : BaseCommand {
        public NasaPictureOfTheDay() {
            Aliases = new List<string> {"npod"};
            Code = (splitInput, botName, postedMessages, chatUsers) => GetMessageToDisplay(CommandType.NasaPictureOfTheDayCommand);
        }
    }
}