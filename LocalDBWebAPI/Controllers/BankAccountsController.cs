using Microsoft.AspNetCore.Mvc;
using LocalDBWebAPI.Data;
using LocalDBWebAPI.Models;
using API_Classes;
using System.Diagnostics;
using System.Security.Principal;

namespace LocalDBWebAPI.Controllers
{
    [Route("account/[controller]")]
    [ApiController]
    public class BankAccountsController : Controller
    {
        [HttpGet("{accountNumber}")]
        public IActionResult Get(int accountNumber)
        {
            var account = DBManager.GetBankAccount(accountNumber);
            DBManager.AddLogEntry("Bank Account" + account.username + "has requested to grab bank account " + account.accountNumber);
            if (account == null)
            {

                DBManager.AddLogEntry("Bank Account" + account.username + "'s request for bank account " + accountNumber + " has failed");
                return NotFound("Account number does not exist.");

            }
            else
            {
                BankDataIntermed returnBankAccount = new BankDataIntermed();
                returnBankAccount.username = account.username;
                returnBankAccount.accountNumber = account.accountNumber;
                returnBankAccount.balance = account.balance;
                returnBankAccount.pin = account.pin;
                returnBankAccount.description = account.description;
                returnBankAccount.email = account.email;

                DBManager.AddLogEntry("Bank Account" + account.username + "'s request for bank account " + accountNumber + " has succeeded");

                return Ok(returnBankAccount);
            }
        }

        [HttpGet]
        public IActionResult Get([FromQuery] string username)
        {
            DBManager.AddLogEntry("bank account search under the name " + username + "commence");
            var accounts = DBManager.GetUserBankAccounts(username);

            if (accounts == null || accounts.Count == 0)
            {
                DBManager.AddLogEntry("bank account search under the name " + username + " has failed");

                return NotFound("No bank accounts found for the user.");

            }
            DBManager.AddLogEntry("bank account search under the name " + username + " has succeeded");

            return Ok(accounts);
        }

        [HttpPost]
        public IActionResult Post([FromBody] BankDataIntermed account)
        {
            DBManager.AddLogEntry("bank account creation commenced");

            if (account == null)
            {
                DBManager.AddLogEntry("bank account creation has failed due to not having any details");

                return BadRequest("Bank account is required.");
            }
            if (account.pin == 0) // Ensure that the user has entered a PIN
            {
                DBManager.AddLogEntry("bank account creation has failed due to not having a PIN");

                return BadRequest("PIN is required.");
            }

            if (DBManager.InsertBankAccount(account))
            {
                DBManager.AddLogEntry("bank account creation has succeeded");

                return Ok("Bank account added.");
            }
            DBManager.AddLogEntry("bank account creation has failed because the selected account number already exist");

            return BadRequest("Bank account with the same account number already exists.");
        }

        [HttpPut]
        public IActionResult Put([FromBody] BankDataIntermed account, [FromQuery]int accountNumber)
        {
            DBManager.AddLogEntry("bank account update commenced");

            if (account == null)
            {
                DBManager.AddLogEntry("bank account update has failed due to not having bank account");

                return BadRequest("Bank account is required.");
            }
            if (account.pin == 0) // Ensure that the user has entered a PIN
            {
                DBManager.AddLogEntry("bank account update has failed due to not having PIN");

                return BadRequest("PIN is required.");
            }

            if (DBManager.UpdateBankAccount(account, accountNumber))
            {
                DBManager.AddLogEntry("bank account update has succeeded");

                return Ok("Bank account updated.");
            }
            DBManager.AddLogEntry("bank account update failed because Account number already exist");
            return NotFound("Account number could not be updated as it already exists."); // Update returns false
        }

        [HttpDelete]
        [Route("{accountNumber}")]
        public IActionResult Delete(int accountNumber)
        {
            DBManager.AddLogEntry("bank account deletion commenced");

            if (DBManager.DeleteBankAccount(accountNumber))
            {
                DBManager.AddLogEntry("bank account deleted successfully");

                return Ok("Bank account deleted.");
            }
            DBManager.AddLogEntry("bank account failed due to not able to find account number");
            return NotFound("Account number does not exist.");
        }
    }
}
