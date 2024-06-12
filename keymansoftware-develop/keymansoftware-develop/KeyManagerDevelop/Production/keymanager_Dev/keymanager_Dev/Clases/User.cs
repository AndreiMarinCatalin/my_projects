using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace keymanager_Dev.Clases
{
    public class User
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string MasterKey { get; set; }

        public User(string username, string email, string masterKey)
        {
            Username = username;
            Email = email;
            MasterKey = masterKey;
        }

        public User(string username)
        {
            Username = username;
        }

        public User()
        {
        }
    }
}
