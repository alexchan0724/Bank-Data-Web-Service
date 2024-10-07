using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace API_Classes
{
    public class UserDataIntermed
    {
        public UserDataIntermed()
        {
            password = "";
            username = "";
            address = "";
            email = "";
            phoneNum = "";
            profilePicture = null;
            isAdmin = 0;
        }


        public UserDataIntermed(string password, string username, string address, string email, byte[] profilePicture, string phoneNum, int isAdmin)
        {
            this.password = password;
            this.username = username;
            this.address = address;
            this.email = email;
            this.phoneNum = phoneNum;
            this.profilePicture = profilePicture;
            this.isAdmin = isAdmin;
        }


        public string password { get; set; }
        public string username { get; set; }
        public string address { get; set; }
        public string email { get; set; }
        public string phoneNum { get; set; }
        public byte[] profilePicture { get; set; }
        public int isAdmin { get; set; }
    }
}
