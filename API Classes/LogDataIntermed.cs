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
            logUsername = "";
        }

        public LogDataIntermed(int logID, string logUsername, string logDescription, DateTime logDate)
        {
            this.logID = logID;
            this.logDate = logDate;
            this.logDescription = logDescription;
            this.logUsername = logUsername;
        }
        public int logID { get; set; }
        public string logDescription { get; set; }
        public string logUsername { get; set; }
        public DateTime logDate { get; set; }
    }
}
