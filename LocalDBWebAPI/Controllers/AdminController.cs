using API_Classes;
using LocalDBWebAPI.Data;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace LocalDBWebAPI.Controllers
{
    [Route("admin/[controller]")]

    public class AdminController : Controller
    {
        [HttpGet("BySearch/{searchString}")]
        public IActionResult GetUsers(string searchString)
        {
            List<UserDataIntermed> users = DBManager.SearchUsersByName(searchString);
            Debug.WriteLine("Users found: " + users.Count);
            if (users == null || !users.Any())
            {
                DBManager.AddLogEntry("transaction search failed due to not finding any transaction under user " + searchString);
                return NotFound("No transactions have been found in the database.");
            }
            DBManager.AddLogEntry("transaction search successful, returning list of transaction under " + searchString);

            return Ok(users);
        }

        [HttpGet("auditLogs")]
        public IActionResult GetLogs()
        {
            List<LogDataIntermed> logEntries = DBManager.GetAllLogs();
            Debug.WriteLine("Entries found: " + logEntries.Count);
            if (logEntries == null || !logEntries.Any())
            {
                DBManager.AddLogEntry("Admin failed to get all logs");
                return NotFound("No transactions have been found in the database.");
            }
            DBManager.AddLogEntry("Admin succeeded to get all logs");
            return Ok(logEntries);
        }

        [Route("filterTransactions")]
        [HttpPost]
        public IActionResult FilterTransactions([FromBody] FilterTransactionsIntermed filter)
        {
            string username = filter.username;
            int? minAmount = filter.minAmount;
            int? maxAmount = filter.maxAmount;
            bool ascending;
            if (filter.orderType == "ascending")
            {
                ascending = true;
            }
            else
            {
                ascending = false;
            }
            List<TransactionDataIntermed> transactions = DBManager.getTransactionsUsingFilter(username, minAmount, maxAmount, ascending);
            Debug.WriteLine("Transactions found: " + transactions.Count);
            if (transactions == null || !transactions.Any())
            {
                DBManager.AddLogEntry("Filter transactions failed with parameters. username: " + username + "minAmount: " + minAmount + "maxAmount: " + "orderType: " + filter.orderType);
                return NotFound("No transactions have been found in the database.");
            }
            DBManager.AddLogEntry("Filter transactions successful with parameters. username: " + username + "minAmount: " + minAmount + "maxAmount: " + "orderType: " + filter.orderType);
            return Ok(transactions);
        }

    }
}
