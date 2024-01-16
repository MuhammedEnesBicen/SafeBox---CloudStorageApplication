using System;
using System.Net.Mail;
using EntityLayer.Concrete;

namespace BusinessLayer.Concrete
{
    /// <summary>
    ///  Due to regulations of Google, this service is not working anymore.
    ///  And for this reason, this service is not used in the project.
    /// </summary>
    public class EmailService
    {
        public ResultModel SendEmail(string email)
        {
            string validationCode = "";

            Random rnd = new Random();
            string mesaj = "";
            string rast = "";
            for (int i = 0; i < 4; i++)
            {
                rast = rnd.Next(10).ToString();
                mesaj += rast;
            }

            try
            {

                string kod = "SafeBox Registration Pass Code:  " + mesaj;
                MailMessage maila = new MailMessage();
                SmtpClient SmtpServer = new SmtpClient("smtp.gmail.com");

                maila.From = new MailAddress("firatabs23@gmail.com");
                maila.To.Add(email);
                maila.Subject = "SafeBox Registration";
                maila.Body = kod;

                SmtpServer.Port = 587;
                SmtpServer.Host = "smtp.gmail.com";
                
                SmtpServer.UseDefaultCredentials = false;
                SmtpServer.Credentials = new System.Net.NetworkCredential("firatabs23@gmail.com", "");// Password of account should added to second parameter
                SmtpServer.EnableSsl = true;
                SmtpServer.TargetName = "STARTTLS/smtp.gmail.com";
                SmtpServer.Send(maila);
                validationCode = mesaj;

            }
            catch (Exception ex)
            {
                return new ResultModel { Result = false, Message = ex.Message };
            }
            return new ResultModel {Result=true,Message= validationCode };
        }
    }
}
