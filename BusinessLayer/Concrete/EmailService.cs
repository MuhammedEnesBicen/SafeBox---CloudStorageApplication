using System;
using System.Net.Mail;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntityLayer.Concrete;

namespace BusinessLayer.Concrete
{
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
                SmtpServer.EnableSsl = true;
                SmtpServer.UseDefaultCredentials = false;
                SmtpServer.Credentials = new System.Net.NetworkCredential("firatabs23@gmail.com", "Enes125@");

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
