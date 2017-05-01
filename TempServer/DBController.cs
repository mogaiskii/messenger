using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TempServer
{
    class DBController
    {
        public DBController(string server, string port, string login, string password, string table)
        {

        }
        public bool CheckAuth(string username, string passwordHash)
        {
            //TODO: debug
            
            return username.GetHashCode().ToString() == passwordHash;
        }
    }
}
