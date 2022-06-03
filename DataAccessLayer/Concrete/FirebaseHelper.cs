using EntityLayer.Concrete;
using Firebase.Database;
using Firebase.Database.Query;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DataAccessLayer.Concrete
{
    public class FirebaseHelper
    {
        FirebaseClient firebase = new FirebaseClient("https://safebox-49bc8-default-rtdb.europe-west1.firebasedatabase.app/");

        public async Task<List<User>> GetAllUsers()
        {
            return (await firebase
              .Child("Users")
              .OnceAsync<User>()).Select(item => new User
              {
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
              .PostAsync(user);
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
            string key = (await firebase
                .Child("Users")
                .OnceAsync<User>()).Where(a => a.Object.Mail == mail).FirstOrDefault()?.Key;

            return key;
        }

        public async Task UpdateUser(User user)
        {
            var toUpdateUser = (await firebase
              .Child("Users")
              .OnceAsync<User>()).Where(a => a.Object.Mail == user.Mail).FirstOrDefault();

            await firebase
              .Child("Users")
              .Child(toUpdateUser.Key)
              .PutAsync(user);
        }

        public async Task UpdateUserMail(string oldMail,User user)
        {
            var toUpdateUser = (await firebase
              .Child("Users")
              .OnceAsync<User>()).Where(a => a.Object.Mail == oldMail).FirstOrDefault();

            await firebase
              .Child("Users")
              .Child(toUpdateUser.Key)
              .PutAsync(user);
        }
        public async Task DeleteUser(string userMail)
        {
            var toDeleteUser = (await firebase
              .Child("Users")
              .OnceAsync<User>()).Where(a => a.Object.Mail == userMail).FirstOrDefault();
            await firebase.Child("Users").Child(toDeleteUser.Key).DeleteAsync();

        }

        #region FileOperations

        public async Task<List<StorageFileInfo>> GetUserFiles(string userMail)
        {
            string userFbKey = await GetUserFBKey(userMail);

            return (await firebase
              .Child("Files")
              .Child(userFbKey)
              .OnceAsync<StorageFileInfo>()).Select(item => new StorageFileInfo
              {
                  Name = item.Object.Name,
                  DownloadUrl = item.Object.DownloadUrl,
                  LocalDownloadUrl = "files/" + userFbKey + "/decrypted/" +
                        item.Object.Name,
                  Extension = item.Object.Extension,
                  FileSize = item.Object.FileSize

              }).ToList();
        }

        public async Task AddFile(StorageFileInfo file, string userMail)
        {
            string userFbKey = await GetUserFBKey(userMail);

            await firebase
                            .Child("Files")
              .Child(userFbKey)
              .PostAsync(file);

            //changing used total space by user
            var user = GetUserWithMail(userMail).Result;
            user.TotalSpaceUsed += file.FileSize / 1000;
            await UpdateUser(user);
        }

        public async Task DeleteFile(string fileName, string userMail)
        {
            string userFbKey = await GetUserFBKey(userMail);

            var toDeleteFile = (await firebase
  .Child("Files")
  .Child(userFbKey)
  .OnceAsync<StorageFileInfo>()).Where(a => a.Object.Name == fileName).FirstOrDefault();
            await firebase.Child("Files").Child(userFbKey)
                .Child(toDeleteFile.Key).DeleteAsync();

            //changing total space used by user
            var user = GetUserWithMail(userMail).Result;
            user.TotalSpaceUsed -= toDeleteFile.Object.FileSize / 1024;
            await UpdateUser(user);

        }

        #endregion
    }
}
