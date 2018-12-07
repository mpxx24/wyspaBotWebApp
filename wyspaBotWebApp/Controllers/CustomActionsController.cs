using System.Web.Mvc;
using wyspaBotWebApp.Services;
using wyspaBotWebApp.Services.Markov;

namespace wyspaBotWebApp.Controllers {
    public class CustomActionsController : Controller {
        private readonly IMarkovService markovService;
        private readonly IWyspaBotService wyspaBotService;

        public CustomActionsController(IWyspaBotService wyspaBotService, IMarkovService markovService) {
            this.wyspaBotService = wyspaBotService;
            this.markovService = markovService;
        }

        public void StartBot() {
            this.wyspaBotService.StartBot();
        }

        public void StopBot() {
            this.wyspaBotService.StopBot();
        }

        public void PersistMarkovState() {
            this.markovService.PersistMarkovObject();
        }
    }
}