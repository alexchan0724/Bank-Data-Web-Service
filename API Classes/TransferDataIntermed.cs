using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace API_Classes
{
    public class TransferDataIntermed
    {
        public TransactionDataIntermed senderTransaction { get; set; }
        public TransactionDataIntermed receiverTransaction { get; set; }
        public string senderUsername { get; set; }
    }
}
