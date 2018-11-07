namespace wyspaBotWebApp.Services.Youtube {
    public interface IYoutubeService {
        string GetVideoName(string url);
        bool IsYoutubeLink(string text);
    }
}