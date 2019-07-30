using System.Collections.Generic;

namespace wyspaBotWebApp.Core.Commands {
    public class GoogleMapDistance : BaseCommand {
        public GoogleMapDistance() {
            Aliases = new List<string> { "gmdistance" };
            Code = (splitInput, botName, postedMessages, chatUsers) => {
                if (splitInput.Count >= 7) {
                    var origin = splitInput[5];
                    var destination = splitInput[6];

                    return GetMessageToDisplay(CommandType.GoogleMapDistanceCommand, new List<string> {origin, destination});
                }

                return GetMessageToDisplay(CommandType.LogErrorCommand, "You need to specify both: origin and destination!)");
            };
        }
    }
}