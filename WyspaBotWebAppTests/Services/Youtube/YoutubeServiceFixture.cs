using NUnit.Framework;
using Rhino.Mocks;
using wyspaBotWebApp.Services;
using wyspaBotWebApp.Services.Youtube;

namespace WyspaBotWebAppTests.Services.Youtube {
    [TestFixture]
    public class YoutubeServiceFixture {
        private IYoutubeService testee;

        private IRequestsService requestsService;

        [SetUp]
        public void SetUp() {
            this.requestsService = MockRepository.GenerateMock<IRequestsService>();

            this.testee = new YoutubeService(this.requestsService, string.Empty);
        }

        [TestCase("https://www.youtube.com/watch?v=costam&feature=youtu.be&t=579", true)]
        [TestCase("https://youtu.be/costam?t=579", true)]
        [TestCase("https://www.youtube.com/watch?v=costam  ", true)]
        [TestCase("www.youtube.com/watch?v=costam  ", true)]
        [TestCase("youtube.com/watch?v=costam  ", true)]
        [TestCase("https://www.youtube.com/watch?v=costam  asdqwfqg", false)]
        [TestCase("https://youtu.be", false)]
        [TestCase("www.youtu.be", false)]
        [TestCase("youtu.be", false)]
        [TestCase("https://www.youtube.com/", false)]
        [TestCase("www.youtube.com", false)]
        [TestCase("youtube.com", false)]
        public void IsYoutubeVideoLink_Works(string url, bool isYoutubeLinkExcepted) {
            var isYoutubeLink = this.testee.IsYoutubeVideoLink(url);
            Assert.That(isYoutubeLink, Is.EqualTo(isYoutubeLinkExcepted));

            var isYoutubeLink2 = this.testee.IsYoutubeVideoLink($"{url}");
            Assert.That(isYoutubeLink2, Is.EqualTo(isYoutubeLinkExcepted));
        }

        [TestCase("https://www.youtube.com/watch?v=1qSTcxt2t74", "1qSTcxt2t74")]
        [TestCase("https://www.youtube.com/watch?v=1qSTcxt2t74&t=579", "1qSTcxt2t74")]
        [TestCase("https://youtu.be/1qSTcxt2t74?t=579 ", "1qSTcxt2t74")]
        [TestCase("https://youtu.be/1qSTcxt2t74 ", "1qSTcxt2t74")]
        [TestCase("www.youtu.be/1qSTcxt2t74?t=579 ", "1qSTcxt2t74")]
        [TestCase("youtu.be/1qSTcxt2t74?t=579 ", "1qSTcxt2t74")]
        [TestCase("https://www.youtube.com/watch?v=1qSTcxt2t74&feature=youtu.be&t=579 ", "1qSTcxt2t74")]
        public void GetVideoId_Works(string url, string id) {
            var videoId = this.testee.GetVideoId(url);

            Assert.That(videoId, Is.EqualTo(id));
        }
    }
}