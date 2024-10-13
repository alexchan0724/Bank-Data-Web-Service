using LocalDBWebAPI.Data;
using Microsoft.AspNetCore.Mvc;
using LocalDBWebAPI.Models;
using API_Classes;
using System.Diagnostics;

namespace LocalDBWebAPI.Controllers
{
    [Route("transaction/[controller]")]
    [ApiController]
    public class TransactionsController : ControllerBase
    {
        [HttpGet]
        public IActionResult GetTransactions()
        {
            List<TransactionDataIntermed> transactions = DBManager.GetAllTransactions();
            Debug.WriteLine("Transactions found: " + transactions.Count);
            if (transactions == null || !transactions.Any())
            {
                return NotFound("No transactions have been found in the database.");
            }
            //var transactionsToReturn = ConvertTransactionList(TransactionDataIntermed);
            return Ok(transactions);
        }

        [HttpGet("ByUsername/{username}")]
        public IActionResult GetTransactionByName(string username)
        {
            List<TransactionDataIntermed> transactions = DBManager.GetTransactionsByUser(username);

            foreach(TransactionDataIntermed transaction in transactions)
            {
                Debug.WriteLine("SSSSSSSSSSSSSSSSSSSSSSSSSSSSS " + transaction.transactionDescription);
            }
            if (transactions == null || !transactions.Any())
            {
                return NotFound("No transactions found for the user.");
            }
            return Ok(transactions);
        }

        [HttpGet("byAccountNumber/{accountNumber}")]
        public IActionResult GetTransactionByAccount(int accountNumber)
        {
            List<TransactionDataIntermed> transactions = DBManager.GetTransactionByBankAccount(accountNumber);
            if (transactions == null || !transactions.Any())
            {
                return NotFound("No transactions found for the account.");
            }
            //var transactionsToReturn = ConvertTransactionList(transactions);
            return Ok(transactions);
        }

        [HttpGet("byAccountNumberOrdered/{accountNumber}")]
        public IActionResult GetTransactionByAccountOrdered(int accountNumber)
        {
            List<TransactionDataIntermed> transactions = DBManager.GetTransactionOrderedByBankAccount(accountNumber);
            if (transactions == null || !transactions.Any())
            {
                return NotFound("No transactions found for the account.");
            }
            //var transactionsToReturn = ConvertTransactionList(transactions);
            return Ok(transactions);
        }



        [HttpPost("Deposit")]
        public IActionResult Deposit([FromBody] TransactionDataIntermed transaction)
        {
            if (transaction == null)
            {
                return BadRequest("Transaction is null.");
            }
            Debug.WriteLine("Transaction: " + transaction.transactionDescription);
            if (DBManager.DepositTransaction(transaction))
            {
                return Ok("Deposit successful.");
            }
            return BadRequest("Deposit failed.");
        }

        [HttpPost("Withdraw")]
        public IActionResult Withdraw([FromBody] TransactionDataIntermed transaction)
        {
            if (transaction == null)
            {
                return BadRequest("Transaction is null.");
            }

            if (DBManager.WithdrawTransaction(transaction))
            {
                return Ok("Withdrawal successful.");
            }
            return BadRequest("Withdrawal failed.");
        }

        [HttpPost("Transfer")]
        public IActionResult Transfer([FromBody] TransferDataIntermed transfer)
        {
            if (transfer == null || transfer.senderTransaction == null || transfer.receiverTransaction == null)
            {
                return BadRequest("Invalid transfer data.");
            }

            TransactionDataIntermed senderTransaction = transfer.senderTransaction;
            TransactionDataIntermed receiverTransaction = transfer.receiverTransaction;

            bool transferResult = DBManager.TransferMoney(senderTransaction, receiverTransaction);

            if (transferResult)
            {
                return Ok("Transfer successful.");
            }
            else
            {
                return BadRequest("Transfer failed.");
            }
        }
    }
}
