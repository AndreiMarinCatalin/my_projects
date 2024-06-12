using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace keymanager_Dev.Clases
{
    public class Credential
    {
        public int Id { get; set; }
        //public User User { get; set; } = new User();
        public int id_user { get; set; }
        public string Name { get; set; }
        public string Link { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string Description { get; set; }
        public DateTime expiration_data { get; set; }

        //public Credential(int id, User user, string name, string link, string username, string password, string description, DateTime expiration_data)
        //{
        //    Id = id;
        //    User = user;
        //    Name = name;
        //    Link = link;
        //    Username = username;
        //    Password = password;
        //    Description = description;
        //    this.expiration_data = expiration_data;
        //}
        public Credential()
        {

        }

        //public Credential(User user, string name, string username, string password, DateTime expiration_data)
        //{
        //    User = user;
        //    Name = name;
        //    Username = username;
        //    Password = password;
        //    this.expiration_data = expiration_data;
        //}

        public Credential(int iduser, string name, string username, string pass, DateTime expiDate) 
        {
            id_user = iduser;
            Name = name;
            Username = username;
            Password = pass;
            expiration_data = expiDate;
        
        }




        public Credential(string name, string link, string username, string password, string description)
        {
            Name = name;
            Link = link;
            Username = username;
            Password = password;
            Description = description;
        }
        public Credential(int id, string name, string link, string username, string password, string description, DateTime expData)
        {
            Id = id;
            Name = name;
            Link = link;
            Username = username;
            Password = password;
            Description = description;
            expiration_data = expData;
        }
    }


}


//CREATE TABLE CREDENTIAL (
//  id_credential INT(4) UNSIGNED AUTO_INCREMENT PRIMARY KEY,
//  id_user INT(4) UNSIGNED,
//  name VARCHAR(50),
//  link VARCHAR(200),
//  username VARCHAR(50),
//  password VARCHAR(50) NOT NULL,
//  description VARCHAR(100),
//  expiration_date DATE,
//  FOREIGN KEY (id_user) REFERENCES USER(id_user) ON DELETE CASCADE ON UPDATE CASCADE
//);