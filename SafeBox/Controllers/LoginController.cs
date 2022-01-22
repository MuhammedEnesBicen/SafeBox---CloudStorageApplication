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
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Index(User user)
        {
            var result = firebase.GetUserWithMail(user.Mail).Result;

            if(result!= null && result.Password == user.Password)
            {
                HttpContext.Session.SetString("userMail", user.Mail);
                return RedirectToAction("Index","Home");
            }

            if(result == null) { ViewBag.message = "There is no such a user"; }
            else if(result.Password != user.Password) { ViewBag.message = "Password is invalid"; }

            return View();
        }
    }
}
