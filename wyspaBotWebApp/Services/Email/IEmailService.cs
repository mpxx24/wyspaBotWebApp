using System.Collections.Generic;

namespace wyspaBotWebApp.Services.Email {
    public interface IEmailService {
        void SendEmailWithAttachments(IList<string> recipients, string subject, string body, IList<string> attachments);
    }
}