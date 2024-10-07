﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace API_Classes
{
    public class TransactionDataIntermed
    {
        public TransactionDataIntermed()
        {
            transactionID = 0;
            acctNo = 0;
            transactionDescription = "";
            amount = 0;
        }
        public TransactionDataIntermed(int acctNo, string transactionDescription, int amount)
        {
            this.transactionID = transactionID;
            this.acctNo = acctNo;
            this.transactionDescription = transactionDescription;
            this.amount = amount;
        }

        public int transactionID { get; set; } 
        public int acctNo { get; set; } 
        public string transactionDescription { get; set; } 
        public int amount { get; set; } 
    }
}
