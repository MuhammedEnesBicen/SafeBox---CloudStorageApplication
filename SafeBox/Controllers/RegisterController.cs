using BusinessLayer.Concrete;
using DataAccessLayer.Concrete;
using EntityLayer.Concrete;
using Microsoft.AspNetCore.Mvc;

namespace SafeBox.Controllers
{
    public class RegisterController : Controller
    {
        FirebaseHelper firebase = new FirebaseHelper();

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Index(User user)
        {
            EmailService es = new EmailService();

            User result = firebase.GetUserWithMail(user.Mail).Result;
            if (result != null)
            {
                
                ViewBag.message = "This email already in use!";
                return View(user);
            }
            /*Because of gmail doesnt support less secure apps feature, our email service dont work anymore
             but i want to keep this code for my future usecases.
             */

            //var resultSendEmail = es.SendEmail(user.Mail);
            //if (resultSendEmail.Result)
            //{
            //    ValidationCode = resultSendEmail.Message;
            //    temp = user;
            //    return RedirectToAction("Validate", new { user = user });
            //}
            //else
            //{
            //    ViewBag.message = "Something went wrong when validation code was sending!" +
            //        "The exception is: "+resultSendEmail.Message;
            //    return View(user);
            //}

            try
            {
                firebase.AddUser(user).Wait();
                TempData["newUserMsg"] = "Your account created succesfully! Now you can login with your credentials";
                return RedirectToAction("Index","Login");
            }
            catch (System.Exception)
            {
                @ViewBag.message = "Something went wrong when account was creating!";
                return View(user);
            }
            

        }


        [HttpGet]
        public IActionResult Validate()
        {
           
            return View();
        }


        [HttpPost]
        public IActionResult  IsValid(string userInput)
        {
            //if (ValidationCode == userInput) {
            //    firebase.AddUser(temp).Wait();
            //    return View(true); }
            ViewBag.message = "Code is not valid";
            return View(false);
        }
    }
}
