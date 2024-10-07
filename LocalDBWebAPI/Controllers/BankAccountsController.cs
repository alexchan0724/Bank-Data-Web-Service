﻿using Microsoft.AspNetCore.Mvc;
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
        public IActionResult Get(int accountNumber, [FromQuery]string username)
        {
            var account = DBManager.GetBankAccount(accountNumber, username);

            if (account == null)
            {
                return NotFound("Account number does not exist.");
            }
            else
            {
                BankDataIntermed returnBankAccount = new BankDataIntermed();
                returnBankAccount.accountNumber = account.accountNumber;
                returnBankAccount.balance = account.balance;
                returnBankAccount.pin = account.pin;
                returnBankAccount.description = account.description;
                return Ok(returnBankAccount);
            }
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
