using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.Concrete
{
    public class UserFBKeySingleton
    {
        private static UserFBKeySingleton ClassInstance { get; set; }
        public static UserFBKeySingleton GetInstance()
        {
            if (ClassInstance==null)
            {
                ClassInstance = new UserFBKeySingleton();
            }
            return ClassInstance;
        }

        public  string UserFbKey { get; set; }

    }
}
