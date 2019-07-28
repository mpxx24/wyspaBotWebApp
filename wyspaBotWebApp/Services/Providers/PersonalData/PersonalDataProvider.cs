using System.ComponentModel.DataAnnotations;

namespace wyspaBotWebApp.Services.Providers.PersonalData {
    public class PersonalDataProvider : IPersonalDataProvider {
        private readonly string emailAddress;

        public PersonalDataProvider(string emailAddress) {
            this.emailAddress = emailAddress;
        }

        public string GetEmailForBackups() {
            var emailAddressAttribute = new EmailAddressAttribute();
            return emailAddressAttribute.IsValid(this.emailAddress)
                ? this.emailAddress
                : string.Empty;
        }
    }
}