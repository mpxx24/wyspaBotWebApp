using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using NLog;

namespace wyspaBotWebApp.Services.Email {
    public class EmailService : IEmailService {
        private readonly ILogger logger = LogManager.GetCurrentClassLogger();

        private readonly string mailSenderAddress;

        private readonly string mailSenderPassword;

        public EmailService(string mailSenderAddress, string mailSenderPassword) {
            this.mailSenderAddress = mailSenderAddress;
            this.mailSenderPassword = mailSenderPassword;
        }

        public void SendEmailWithAttachments(IList<string> recipients, string subject, string body, IList<string> attachments) {
            try {
                using (var mail = new MailMessage()) {
                    if (recipients.All(string.IsNullOrEmpty)) {
                        return;
                    }

                    mail.From = new MailAddress(this.mailSenderAddress);

                    foreach (var recipient in recipients) {
                        if (!string.IsNullOrEmpty(recipient)) {
                            mail.To.Add(recipient);
                        }
                    }

                    mail.Subject = subject;
                    mail.Body = body;
                    mail.IsBodyHtml = false;

                    foreach (var attachment in attachments) {
                        if (!string.IsNullOrEmpty(attachment)) {
                            mail.Attachments.Add(new Attachment(attachment));
                        }
                    }

                    //may be hardcoded, w/e
                    using (var smtp = new SmtpClient("smtp.gmail.com", 587)) {
                        smtp.Credentials = new NetworkCredential(this.mailSenderAddress, this.mailSenderPassword);
                        smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
                        smtp.EnableSsl = true;
                        smtp.Send(mail);
                    }
                }
            }
            catch (Exception e) {
                this.logger.Debug($"Failed to send an email! {e}");
            }
        }
    }
}