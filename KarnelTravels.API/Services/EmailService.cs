using System.Net;
using System.Net.Mail;
using System.Text;

namespace KarnelTravels.API.Services;

public interface IEmailService
{
    Task<bool> SendEmailAsync(string toEmail, string subject, string body);
    Task<bool> SendBookingConfirmationAsync(string toEmail, string customerName, string bookingCode, 
        string serviceName, DateTime checkIn, DateTime checkOut, decimal totalAmount);
    Task<bool> SendReplyEmailAsync(string toEmail, string customerName, string subject, string replyMessage, string originalMessage);
}

public class EmailService : IEmailService
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<EmailService> _logger;

    public EmailService(IConfiguration configuration, ILogger<EmailService> logger)
    {
        _configuration = configuration;
        _logger = logger;
    }

    public async Task<bool> SendEmailAsync(string toEmail, string subject, string body)
    {
        try
        {
            var smtpHost = _configuration["Email:SmtpHost"] ?? "smtp.gmail.com";
            var smtpPort = int.Parse(_configuration["Email:SmtpPort"] ?? "587");
            var smtpUser = _configuration["Email:Username"] ?? "";
            var smtpPass = _configuration["Email:Password"] ?? "";
            var fromEmail = _configuration["Email:FromEmail"] ?? "noreply@karneltravels.com";
            var fromName = _configuration["Email:FromName"] ?? "Karnel Travels";

            using var client = new SmtpClient(smtpHost, smtpPort)
            {
                EnableSsl = true,
                Credentials = new NetworkCredential(smtpUser, smtpPass),
                Timeout = 30000
            };

            var message = new MailMessage
            {
                From = new MailAddress(fromEmail, fromName),
                Subject = subject,
                Body = body,
                IsBodyHtml = true,
                BodyEncoding = Encoding.UTF8,
                SubjectEncoding = Encoding.UTF8
            };

            message.To.Add(toEmail);

            await client.SendMailAsync(message);
            _logger.LogInformation("Email sent successfully to {Email}", toEmail);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send email to {Email}", toEmail);
            return false;
        }
    }

    public async Task<bool> SendBookingConfirmationAsync(
        string toEmail, 
        string customerName, 
        string bookingCode, 
        string serviceName, 
        DateTime checkIn, 
        DateTime checkOut, 
        decimal totalAmount)
    {
        var subject = $"Booking confirmation - {bookingCode} - Karnel Travels";
        
        var body = $@"
<!DOCTYPE html>
<html>
<head>
    <meta charset='UTF-8'>
    <style>
        body {{ font-family: Arial, sans-serif; line-height: 1.6; color: #333; }}
        .container {{ max-width: 600px; margin: 0 auto; padding: 20px; }}
        .header {{ background: linear-gradient(135deg, #667eea 0%, #764ba2 100%); color: white; padding: 30px; text-align: center; border-radius: 10px 10px 0 0; }}
        .content {{ background: #f9f9f9; padding: 30px; border-radius: 0 0 10px 10px; }}
        .info-table {{ width: 100%; border-collapse: collapse; margin: 20px 0; }}
        .info-table td {{ padding: 12px; border-bottom: 1px solid #ddd; }}
        .info-table td:first-child {{ font-weight: bold; width: 40%; color: #555; }}
        .total {{ font-size: 24px; font-weight: bold; color: #667eea; }}
        .footer {{ text-align: center; margin-top: 20px; color: #888; font-size: 12px; }}
    </style>
</head>
<body>
    <div class='container'>
        <div class='header'>
            <h1>🎉 Booking confirmation</h1>
            <p>Karnel Travels</p>
        </div>
        <div class='content'>
            <p>Xin chào <strong>{customerName}</strong>,</p>
            <p>Thank you for booking at Karnel Travels! Below are the details:</p>
            
            <table class='info-table'>
                <tr>
                    <td>Booking code:</td>
                    <td><strong>{bookingCode}</strong></td>
                </tr>
                <tr>
                    <td>Service:</td>
                    <td>{serviceName}</td>
                </tr>
                <tr>
                    <td>Check-in date:</td>
                    <td>{checkIn:dd/MM/yyyy}</td>
                </tr>
                <tr>
                    <td>Check-out date:</td>
                    <td>{checkOut:dd/MM/yyyy}</td>
                </tr>
                <tr>
                    <td>Total amount:</td>
                    <td class='total'>{totalAmount.ToString("N0")} VND</td>
                </tr>
            </table>
            
                <p><strong>Note:</strong></p>
            <ul>
                <li>Please arrive 30 minutes before the check-in time.</li>
                <li>Please bring your ID card or passport for the check-in process.</li>
                <li>If you need support, contact the hotline: 1900 6677</li>
            </ul>
            
            <p>Have a great vacation!</p>
        </div>
        <div class='footer'>
            <p>© 2026 Karnel Travels. All rights reserved.</p>
            <p>Email: support@karneltravels.com | Hotline: 1900 6677</p>
        </div>
    </div>
</body>
</html>";

        return await SendEmailAsync(toEmail, subject, body);
    }

    public async Task<bool> SendReplyEmailAsync(
        string toEmail, 
        string customerName, 
        string subject, 
        string replyMessage,
        string originalMessage)
    {
        var emailSubject = $"Re: {subject}";
        
        var body = $@"
<!DOCTYPE html>
<html>
<head>
    <meta charset='UTF-8'>
    <style>
        body {{ font-family: Arial, sans-serif; line-height: 1.6; color: #333; }}
        .container {{ max-width: 600px; margin: 0 auto; padding: 20px; }}
        .header {{ background: linear-gradient(135deg, #059669 0%, #10b981 100%); color: white; padding: 20px; text-align: center; border-radius: 10px 10px 0 0; }}
        .content {{ background: #f9f9f9; padding: 25px; border-radius: 0 0 10px 10px; }}
        .original-message {{ background: #e5e7eb; padding: 15px; border-radius: 8px; margin-bottom: 20px; font-size: 14px; }}
        .reply-message {{ background: white; padding: 20px; border-radius: 8px; border-left: 4px solid #10b981; }}
        .footer {{ text-align: center; margin-top: 20px; color: #888; font-size: 12px; }}
    </style>
</head>
<body>
    <div class='container'>
        <div class='header'>
            <h2>📩 Reply from Karnel Travels</h2>
        </div>
        <div class='content'>
            <p>Xin chào <strong>{customerName}</strong>,</p>
            
            <p>Thank you for contacting us. Below is our reply:</p>
            
            <div class='original-message'>
                <strong>Original message:</strong><br/>
                {originalMessage}
            </div>
            
            <div class='reply-message'>
                <strong>Reply:</strong><br/>
                {replyMessage}
            </div>
            
                <p style='margin-top: 20px;'>If you need more information, please contact us again or call the hotline: <strong>1900 6677</strong></p>
            
            <p>Sincerely,<br/>The Karnel Travels team</p>
        </div>
        <div class='footer'>
            <p>© 2026 Karnel Travels. All rights reserved.</p>
            <p>Email: support@karneltravels.com | Hotline: 1900 6677</p>
        </div>
    </div>
</body>
</html>";

        return await SendEmailAsync(toEmail, emailSubject, body);
    }
}
