using System.Net;
using System.Net.Mail;
using Microsoft.Extensions.Configuration;

namespace Repository.Libraries;

public class MailerService
{
    private readonly NetworkCredential credential;
    public MailerService(IConfiguration configuration)
    {
        credential = new(configuration.GetConnectionString("EMAIL_ADDRESS"), configuration.GetConnectionString("EMAIL_PASSWORD"));
    }

    public static string EmailTemplate(string username, string message, string url, string buttonValue)
    {
        return $"<!DOCTYPE html><html><body style='background-color:#faf9f5;font-family:Lato,sans-serif;margin:0;padding:0'><center style='background:#faf9f5;padding:20px 0'><table align='center' border='0' cellpadding='0' cellspacing='0' style='width:600px;background:#fff;margin-top:30px;box-shadow:0 0 5px #ddd'><tbody><tr><td align='center' id='bodyCell' valign='top' style='border-top:0'><table border='0' cellpadding='0' cellspacing='0' width='100%'><tbody><tr><td align='center' id='templatePreheader' valign='top'><table align='center' border='0' cellpadding='0' cellspacing='0' width='100%'><tbody><tr><td style='background:#055;text-align:center;padding:40px 30px'><img alt='Residize' src='https://res.cloudinary.com/drpwjoiik/image/upload/v1732007974/fswyh3o93bstk40tivfn.png' style='border:0;height:auto;outline:0;text-decoration:none'></td></tr></tbody></table></td></tr><tr><td><table border='0' cellpadding='0' cellspacing='0' width='100%'><tbody><tr><td style='padding:30px'><p style='color:#4c4c4a;font-family:Lato,sans-serif;font-weight:700;font-size:16px;margin:10px 0;padding:0'>Hi {username}!</p><p style='color:#7c7c7a;line-height:26px;font-family:Lato,sans-serif;font-size:15px;margin:10px 0;padding:0'>{message}</p></td></tr><tr><td style='padding:30px;font-size:15px;padding-top:0;font-family:Lato,sans-serif'><table border='0' cellpadding='0' cellspacing='0' width='100%'><tbody><tr><td style='width:100%;padding:12px 0;color:#7c7c7a;font-size:16px;font-family:Arial,Helvetica,sans-serif;font-weight:700'>Username :<span style='font-family:Arial,Helvetica,sans-serif;color:#4a88e9;font-size:16px;text-transform:none;text-decoration:none'>{username}</span></td></tr></tbody></table></td></tr><tr><td style='padding:30px;font-size:15px;padding-top:0;font-family:Lato,sans-serif'><a href='{url}' style='color:#fff;font-family:Lato,sans-serif;background-color:#055;box-shadow:0 2px 4px 0 rgba(74,136,233,.5);height:42px;font-size:15px;padding:10px 20px;line-height:1.4;text-align:center;white-space:nowrap;vertical-align:middle;border:none;border-radius:3px;cursor:pointer;text-decoration:none'>{buttonValue}</a></td></tr><tr><td style='padding:30px 30px 0;font-family:Lato,sans-serif;font-size:15px'><p style='color:#7c7c7a;line-height:26px;margin:0;padding:0;font-family:Lato,sans-serif'>Sincerely,</p><p style='color:#4c4c4a;line-height:26px;margin:0;padding:0;font-family:Lato,sans-serif'>Residize Team</p></td></tr><tr><td style='padding:30px 30px 15px'></td></tr></tbody></table></td></tr></tbody></table></td></tr></tbody></table></center></body></html>";
    }

    public void SendEmail(string recipient, string subject, string body)
    {
        SmtpClient smtp = new()
        {
            Host = "smtp.gmail.com",
            Port = 587,
            EnableSsl = true,
            DeliveryMethod = SmtpDeliveryMethod.Network,
            UseDefaultCredentials = false,
            Credentials = credential
        };

        using MailMessage mailMessage = new(credential.UserName, recipient)
        {
            IsBodyHtml = true,
            Subject = subject,
            Body = body
        };
        smtp.Send(mailMessage);
    }

    public static string EmailTemplate2(string username, string email, string phonenumber)
    {
        return $"<!DOCTYPE html><html><body style='background-color:#faf9f5;font-family:Lato,sans-serif;margin:0;padding:0'><center style='background:#faf9f5;padding:20px 0'><table align='center' border='0' cellpadding='0' cellspacing='0' style='width:600px;background:#fff;margin-top:30px;box-shadow:0 0 5px #ddd'><tbody><tr><td align='center' id='bodyCell' valign='top' style='border-top:0'><table border='0' cellpadding='0' cellspacing='0' width='100%'><tbody><tr><td align='center' id='templatePreheader' valign='top'><table align='center' border='0' cellpadding='0' cellspacing='0' width='100%'><tbody><tr><td style='background:#055;text-align:center;padding:40px 30px'><img alt='Residize' src='https://res.cloudinary.com/drpwjoiik/image/upload/v1732007974/fswyh3o93bstk40tivfn.png' style='border:0;height:auto;outline:0;text-decoration:none'></td></tr></tbody></table></td></tr><tr><td><table border='0' cellpadding='0' cellspacing='0' width='100%'><tbody><tr><td style='padding:30px'><p style='color:#4c4c4a;font-family:Lato,sans-serif;font-weight:700;font-size:16px;margin:10px 0;padding:0'></p><p>Your contact information has been requested and shared with:</p><p style='color:#7c7c7a;line-height:26px;font-family:Lato,sans-serif;font-size:15px;margin:10px 0;padding:0'><br>RequesterName: {username}<br>Contact Number: {phonenumber}<br>Email Address: {email}<br></p></td></tr><tr><td style='padding:30px;font-size:15px;padding-top:0;font-family:Lato,sans-serif'><table border='0' cellpadding='0' cellspacing='0' width='100%'><tbody><tr></tr></tbody></table></td></tr><tr><td style='padding:30px;font-size:15px;padding-top:0;font-family:Lato,sans-serif'><p>If you have any questions or concerns, please contact us.</p></td></tr><tr><td style='padding:30px 30px 0;font-family:Lato,sans-serif;font-size:15px'><p style='color:#7c7c7a;line-height:26px;margin:0;padding:0;font-family:Lato,sans-serif'>Sincerely,</p><p style='color:#4c4c4a;line-height:26px;margin:0;padding:0;font-family:Lato,sans-serif'>Residize Team</p></td></tr><tr><td style='padding:30px 30px 15px'></td></tr></tbody></table></td></tr></tbody></table></td></tr></tbody></table></center></body></html>";
    }

    public static string EmailTemplate3(string username, string email, string phonenumber)
    {
        return $"<!DOCTYPE html><html><body style='background-color:#faf9f5;font-family:Lato,sans-serif;margin:0;padding:0'><center style='background:#faf9f5;padding:20px 0'><table align='center' border='0' cellpadding='0' cellspacing='0' style='width:600px;background:#fff;margin-top:30px;box-shadow:0 0 5px #ddd'><tbody><tr><td align='center' id='bodyCell' valign='top' style='border-top:0'><table border='0' cellpadding='0' cellspacing='0' width='100%'><tbody><tr><td align='center' id='templatePreheader' valign='top'><table align='center' border='0' cellpadding='0' cellspacing='0' width='100%'><tbody><tr><td style='background:#055;text-align:center;padding:40px 30px'><img alt='Residize' src='https://res.cloudinary.com/drpwjoiik/image/upload/v1732007974/fswyh3o93bstk40tivfn.png' style='border:0;height:auto;outline:0;text-decoration:none'></td></tr></tbody></table></td></tr><tr><td><table border='0' cellpadding='0' cellspacing='0' width='100%'><tbody><tr><td style='padding:30px'><p style='color:#4c4c4a;font-family:Lato,sans-serif;font-weight:700;font-size:16px;margin:10px 0;padding:0'></p><p>You have requested the contact information for the property:</p><p style='color:#7c7c7a;line-height:26px;font-family:Lato,sans-serif;font-size:15px;margin:10px 0;padding:0'><br>PropertyName: {username}<br>Contact Number: {phonenumber}<br>Email Address: {email}<br></p></td></tr><tr><td style='padding:30px;font-size:15px;padding-top:0;font-family:Lato,sans-serif'><table border='0' cellpadding='0' cellspacing='0' width='100%'><tbody><tr></tr></tbody></table></td></tr><tr><td style='padding:30px;font-size:15px;padding-top:0;font-family:Lato,sans-serif'><p>If you need further assistance, feel free to reach out to us.</p></td></tr><tr><td style='padding:30px 30px 0;font-family:Lato,sans-serif;font-size:15px'><p style='color:#7c7c7a;line-height:26px;margin:0;padding:0;font-family:Lato,sans-serif'>Sincerely,</p><p style='color:#4c4c4a;line-height:26px;margin:0;padding:0;font-family:Lato,sans-serif'>Residize Team</p></td></tr><tr><td style='padding:30px 30px 15px'></td></tr></tbody></table></td></tr></tbody></table></td></tr></tbody></table></center></body></html>";
    }
}