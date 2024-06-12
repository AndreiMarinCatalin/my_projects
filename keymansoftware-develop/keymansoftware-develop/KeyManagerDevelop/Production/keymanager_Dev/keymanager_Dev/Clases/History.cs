using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace keymanager_Dev.Clases
{
    public class History
    {
        public int Id { get; set; }
        public int id_user { get; set; }
        public string ExpiredPassword { get; set; }

        public History(int id, int idUser, string expiredPassword)
        {
            Id = id;
            id_user = idUser;
            ExpiredPassword = expiredPassword;
        }

        public History() { }
    }
}


//CREATE TABLE HISTORY (
//  id_history INT(4) UNSIGNED AUTO_INCREMENT PRIMARY KEY,
//  id_user INT(4) UNSIGNED,
//  expired_password VARCHAR(50) NOT NULL,
//  FOREIGN KEY (id_user) REFERENCES USER(id_user) ON DELETE CASCADE ON UPDATE CASCADE
//);
