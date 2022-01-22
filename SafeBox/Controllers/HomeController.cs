using DataAccessLayer.Concrete;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Firebase.Storage;
using Microsoft.AspNetCore.Hosting;

namespace SafeBox.Controllers
{
    public class HomeController : Controller
    {
        FirebaseStorageHelper firebaseStorage = new FirebaseStorageHelper();
        FirebaseHelper firebaseHelper = new FirebaseHelper();

        public IActionResult Index()
        {
            string userMail = HttpContext.Session.GetString("userMail");
            //if (!String.IsNullOrEmpty(userMail)) {
            //    var files = firebaseHelper.GetUserFiles(userMail).Result;
            //    return View(files);
            //}
            var files = firebaseHelper.GetUserFiles(userMail).Result;
            return View(files);

        }



        [HttpPost]
        public async Task<IActionResult> UploadFile(IFormFile file)
        {
            //foreach (var file in files)
            //{



            if (file == null || file.Length == 0)
                return Content("file not selected");

            var path = Path.Combine(
                        Directory.GetCurrentDirectory(), "wwwroot\\files",
                        file.FileName);

            using (var stream = new FileStream(path, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }


            FileStream ms = new FileStream(path, FileMode.Open);
            string userMail = HttpContext.Session.GetString("userMail");
            await firebaseStorage.SendFileToFirebase(ms, file.FileName, file.ContentType, userMail);
            ms.Close();

            FileInfo tempFile = new FileInfo(path);
            if (tempFile.Exists)//check file exsit or not  
            {
                tempFile.Delete();
            }
            //}
            return RedirectToAction("Index");
        }


        [HttpGet]
        public async Task<IActionResult> DeleteFile(string fileName)
        {
            string userMail = HttpContext.Session.GetString("userMail");
            await firebaseStorage.DeleteStorageFile(fileName,userMail);
            return RedirectToAction("Index");
        }
    }
}
