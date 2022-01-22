using EntityLayer.Concrete;
using Firebase.Database;
using Firebase.Database.Query;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Concrete
{
    public class FirebaseHelper
    {
        FirebaseClient firebase;
        public FirebaseHelper()
        {
            firebase = new FirebaseClient("https://safebox-cd51f-default-rtdb.europe-west1.firebasedatabase.app/");
        }

        public async Task<List<User>> GetAllUsers()
        {

            return (await firebase
              .Child("Users")
              .OnceAsync<User>()).Select(item => new User
              {
                  UserId = item.Object.UserId,
                  FirstName = item.Object.FirstName,
                  LastName = item.Object.LastName,
                  Mail = item.Object.Mail,
                  Password = item.Object.Password
              }).ToList();
        }

        public async Task AddUser(User user)
        {

            await firebase
              .Child("Users")
              .PostAsync(user); //new User() { UserId = personId, Name = name }
        }

        public async Task<User> GetUser(int userId)
        {
            User user = (await firebase
              .Child("Users")
              .OnceAsync<User>()).Where(a => a.Object.UserId == userId).FirstOrDefault().Object;

            return user;

        }

        public async Task<User> GetUserWithMail(string mail)
        {
            User user = (await firebase
              .Child("Users")
              .OnceAsync<User>()).Where(a => a.Object.Mail == mail).FirstOrDefault()?.Object;

            return user;
        }

        //brings users firebase key
        public async Task<string> GetUserFBKey(string mail)
        {
            string key= (await firebase
  .Child("Users")
  .OnceAsync<User>()).Where(a => a.Object.Mail == mail).FirstOrDefault()?.Key;

            return key;
        }

        public async Task UpdateUser(User user)
        {
            var toUpdateUser = (await firebase
              .Child("Users")
              .OnceAsync<User>()).Where(a => a.Object.UserId == user.UserId).FirstOrDefault();

            await firebase
              .Child("Users")
              .Child(toUpdateUser.Key)
              .PutAsync(user); //new User() { UserId = personId, Name = name }
        }

        public async Task DeleteUser(int userId)
        {
            var toDeleteUser = (await firebase
              .Child("Users")
              .OnceAsync<User>()).Where(a => a.Object.UserId == userId).FirstOrDefault();
            await firebase.Child("Users").Child(toDeleteUser.Key).DeleteAsync();

        }


        public async Task<List<StorageFileInfo>> GetUserFiles(string userMail)
        {
            string userFbKey = await GetUserFBKey(userMail);
            /*content types
             - file.ContentType = "text/plain"
             */
            // await AddFile();

            return (await firebase
              .Child("Files")
              .Child("-Ms11qwgtxC1vDIkHbZq")
              .OnceAsync<StorageFileInfo>()).Select(item => new StorageFileInfo
              {
                  Name = item.Object.Name,
                  DownloadUrl = item.Object.DownloadUrl,
                  Extension = item.Object.Extension

              }).ToList();
        }

        public async Task AddFile(StorageFileInfo file)
        {
            await firebase
                            .Child("Files")
              .Child("-Ms11qwgtxC1vDIkHbZq")
              .PostAsync(file); //new User() { UserId = personId, Name = name }
        }

        public async Task DeleteFile(string fileName, string userMail)
        {
            var toDeleteFile = (await firebase
  .Child("Files")
  .Child("-Ms11qwgtxC1vDIkHbZq")
  .OnceAsync<StorageFileInfo>()).Where(a => a.Object.Name == fileName).FirstOrDefault();
            await firebase.Child("Files").Child("-Ms11qwgtxC1vDIkHbZq")
                .Child(toDeleteFile.Key).DeleteAsync();
        }
    }
}
