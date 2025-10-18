using System.Net.Mail;
using System.Net;
using System.Threading.Tasks;

public interface IEmailService
{
    Task SendOrderConfirmationAsync(string toEmail, string userName, DateTime reservedDate, string status, bool isupdate=false);
}

public class EmailService : IEmailService
{
    public async Task SendOrderConfirmationAsync(string toEmail, string userName, DateTime reservedDate, string status, bool isupdate = false)
    {
        try
        {
            string subject = "Order Confirmation - Cucina De Corazon";
            string body = "";
            if (isupdate)
            {
                body = $"Dear {userName},<br/><br/>" +
                          $"Your order scheduled for <strong>{reservedDate:MMMM dd, yyyy}</strong> has been successfully <strong>updated</strong> and is awaiting for confirmation.<br/><br/>" +
                          "Thank you for choosing <strong>Cucina De Corazon!</strong>";
            }
            else if(status == "Pending")
            {
                body = $"Dear {userName},<br/><br/>" +
                          $"Your order scheduled for <strong>{reservedDate:MMMM dd, yyyy}</strong> has been successfully placed and is awaiting for confirmation.<br/><br/>" +
                          "Thank you for <strong>Cucina De Corazon</strong>";
            }
            else
            {
                body = $"Dear {userName},<br/><br/>" +
                       $"Great news! Your order scheduled for <strong>{reservedDate:MMMM dd, yyyy}</strong> has been <strong>confirmed</strong> and is now being prepared.<br/><br/>" +
                       $"Our team will ensure everything is set and ready for your event.<br/><br/>" +
                       $"Thank you for choosing <strong>Cucina De Corazon!</strong> — we look forward to serving you!";
            }

            using (var smtp = new SmtpClient("smtp.gmail.com"))
                {
                    smtp.Port = 587; // or your SMTP port
                    smtp.Credentials = new NetworkCredential("act.cjodato@gmail.com", "lloy qxnz qdmy flef");
                    smtp.EnableSsl = true;

                    var mail = new MailMessage();
                    mail.From = new MailAddress("act.cjodato@gmail.com", "Cucina De Corazon");
                    mail.To.Add(toEmail);
                    mail.Subject = subject;
                    mail.Body = body;
                    mail.IsBodyHtml = true;

                    await smtp.SendMailAsync(mail); // async sending
                }
        }
        catch (Exception ex)
        {
            Console.WriteLine("Email sending failed: " + ex.Message);
            // Optional: log to database or file
        }
    }
}
