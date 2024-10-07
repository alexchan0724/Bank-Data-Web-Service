using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;

namespace API_Classes
{
    public class BankDataIntermed
    {
        public BankDataIntermed(int acctNo, int pin, int balance, string username, string email, string description)
        {
            this.accountNumber = acctNo;
            this.pin = pin;
            this.balance = balance;
            this.username = username;
            this.email = email;
            this.description = description;
        }
        public BankDataIntermed()
        {
            accountNumber = 0;
            pin = 0;
            balance = 0;
            username = "";
            email = "";
            description = "";
        }

        public int accountNumber { get; set; }
        public int balance { get; set; }
        public int pin { get; set; }
        public string description { get; set; }
        public string username { get; set; }
        public string email { get; set; }
    }


}
