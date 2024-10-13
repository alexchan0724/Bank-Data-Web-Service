using System;
using System.Collections.Generic;

namespace API_Classes
{
    public class LogDataIntermed
    {
        public LogDataIntermed() 
        {
            logID = 0;
            logDate = DateTime.Now;
            logDescription = "";
        }

        public LogDataIntermed(int logID, string logDescription, DateTime logDate)
        {
            this.logID = logID;
            this.logDate = logDate;
            this.logDescription = logDescription;
        }
        public int logID { get; set; }
        public string logDescription { get; set; }
        public DateTime logDate { get; set; }
    }
}
