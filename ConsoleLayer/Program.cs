using DataAccessLayer.Concrete;
using EntityLayer.Concrete;
using System;

namespace ConsoleLayer
{
    class Program
    {
        static void Main(string[] args)
        {
            FirebaseHelper fb = new FirebaseHelper();
            FirebaseStorageHelper fbStorage = new FirebaseStorageHelper();
            User temp = new User
            {
                UserId = 1,
                FirstName = "Enes",
                LastName = "Biçen",
                Mail = "muhammedenesbicen49@gmail.com",
                Password = "1234"
            };

            //fb.AddUser(temp).Wait();
            //Console.WriteLine(fb.GetUser(1).Result.FirstName);

            //temp.LastName = "Updated User Lastname";
            //fb.UpdateUser(temp).Wait();

            //temp.Mail = "blabla@tera.com"; temp.UserId = 2;
            //fb.AddUser(temp).Wait();
            //fb.DeleteUser(2).Wait();

            //var list =  fb.GetAllFiles().Result;
            //foreach (var item in list)
            //{
            //    Console.WriteLine(item.Name+" "+item.FolderPath);
            //}
            fbStorage.GetUserFiles().Wait();
        }
    }
}
