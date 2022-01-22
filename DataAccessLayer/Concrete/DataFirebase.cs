using EntityLayer.Concrete;
using Firebase.Database;
using Firebase.Database.Query;
//using Firebase.Storage;  Warning
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Firebase.Database;
using Firebase.Database.Query;
//using Firebase.Storage;   ***warning
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Concrete
{
	public class DataFirebase
	{

		FirebaseClient fbClient;
		public DataFirebase()
		{
			fbClient = new FirebaseClient("https://safebox-cd51f-default-rtdb.europe-west1.firebasedatabase.app/");
		}
		private string GetLocalAddress()
		{
			var IpAddress = Dns.GetHostAddresses(Dns.GetHostName()).FirstOrDefault();

			if (IpAddress != null)
				return "1_118_137_162";//IpAddress.ToString();

			return "Could not locate IP Address";
		}

		public async Task<List<DataUser>> getUserList()
		{
			string ip = GetLocalAddress().Replace(".", "_");
			return (await fbClient
				.Child("1_118_137_162")
				.OnceAsync<DataUser>())
				.Select((item) =>
				new DataUser
				{
					KeyDeger = item.Object.KeyDeger,
					Key = item.Key,
					Posta = item.Object.Posta,
					AdSoyad = item.Object.AdSoyad,
					//KullanıcıAdı = item.Object.KullanıcıAdı,
					DogumTarih = item.Object.DogumTarih,
					Sifre = item.Object.Sifre,
					DogKodu = item.Object.DogKodu,

				}).ToList();
		}
		public async Task saveUse(DataUser du)
		{
			var ip = "Users";// GetLocalAddress().Replace(".", "_");

			await fbClient.Child(ip)
					.PostAsync(du);

		}

		public async Task<DataUser> GetLook()
		{
			var ip = GetLocalAddress().Replace(".", "_");

			var allUsers = await getUserList();
			await fbClient
			  .Child(ip)
			  .OnceAsync<DataUser>();
			return allUsers.FirstOrDefault();
		}

		public async Task DeleteUser()
		{
			var ip = GetLocalAddress().Replace(".", "_");
			var delete = (await fbClient.Child(ip).OnceAsync<DataUser>())

			   .FirstOrDefault();

			await fbClient.Child(ip).Child(delete.Key).DeleteAsync();
			//await fbClient.Child("Asama1/"+ "delete.Key").Child(delete.Key).DeleteAsync();

		}

		//public async Task UpdateUser(int keyDeger, string gsoru, string gcevap)
		//{
		//	var ip = GetLocalAddress().Replace(".", "_");
		//	var toUpdateUser = (await fbClient
		//	  .Child(ip)
		//	  .OnceAsync<DataUser>()).Where(a => a.Object.KeyDeger == keyDeger).FirstOrDefault();

		//	await fbClient
		//	  .Child(ip)
		//	  .Child(toUpdateUser.Key)
		//	  .PutAsync(new DataUser() { KeyDeger = keyDeger, Gsoru = gsoru, Gcevap = gcevap });
		//}


		//public async Task SaveUserRequest(Stream imgStream1, Stream imgStream2, StorageUser req)
		//{
		//	var ip = GetLocalAddress().Replace(".", "_");

		//	var postData = await fbClient.Child(ip).PostAsync<StorageUser>(req);
		//	var postData2 = await fbClient.Child(ip).PostAsync<StorageUser>(req);

		//	var imgUrl1 = await new FirebaseStorage("steptwo-db334.appspot.com")

		//		.Child(ip)
		//		.Child(postData.Key)
		//		.PutAsync(imgStream1);

		//	var imgUrl2 = await new FirebaseStorage("steptwo-db334.appspot.com")

		//		.Child(ip)
		//		.Child(postData2.Key)
		//		.PutAsync(imgStream2);

		//	req.ImageUrl1 = imgUrl1;
		//	req.ImageUrl2 = imgUrl2;

		//	var updateData1 = fbClient.Child(ip + "/" + postData.Key + postData2.Key)
		//						   .PutAsync<StorageUser>(req);

		//	//---

		//	//var imgUrl2 = await new FirebaseStorage("steptwo-db334.appspot.com")

		//	//	.Child("Asama1")
		//	//	.Child(postData.Key)
		//	//	.PutAsync(imgStream2);

		//	//req.ImageUrl2 = imgUrl2;

		//	//var updateData2 = fbClient.Child("Asama1" + "/" + postData.Key)
		//	//					   .PutAsync<StorageUser>(req);
		//}

		//public async Task<List<StorageUser>> GetUsers()
		//{
		//	var ip = GetLocalAddress().Replace(".", "_");
		//	var list1 = (await fbClient.Child(ip).OnceAsync<StorageUser>()).Select(item =>
		//	new StorageUser
		//	{
		//		ImageUrl1 = item.Object.ImageUrl1,
		//		ImageUrl2 = item.Object.ImageUrl2,
		//		//Idda = item.Object.Idda,
		//		//Tarih = item.Object.Tarih

		//	}).ToList();

		//	return list1;
		//}

	}
}
