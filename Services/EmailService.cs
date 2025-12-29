using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Mail;
using Supabase;

public static class EmailService
{
    public static void SendOtp(string toEmail, string otpCode, string username)
    {
        try
        {
            var fromAddress = new MailAddress("blackmeettetris@gmail.com", "Tetris Game");
            var toAddress = new MailAddress(toEmail);
            const string fromPassword = "lhxj ftqf pdzt pzxm";
            string subject = "Tetris OTP Code";
            string body = $"Hello {username},\n\nYour OTP code is: {otpCode}\nThe code is valid for 5 minutes.\n\nGood luck!";
            var smtp = new SmtpClient
            {
                Host = "smtp.gmail.com",
                Port = 587,
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(fromAddress.Address, fromPassword)
            };
            using (var message = new MailMessage(fromAddress, toAddress)
            {
                Subject = subject,
                Body = body
            })
            {
                smtp.Send(message);
            }
        }
        catch (Exception ex)
        {
            throw new Exception("Unable to send email: " + ex.Message);
        }
    }
}