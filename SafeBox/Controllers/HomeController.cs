using DataAccessLayer.Concrete;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using BusinessLayer.Concrete;
using EntityLayer.Concrete;

namespace SafeBox.Controllers
{
    public class HomeController : Controller
    {
        private readonly FirebaseStorageHelper firebaseStorage = new FirebaseStorageHelper();
        private readonly FirebaseHelper firebaseHelper = new FirebaseHelper();
        private static List<EntityLayer.Concrete.StorageFileInfo> files =
            new List<EntityLayer.Concrete.StorageFileInfo>();
        static string userMail;
        static string userFBKey;
        static string ValidationCode;


        // TODO: File local işlemlerini yap

        public IActionResult Index()
        {
            string tempMail = HttpContext.Session.GetString("userMail");

            ///* Delete these lines*/
            //if(tempMail==null) tempMail = "muhammedenesbicen49@gmail.com";
            //HttpContext.Session.SetString("userMail", tempMail);
            //userMail = tempMail;



            //if user logged in with second account, this control statement removes old files from memory
            if (tempMail != userMail)
            {
                files.Clear();
            }
            userMail = tempMail;
            if (String.IsNullOrEmpty(userMail))
            {
                return RedirectToAction("Index", "Login");
            }
            var user = firebaseHelper.GetUserWithMail(userMail).Result;
            ViewBag.user = user;
            userFBKey = firebaseHelper.GetUserFBKey(userMail).Result;


            return View(files);

        }

        public async Task InitOperations()
        {


            //for not bringing all files again at upload & delete operations
            if (files.Count > 0)
                return;



            files =firebaseHelper.GetUserFiles(userMail).Result;
            GetFilesToLocal(); // It brings encrypted files from firebase and saves them as decrypted files



        }

        private static void GetFilesToLocal()
        {
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
            var tempList = files;
            if (!String.IsNullOrEmpty(filter))
                tempList = files.Where(f => f.Name.ToLower().Contains(filter.ToLower())).ToList();

            return PartialView(tempList);
        }




        [HttpPost]
        public async Task<IActionResult> UploadFile(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return Content("file not selected");

            var path = Path.Combine(
                        Directory.GetCurrentDirectory(), "wwwroot\\files\\" + userFBKey + "\\decrypted",
                        file.FileName);

            using (var stream = new FileStream(path, FileMode.Create))
            {
                await file.CopyToAsync(stream);
                string localUrl = "files/" + userFBKey + "/decrypted/" + file.FileName;
                var File = new StorageFileInfo { Name = file.FileName, LocalDownloadUrl = localUrl, Extension = file.ContentType, FileSize = file.Length / 1000 };

                int index = files.FindIndex(f => f.Name == File.Name); //is there a file that has same name with new uploaded file?
                if (index < 0)
                    files.Add(File);
                else
                    files[index] = File;
            }

            var encryptedPath = Path.Combine(
                        Directory.GetCurrentDirectory(), "wwwroot\\files\\" + userFBKey + "\\encrypted",
                        file.FileName);

            FileEncryptionHelper.FileEncrypt(path, encryptedPath);

            FileStream ms = new FileStream(encryptedPath, FileMode.Open);
            string userMail = HttpContext.Session.GetString("userMail");
            await firebaseStorage.SendFileToFirebase(ms, file.FileName, file.ContentType, userMail);
            ms.Close();


            return RedirectToAction("Index");
        }


        [HttpGet]
        public async Task<IActionResult> DeleteFile(string fileName)
        {
            //TODO: delete files from local
            //we dont delete file from local because when session closed all files will be already deleted
            //actually i couldn't find a way to do that. files are not deleting automatically. 
            string userMail = HttpContext.Session.GetString("userMail");
            await firebaseStorage.DeleteStorageFile(fileName, userMail);

            int index = files.FindIndex(f => f.Name == fileName); //finding file's index in List
            if (index >= 0)
                files.RemoveAt(index);


            return RedirectToAction("Index");
        }




        #region ProfileProcesses

        public PartialViewResult ProfilePartial()
        {
            userMail = HttpContext.Session.GetString("userMail");
            var temp = firebaseHelper.GetUserWithMail(userMail).Result;

            return PartialView(temp);
        }


        [HttpPost]
        public async Task<ResultModel> ChangeUserInfos(string firstName, string lastName)
        {
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
            var temp = firebaseHelper.GetUserWithMail(userMail).Result;
            if (temp.Password == oldPassword)
            {
                temp.Password = newPassword;
                await firebaseHelper.UpdateUser(temp);
                return new ResultModel { Result = true, Message = "Password changed succesfully." };
            }
            return new ResultModel { Result = false, Message = "The password could not be changed. The password you entered is incorrect! " };
        }

        [HttpPost]
        public async Task<ResultModel> ChangeEmail(string newMail, string? valCode, string whichOperation)
        {
            EmailService es = new EmailService();
            var user = firebaseHelper.GetUserWithMail(newMail).Result;
            if (user != null)
                return new ResultModel
                {
                    Result = false,
                    Message = "This email is already using by someone else, you cant take this!"
                };

            if (whichOperation == "newEmailInput")
            {
                var result = es.SendEmail(newMail);
                ValidationCode = result.Result ? result.Message : null;
                if (!String.IsNullOrEmpty(ValidationCode))
                {
                    HttpContext.Session.SetString("userMail", newMail);
                    return new ResultModel
                    { Result = true, Message = "Validation Code sent to your email, check it." };
                }
                else
                    return new ResultModel { Result = false, Message = "Validation code couldnt sent mail which you entered!" };

            }
            else
            {
                if (valCode.Equals(ValidationCode))
                {
                    //Getting user object from firebase
                    var tempUser = await firebaseHelper.GetUserWithMail(userMail);
                    tempUser.Mail = newMail; //changing mail info
                    await firebaseHelper.UpdateUserMail(userMail,tempUser);
                    userMail = newMail; // assignment new value of static "userMail" variable
                    return new ResultModel { Result = true, Message = "Your email updated succesfully." };
                }
                else
                {
                    return new ResultModel { Result = false, Message = "Sory your email wasn't update succesfully." };

                }

            }
        }
        #endregion

    }
}
