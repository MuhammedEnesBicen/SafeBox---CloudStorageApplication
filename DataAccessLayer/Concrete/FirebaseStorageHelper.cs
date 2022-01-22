using EntityLayer.Concrete;
using Firebase.Storage;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Concrete
{
    public class FirebaseStorageHelper
    {
        FirebaseStorage firebaseStorage = new FirebaseStorage("safebox-cd51f.appspot.com");// if "gs://" be head of bucket string, it wont work!!!
        FirebaseHelper firebaseHelper = new FirebaseHelper();
        public async Task SendFileToFirebase(Stream file, string fileName, string contentType, string userMail)
        {
            string userFbKey = await firebaseHelper.GetUserFBKey(userMail);
            var imageUrl = await firebaseStorage
            .Child("-Ms11qwgtxC1vDIkHbZq")
            .Child(fileName)
            .PutAsync(file);

            if (!String.IsNullOrEmpty(imageUrl))
            {
                var File = new StorageFileInfo { Name = fileName, DownloadUrl = imageUrl.ToString(), Extension = contentType };
                await firebaseHelper.AddFile(File);
            }

        }

        public async Task DeleteStorageFile(string fileName, string userMail)
        {
            string userFbKey = await firebaseHelper.GetUserFBKey(userMail);
            await firebaseStorage
 .Child("-Ms11qwgtxC1vDIkHbZq")
 .Child(fileName)
 .DeleteAsync();

            await firebaseHelper.DeleteFile(fileName, userMail);

        }

    }
}
