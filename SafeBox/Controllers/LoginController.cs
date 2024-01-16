using DataAccessLayer.Concrete;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SafeBox.Models;
using System;
namespace SafeBox.Controllers
{
    public class LoginController : Controller
    {
        FirebaseHelper firebase = new FirebaseHelper();

        public IActionResult Index()
        {

            String mail = Request.Cookies["userMailCookie"];
            if (mail != null)
            {
                String password = Request.Cookies["userPasswordCookie"];

                LoginDTO user = new LoginDTO { Mail = mail, Password = password ,RememberMe=true};
                return View(user);
            }

            return View();
        }

        [HttpPost]
        public IActionResult Index(LoginDTO credentials)
        {
            CookieOperations(credentials.RememberMe ? "save" : "delete", credentials.Mail, credentials.Password);
            var result = firebase.GetUserWithMail(credentials.Mail).Result;

            if(result is not null && result.Password == credentials.Password)
            {
                
                HttpContext.Session.SetString("userMail", credentials.Mail);
                return RedirectToAction("Index","Home");
            }

            if(result == null) { ViewBag.message = "There is no such a user"; }
            else if(result.Password != credentials.Password) { ViewBag.message = "Password is invalid"; }


            return View(credentials);
        }

        [HttpGet]
        public IActionResult LogOut()
        {
            HttpContext.Session.Remove("userMail");
            return RedirectToAction("Index");
        }
        public void CookieOperations(string operation, string email, string password)
        {

            switch (operation)
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


    }
}
