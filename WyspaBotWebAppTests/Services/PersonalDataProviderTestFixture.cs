using NUnit.Framework;
using wyspaBotWebApp.Services;
using wyspaBotWebApp.Services.Providers.PersonalData;

namespace WyspaBotWebAppTests.Services {
    [TestFixture]
    public class PersonalDataProviderTestFixture {
        private IPersonalDataProvider testee;

        [SetUp]
        public void SetUp() {
        }

        [TestCase("qwe@rty.com", "qwe@rty.com")]
        [TestCase("asjhd@gmail.com", "asjhd@gmail.com")]
        [TestCase("asasas.com", "")]
        [TestCase("11@11.11", "")]
        [TestCase("abcdefgh", "")]
        public void GetEmailForBackups_Works(string email, string expectedValue) {
            this.testee = new PersonalDataProvider(email);
            var result = this.testee.GetEmailForBackups();
            Assert.That(result, Is.EqualTo(expectedValue));
        }
    }
}