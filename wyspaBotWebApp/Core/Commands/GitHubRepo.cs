using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace wyspaBotWebApp.Core.Commands
{
    public class GitHubRepo : BaseCommand
    {
        public GitHubRepo() {
            Aliases = new List<string> { "ghrepo" };
            Code = (splitInput, botName, postedMessages, chatUsers) => GetMessageToDisplay(CommandType.GetRepositoryAddressCommand);
        }
    }
}