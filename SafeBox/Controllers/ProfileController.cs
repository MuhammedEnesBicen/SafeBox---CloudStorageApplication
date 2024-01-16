using BusinessLayer.Concrete;
using DataAccessLayer.Concrete;
using EntityLayer.Concrete;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using System;

namespace SafeBox.Controllers
{
    public class ProfileController : Controller
    {
        private readonly FirebaseHelper firebaseHelper;

        public ProfileController(FirebaseHelper firebaseHelper)
        {
            this.firebaseHelper = firebaseHelper;
        }

        public PartialViewResult ProfilePartial()
        {
            string userMail = HttpContext.Session.GetString("userMail");

            var temp = firebaseHelper.GetUserWithMail(userMail).Result;

            return PartialView(temp);
        }

        [HttpPost]
        public async Task<ResultModel> ChangeUserInfos(string firstName, string lastName)
        {
            string userMail = HttpContext.Session.GetString("userMail");

            try
            {
                var temp = firebaseHelper.GetUserWithMail(userMail).Result;
                temp.FirstName = firstName;
                temp.LastName = lastName;

                await firebaseHelper.UpdateUser(temp);
            }
            catch (Exception)
            {
                return new ResultModel { Result = false, Message = "Something Went Wrong. sory" };

            }
            return new ResultModel { Result = true, Message = "Your infos changed succesfully." };

        }

        [HttpPost]
        public async Task<ResultModel> ChangePassword(string oldPassword, string newPassword)
        {
            string userMail = HttpContext.Session.GetString("userMail");

            var temp = firebaseHelper.GetUserWithMail(userMail).Result;
            if (temp != null && temp.Mail == "test@test.com")
            {
                return new ResultModel { Result = false, Message = "Test account's passwords cant be changed" };

            }
            if (temp.Password == oldPassword)
            {
                temp.Password = newPassword;
                await firebaseHelper.UpdateUser(temp);
                return new ResultModel { Result = true, Message = "Password changed succesfully." };
            }
            return new ResultModel { Result = false, Message = "The password could not be changed. The password you entered is incorrect! " };
        }

        [HttpPost]
        public async Task<ResultModel> ChangeEmail(string newMail)
        {
            EmailService es = new EmailService();
            var user = firebaseHelper.GetUserWithMail(newMail).Result;
            if (user != null)
                return new ResultModel
                {
                    Result = false,
                    Message = "This email is already using by someone else, you cant take this!"
                };


            try
            {
                string userMail = HttpContext.Session.GetString("userMail");

                //Getting user object from firebase
                var tempUser = await firebaseHelper.GetUserWithMail(userMail);
                tempUser.Mail = newMail; //changing mail info
                await firebaseHelper.UpdateUserMail(userMail, tempUser);
                HttpContext.Session.SetString("userMail", newMail); //changing session mail info
                return new ResultModel { Result = true, Message = "Your email updated succesfully." };
            }
            catch (Exception)
            {
                return new ResultModel { Result = false, Message = "Sory your email didn't update succesfully." };
            }

        }

    }
}
