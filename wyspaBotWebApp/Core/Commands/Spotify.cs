using System.Collections.Generic;

namespace wyspaBotWebApp.Core.Commands {
    public class ReccomendedTracksBasedOnTrack : BaseCommand {
        public ReccomendedTracksBasedOnTrack() {
            Aliases = new List<string> {"rtracks"};
            Code = (splitInput, botName, postedMessages, chatUsers) => {
                if (splitInput.Count >= 6) {
                    var trackId = splitInput[5];
                    var limit = 10;

                    if (splitInput.Count >= 7) {
                        var limitAsString = splitInput[6];
                        int.TryParse(limitAsString, out limit);
                    }

                    return GetMessageToDisplay(CommandType.RecommendedTracksBasedOnTrackCommand, trackId, limit);
                }

                return new List<string>();
            };
        }
    }
}