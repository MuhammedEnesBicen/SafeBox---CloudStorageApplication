using DataAccessLayer.Concrete;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;

namespace SafeBox.Controllers
{
    public class HomeController : Controller
    {
        private readonly FirebaseStorageHelper firebaseStorage;
        private readonly FirebaseHelper firebaseHelper;

        public HomeController(FirebaseHelper firebaseHelper, FirebaseStorageHelper firebaseStorage)
        {
            this.firebaseHelper = firebaseHelper;
            this.firebaseStorage = firebaseStorage;
        }

        public IActionResult Index()
        {
            string userMail = HttpContext.Session.GetString("userMail");


            /// To Open Home Page Without Login
            //if (userMail == null)
            //    userMail = "muhammedenesbicen49@gmail.com";//muhammedenesbicen49@gmail.com
            //HttpContext.Session.SetString("userMail", userMail);
            

            if (String.IsNullOrEmpty(userMail))
            {
                return RedirectToAction("Index", "Login");
            }
            var user = firebaseHelper.GetUserWithMail(userMail).Result;

            string userFBkey= firebaseHelper.GetUserFBKey(userMail).Result;

            HttpContext.Session.SetString("userFBkey", userFBkey);
            
            return View(user);
        }
               

    }
}
