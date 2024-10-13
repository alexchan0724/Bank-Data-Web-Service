namespace API_Classes
{
    public class FilterTransactionsIntermed
    {
        public FilterTransactionsIntermed()
        {
            this.minAmount = 0;
            this.maxAmount = 0;
            this.orderType = "";
            this.username = "";
            this.adminUsername = "";
        }

        public FilterTransactionsIntermed(int minAmount, int maxAmount, string transactionType, string username, string adminUsername)
        {
            this.minAmount = minAmount;
            this.maxAmount = maxAmount;
            this.orderType = transactionType;
            this.username = username;
            this.adminUsername = adminUsername;
        }

        public int? minAmount { get; set; }
        public int? maxAmount { get; set; }
        public string orderType { get; set; }
        public string username { get; set; }
        public string adminUsername { get; set; }
    }
}
