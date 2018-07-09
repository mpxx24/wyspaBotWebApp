using System.Web.Mvc;
using wyspaBotWebApp.Services;

namespace wyspaBotWebApp.Controllers {
    public class HomeController : Controller {
        private readonly IWyspaBotService wyspaBotService;

        public HomeController(IWyspaBotService wyspaBotService) {
            this.wyspaBotService = wyspaBotService;
        }

        public ActionResult Index() {
            return this.View();
        }

        public ActionResult About() {
            this.ViewBag.Message = "Your application description page.";

            return this.View();
        }

        public ActionResult Contact() {
            this.ViewBag.Message = "Your contact page.";

            return this.View();
        }

        public void StartBot() {
            this.wyspaBotService.StartBot();
        }

        public void StopBot() {
            this.wyspaBotService.StopBot();
        }
    }
}