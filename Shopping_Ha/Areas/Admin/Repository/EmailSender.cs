using System.Net.Mail;
using System.Net;

namespace Shopping_Ha.Areas.Admin.Repository
{
	public class EmailSender:IEmailSender
	{
		public Task SendEmailAsync(string email, string subject, string message)
		{
			var client = new SmtpClient("smtp.gmail.com", 587)
			{
				EnableSsl = true, //bật bảo mật
				UseDefaultCredentials = false,
				Credentials = new NetworkCredential("logindemo53@gmail.com", "yvmnbarmzbrafmdh")
			};

			return client.SendMailAsync(
				new MailMessage(from: "nguyenducan1526@gmail.com",
								to: email,
								subject,
								message
								));
		}
	}
}
