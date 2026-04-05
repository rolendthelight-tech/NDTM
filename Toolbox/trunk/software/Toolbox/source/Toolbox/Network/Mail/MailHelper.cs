using System.Collections.Generic;
using System.Net;
using System.Net.Mail;
using System.Text;

namespace Toolbox.Network.Mail
{
  public class MailHelper
  {
  	private static log4net.ILog _log = log4net.LogManager.GetLogger(typeof (MailHelper));

    public string ServerName { get; set; }

    public int ServerPort { get; set; }

    public string Email { get; set; }

    public string Login { get; set; }

    public string Password { get; set; }

    public List<Attachment> Attachments { get; set; }

    public void Send(string recipientEmail, string subject, string body, bool IsBodyHtml)
    {
      SmtpClient smtp = new SmtpClient(ServerName, ServerPort);
      smtp.EnableSsl = true;
      smtp.Credentials = new NetworkCredential(Login, Password);
      MailMessage message = new MailMessage();
      message.From = new MailAddress(Email);
      message.To.Add(new MailAddress(recipientEmail));
      message.SubjectEncoding = Encoding.UTF8;
      message.Subject = subject;
      message.BodyEncoding = Encoding.UTF8;
      message.IsBodyHtml = IsBodyHtml;
      message.Body = body;

      if (Attachments != null && Attachments.Count > 0)
        foreach (var a in Attachments)
          message.Attachments.Add(a);

      smtp.Send(message);
    }

    public void Send(string recipientEmail, string subject, string body)
    {
      Send(recipientEmail, subject,body, false);
    }
  }
}
