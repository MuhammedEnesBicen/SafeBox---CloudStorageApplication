using BusinessLayer.Concrete;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.IO;
using System.Threading.Tasks;
using System.Linq;
using DataAccessLayer.Concrete;

namespace SafeBox.Controllers
{
    public class FileController : Controller
    {
        private readonly FirebaseStorageHelper firebaseStorage;
        private readonly FirebaseHelper firebaseHelper;

        public FileController(FirebaseStorageHelper firebaseStorage, FirebaseHelper firebaseHelper)
        {
            this.firebaseStorage = firebaseStorage;
            this.firebaseHelper = firebaseHelper;
        }






        /// <summary>
        /// Encriyption feature is deleted from the project.
        /// Because of that, we dont need to get files from firebase and save them as decrypted files.
        /// This method is not used anymore.
        /// But i dont want to delete it because it can be useful in the future.
        /// </summary>
        private void GetFilesToLocal()
        {
            string userFBKey = HttpContext.Session.GetString("userFBkey");
            string userMail = HttpContext.Session.GetString("userMail");
            var files = firebaseHelper.GetUserFiles(userMail).Result;
            Directory.CreateDirectory(Path.Combine(
Directory.GetCurrentDirectory(), "wwwroot\\files\\" + userFBKey + "\\encrypted"));

            Directory.CreateDirectory(Path.Combine(
Directory.GetCurrentDirectory(), "wwwroot\\files\\" + userFBKey + "\\decrypted"));
            foreach (var item in files)
            {
                using (System.Net.WebClient wc = new System.Net.WebClient())
                {
                    try
                    {

                        var path = Path.Combine(
    Directory.GetCurrentDirectory(), "wwwroot\\files\\" + userFBKey + "\\encrypted",
    item.Name);

                        var decryptPath = Path.Combine(
                                    Directory.GetCurrentDirectory(), "wwwroot\\files\\" + userFBKey + "\\decrypted",
                                    item.Name);
                        wc.DownloadFile(item.DownloadUrl, path);

                        FileEncryptionHelper.FileDecrypt(path, decryptPath);

                    }
                    catch (System.Exception ex)
                    {
                        // check exception object for the error
                    }
                }
            }
        }

        public PartialViewResult GetFilesPartial(string filter)
        {
            string userMail = HttpContext.Session.GetString("userMail");
            if(String.IsNullOrEmpty(userMail))
            {
                TempData["sessionMsg"] = "It seems your session has ended. You need to login again. ";
                return PartialView();
            }
            try
            {
                var files = firebaseHelper.GetUserFiles(userMail).Result;
                var tempList = files;
                if (!String.IsNullOrEmpty(filter))
                    tempList = files.Where(f => f.Name.ToLower().Contains(filter.ToLower())).ToList();

                return PartialView(tempList);
            }
            catch (Exception)
            {
                TempData["message"] = "Something went wrong. Try again later.";
                return PartialView();
            }

        }

        [HttpPost]
        public async Task<IActionResult> UploadFile(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return Content("file not selected");


            string userMail = HttpContext.Session.GetString("userMail");

            string userFBKey = HttpContext.Session.GetString("userFBkey");

            var fs = file.OpenReadStream();
            await firebaseStorage.SendFileToFirebase(fs, file.FileName, file.ContentType, userMail);
            fs.Close();

            return RedirectToAction("Index","Home");
        }

        [HttpGet]
        public async Task<IActionResult> DeleteFile(string fileName)
        {
            string userMail = HttpContext.Session.GetString("userMail");
            await firebaseStorage.DeleteStorageFile(fileName, userMail);

            return RedirectToAction("Index", "Home");
        }
    }
}
