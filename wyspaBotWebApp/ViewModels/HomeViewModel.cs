using System.ComponentModel.DataAnnotations;

namespace wyspaBotWebApp.ViewModels {
    public class HomeViewModel {
        [DataType(DataType.MultilineText)]
        public string Logs { get; set; }
    }
}