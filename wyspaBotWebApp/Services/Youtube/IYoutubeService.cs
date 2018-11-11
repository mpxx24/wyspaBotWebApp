namespace wyspaBotWebApp.Services.Youtube {
    public interface IYoutubeService {
        string GetVideoName(string url);
        bool IsYoutubeVideoLink(string text);
        string GetVideoId(string link);
    }
}