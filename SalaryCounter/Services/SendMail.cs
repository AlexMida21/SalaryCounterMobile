using MailKit.Security;
using MimeKit;

namespace SalaryCounter.Services;
public class SendMail
{
    public void SendEmailWithAttachment(string senderEmail, string senderPassword, string recipientEmail, string subject, string body, string str)
    {
        var message = new MimeMessage();
        message.From.Add(new MailboxAddress("", senderEmail));
        message.To.Add(new MailboxAddress("", recipientEmail));
        message.Subject = subject;

        var builder = new BodyBuilder();
        builder.TextBody = body;

        // Чтение файла вложения
        var attachment = builder.Attachments.Add(AppRepository.HolderPath);
        message.Body = builder.ToMessageBody();
        using (var client = new MailKit.Net.Smtp.SmtpClient())
        {
            client.CheckCertificateRevocation = false;
            client.Connect("smtp.yandex.ru", 465, SecureSocketOptions.SslOnConnect);
            client.Authenticate(senderEmail, senderPassword);
            client.Send(message);
            client.Disconnect(true);
        }
    }

    public void SendFileMail(string str)
    {
        string senderEmail = "alexeimatsalo@ya.ru";
        string senderPassword = "huadgexzskmirrdx";
        string recipientEmail = MainPage.mailHolder;
        string subject = ("Отчётность - " + str);
        string body = "Сгенерированный csv файл: ";

        SendEmailWithAttachment(senderEmail, senderPassword, recipientEmail, subject, body, str);
    }
}
