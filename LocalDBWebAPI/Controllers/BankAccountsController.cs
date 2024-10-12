using Microsoft.AspNetCore.Mvc;
using LocalDBWebAPI.Data;
using LocalDBWebAPI.Models;
using API_Classes;
using System.Diagnostics;

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

            if (account == null)
            {
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
                return Ok(returnBankAccount);
            }
        }

        [HttpGet]
        public IActionResult Get([FromQuery] string username)
        {
            var accounts = DBManager.GetUserBankAccounts(username);

            if (accounts == null || accounts.Count == 0)
            {
                return NotFound("No bank accounts found for the user.");
            }
            return Ok(accounts);
        }

        [HttpPost]
        public IActionResult Post([FromBody] BankDataIntermed account)
        {
            if (account == null)
            {
                return BadRequest("Bank account is required.");
            }
            if (account.pin == 0) // Ensure that the user has entered a PIN
            {
               return BadRequest("PIN is required.");
            }

            if (DBManager.InsertBankAccount(account))
            {
                return Ok("Bank account added.");
            }
            return BadRequest("Bank account with the same account number already exists.");
        }

        [HttpPut]
        public IActionResult Put([FromBody] BankDataIntermed account, [FromQuery]int accountNumber)
        {
            Debug.WriteLine("VVVVVVVVVVVVVVVVVVVV");
            if (account == null)
            {
                return BadRequest("Bank account is required.");
            }
            if (account.pin == 0) // Ensure that the user has entered a PIN
            {
               return BadRequest("PIN is required.");
            }

            if (DBManager.UpdateBankAccount(account, accountNumber))
            {
                return Ok("Bank account updated.");
            }
            return NotFound("Account number could not be updated as it already exists."); // Update returns false
        }

        [HttpDelete]
        [Route("{accountNumber}")]
        public IActionResult Delete(int accountNumber)
        {
            if (DBManager.DeleteBankAccount(accountNumber))
            {
                return Ok("Bank account deleted.");
            }
            return NotFound("Account number does not exist.");
        }
    }
}
