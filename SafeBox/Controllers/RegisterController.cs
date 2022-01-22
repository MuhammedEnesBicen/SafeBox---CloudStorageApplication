using BusinessLayer.Concrete;
using DataAccessLayer.Concrete;
using EntityLayer.Concrete;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SafeBox.Controllers
{
    public class RegisterController : Controller
    {
        FirebaseHelper firebase = new FirebaseHelper();
        static String ValidationCode;
        static User temp;
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
            ValidationCode = es.SendEmail(user.Mail);
            temp = user;
            return RedirectToAction("Validate",new { user=user});
        }


        [HttpGet]
        public IActionResult Validate()
        {
           
            return View();
        }


        [HttpPost]/*When this was HttpPost it throwed error 405*/
        public IActionResult  IsValid(string userInput)
        {
            if (ValidationCode == userInput) {
                firebase.AddUser(temp).Wait();
                return View(true); }
            ViewBag.message = "Code is not valid";
            return View(false);
        }
    }
}
