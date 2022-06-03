using EntityLayer.Concrete;
using Firebase.Storage;
using System;
using System.IO;
using System.Threading.Tasks;

namespace DataAccessLayer.Concrete
{
    public class FirebaseStorageHelper
    {
        FirebaseStorage firebaseStorage = new FirebaseStorage("safebox-49bc8.appspot.com");// if "gs://" be head of bucket string, it wont work!!!
        FirebaseHelper firebaseHelper = new FirebaseHelper();

        public async Task SendFileToFirebase(Stream file, string fileName, string contentType, string userMail)
        {
            string userFbKey = await firebaseHelper.GetUserFBKey(userMail);
            var imageUrl = await firebaseStorage
            .Child(userFbKey)
            .Child(fileName)
            .PutAsync(file);

            if (!String.IsNullOrEmpty(imageUrl))
            {
                var File = new StorageFileInfo { Name = fileName, DownloadUrl = imageUrl.ToString(), Extension = contentType, FileSize = file.Length / 1000 };
                await firebaseHelper.AddFile(File, userMail);
            }

        }

        public async Task DeleteStorageFile(string fileName, string userMail)
        {
            string userFbKey = await firebaseHelper.GetUserFBKey(userMail);
            await firebaseStorage
 .Child(userFbKey)
 .Child(fileName)
 .DeleteAsync();

            await firebaseHelper.DeleteFile(fileName, userMail);

        }
    }
}
