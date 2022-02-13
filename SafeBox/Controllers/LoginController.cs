using BusinessLayer.Concrete;
using DataAccessLayer.Concrete;
using EntityLayer.Concrete;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SafeBox.Controllers
{
    public class LoginController : Controller
    {
        FirebaseHelper firebase = new FirebaseHelper();
        static bool rememberMe = false; //is user click remember me checkbox?
        static User user; // if user had wanted be remembered, this object will be initialized

        public IActionResult Index()
        {
            String mail = Request.Cookies["userMailCookie"];
            if (mail != null)
            {
                String password = Request.Cookies["userPasswordCookie"];
                rememberMe = true;
                ViewBag.rememberMe = rememberMe;
                user = new User { Mail = mail, Password = password };
                return View(user);
            }

            ViewBag.rememberMe = rememberMe;
            return View();
        }

        [HttpPost]
        public IActionResult Index(User postUser)
        {
            ViewBag.rememberMe = rememberMe;
            var result = firebase.GetUserWithMail(postUser.Mail).Result;

            if(result!= null && result.Password == postUser.Password)
            {
                if (rememberMe) CookieOperations("save", postUser.Mail.ToString(), postUser.Password.ToString());
                if (!rememberMe && user != null) CookieOperations("delete", "", "");

                HttpContext.Session.SetString("userMail", postUser.Mail);
                return RedirectToAction("Index","Home");
            }

            if(result == null) { ViewBag.message = "There is no such a user"; }
            else if(result.Password != postUser.Password) { ViewBag.message = "Password is invalid"; }


            return View(postUser);
        }


        public void CookieOperations(string job, string email, string password)
        {
            switch (job)
            {
                case "save":
                    CookieOptions cookie = new CookieOptions();
                    cookie.Expires = DateTime.Now.AddMonths(1);
                    Response.Cookies.Append("userMailCookie", email, cookie);
                    Response.Cookies.Append("userPasswordCookie", password, cookie);
                    break;
                case "delete":
                    Response.Cookies.Delete("userMailCookie");
                    Response.Cookies.Delete("userPasswordCookie");
                    break;
                default:
                    break;
            }
        }

        [HttpPost]
        public void ChangeRememberMeOption(bool value)
        {
            rememberMe = value;
        }
    }
}
