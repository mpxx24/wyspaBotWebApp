using System.Collections.Generic;
using System.Text;
using System.Web.Mvc;
using wyspaBotWebApp.Services.Providers.Logs;
using wyspaBotWebApp.ViewModels;

namespace wyspaBotWebApp.Controllers {
    public class HomeController : Controller {
        private readonly ILogsProvider logsProvider;

        public HomeController(ILogsProvider logsProvider) {
            this.logsProvider = logsProvider;
        }

        public ActionResult Index() {
            var logs = this.logsProvider.GetCurrentLogs();
            var viewModel = new HomeViewModel {Logs = logs};
            return this.View(viewModel);
        }

        public ActionResult About() {
            this.ViewBag.Message = "Your application description page.";

            return this.View();
        }

        public ActionResult Contact() {
            this.ViewBag.Message = "Your contact page.";

            return this.View();
        }
    }
}