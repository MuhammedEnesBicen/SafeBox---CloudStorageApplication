using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChidamberAndKemerer
{
   public  class Program
    {
        //static string[] alfabe = 
        //    {"a","b","c","d","e","f","g","h","i","j","k",
        //    "l","m","n","o","p","r","s","t"
        //    ,"u","v","y","z","x","w","q"};

        static string alfabe = "abcdefghijklmnoprstuvyzxwq ";
       
        static void Main(string[] args)
        {
            Console.WriteLine("Bir metin girin");
            string metin_raw = Console.ReadLine();
            string metin = metin_raw.Replace(" ", "");
            Console.WriteLine(metin);

            int metinUzunluk = metin.Length;
            char [] con_metin = metin.ToCharArray();
            char[] con_alf = alfabe.ToCharArray();

           
            int[] hash2=new int[metin.Length];
            for (int i = 0; i < metin.Length; i++)
            {
                int ara = alfabe.IndexOf(con_metin[i]);
                
                int hash1 = ara % 2;
                hash2[i] = (hash1 + ara);
             
               
            }
          foreach(var i in hash2)
            {
                Console.WriteLine(i);
            }
            var rnd = new Random();
            double key1 = rnd.NextDouble();    
            string con_key1 = key1.ToString().Remove(0,2);
            Console.WriteLine(con_key1);


            double key2 = rnd.NextDouble();
            string con_key2 = key2.ToString().Remove(0, 2);
            Console.WriteLine(con_key2);

            double key3 = rnd.NextDouble();
            string con_key3 = key3.ToString().Remove(0, 2);
            Console.WriteLine(con_key3);


            double key4 = rnd.NextDouble();
            string con_key4 = key4.ToString().Remove(0, 2);
            Console.WriteLine(con_key4);

            var ortaklar = con_key1.Intersect(con_key2);
            string ortak1 = "";
            foreach (var i in ortaklar)
            {
                ortak1 += i;
               
            }
           
            ortaklar = ortak1.Intersect(con_key3);
            string ortak2 = "";
            foreach (var i in ortaklar)
            {
                ortak2 += i;

            }
            


            ortaklar = ortak1.Intersect(ortak2);
            string son_ortak = "";
            foreach(var i in ortaklar)
            {
                son_ortak += i;
            }

           
          
            int integer_ortak = int.Parse(son_ortak);
            int[] tut=new int[son_ortak.Length];
            int k = 0;
            while (integer_ortak > 0)
            {
                tut[k] = integer_ortak % 10;
                integer_ortak = integer_ortak / 10;
                k++;
            }

            int enB = tut[0];
            for(int i = 0; i < tut.Length; i++)
            {
                if (enB < tut[i])
                { enB = tut[i]; }
            }
           


            int enK = tut[0];
            for (int i = 0; i < tut.Length; i++)
            {
                if (enK > tut[i])
                { enK = tut[i]; }
            }

            string birlestir=  con_key4[enB].ToString()+ con_key4[enK].ToString();
           
           
            Console.WriteLine(ortak1 + " " + ortak2 + " " + son_ortak);
            Console.WriteLine(enB + " " + enK);
            Console.WriteLine(birlestir);
            int don_birles = Convert.ToInt32(birlestir);
            if (don_birles < 50)
            {
                don_birles += 50;
                birlestir = don_birles.ToString();
            }
           
            //string birlestir = enB.ToString() + enK.ToString(); 
            int integer_birlesir = Convert.ToInt32(birlestir);



            string[] ekle = new string[metin.Length];
            for (int i = 0; i < metin.Length; i++)
            {
                ekle[i] = char.ConvertFromUtf32(hash2[i] + integer_birlesir);
                Console.Write(ekle[i]);
            }



            Console.ReadKey();
        }
    }

   
   


    
   

   
}
