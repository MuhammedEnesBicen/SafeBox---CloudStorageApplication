using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EntityLayer.Concrete
{
    public class StorageFileInfo
    {
        public string Name { get; set; }
        public string DownloadUrl { get; set; } // this refer to firebase url(files in here are encryted)
        public string LocalDownloadUrl { get; set; } // this refer to wwwroot\\files folder (files in here are decryted)
        public string Extension { get; set; }
        public float FileSize { get; set; }
    }
}
