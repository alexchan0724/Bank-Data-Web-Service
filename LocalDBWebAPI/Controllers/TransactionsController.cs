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
                DBManager.AddLogEntry("getTransactions failed, no transaction found in database");
                return NotFound("No transactions have been found in the database.");
            }
            //var transactionsToReturn = ConvertTransactionList(TransactionDataIntermed);
            DBManager.AddLogEntry("getTransactions successful, returning list of transactions");
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
                DBManager.AddLogEntry("getTransactions failed, could not find any transaction for the user");
                return NotFound("No transactions found for the user.");
            }
            DBManager.AddLogEntry("getTransactionsbyName successful, returning list of transactions from " + username);

            return Ok(transactions);
        }

        [HttpGet("byAccountNumber/{accountNumber}")]
        public IActionResult GetTransactionByAccount(int accountNumber)
        {
            List<TransactionDataIntermed> transactions = DBManager.GetTransactionByBankAccount(accountNumber);
            if (transactions == null || !transactions.Any())
            {
                DBManager.AddLogEntry("getTransactionsByAccount failed, could not find any transaction under the account number " + accountNumber);
                return NotFound("No transactions found for the account.");
            }
            //var transactionsToReturn = ConvertTransactionList(transactions);\
            DBManager.AddLogEntry("getTransactionsByAccount successful, returning list of transaction from accNo " + accountNumber);

            return Ok(transactions);
        }

        [HttpGet("byAccountNumberOrdered/{accountNumber}")]
        public IActionResult GetTransactionByAccountOrdered(int accountNumber)
        {
            List<TransactionDataIntermed> transactions = DBManager.GetTransactionOrderedByBankAccount(accountNumber);
            if (transactions == null || !transactions.Any())
            {
                DBManager.AddLogEntry("getTransactionsByAccountOrdered failed, no transaction found from account " + accountNumber);
                return NotFound("No transactions found for the account.");
            }
            //var transactionsToReturn = ConvertTransactionList(transactions);
            DBManager.AddLogEntry("getTransactionsByAccountOrdered successful, returning list of transaction from " + accountNumber);

            return Ok(transactions);
        }



        [HttpPost("Deposit")]
        public IActionResult Deposit([FromBody] TransactionDataIntermed transaction)
        {
            if (transaction == null)
            {
                DBManager.AddLogEntry("Deposit failed, Deposit is null");

                return BadRequest("Transaction is null.");
            }
            Debug.WriteLine("Transaction: " + transaction.transactionDescription);
            if (DBManager.DepositTransaction(transaction))
            {
                DBManager.AddLogEntry("Deposit successful");

                return Ok("Deposit successful.");
            }
            DBManager.AddLogEntry("Deposit failed");

            return BadRequest("Deposit failed.");
        }

        [HttpPost("Withdraw")]
        public IActionResult Withdraw([FromBody] TransactionDataIntermed transaction)
        {
            if (transaction == null)
            {
                DBManager.AddLogEntry("Withdrawal failed, Withdrawal is null");

                return BadRequest("Transaction is null.");
            }

            if (DBManager.WithdrawTransaction(transaction))
            {
                DBManager.AddLogEntry("Withdrawal successful");

                return Ok("Withdrawal successful.");
            }
            DBManager.AddLogEntry("withdrawl failed");

            return BadRequest("Withdrawal failed.");
        }

        [HttpPost("Transfer")]
        public IActionResult Transfer([FromBody] TransferDataIntermed transfer)
        {
            if (transfer == null || transfer.senderTransaction == null || transfer.receiverTransaction == null)
            {
                DBManager.AddLogEntry("transfer failed, invalid transfer data");

                return BadRequest("Invalid transfer data.");
            }

            TransactionDataIntermed senderTransaction = transfer.senderTransaction;
            TransactionDataIntermed receiverTransaction = transfer.receiverTransaction;

            bool transferResult = DBManager.TransferMoney(senderTransaction, receiverTransaction);

            if (transferResult)
            {
                DBManager.AddLogEntry("transfer successful");

                return Ok("Transfer successful.");
            }
            else
            {
                DBManager.AddLogEntry("transfer failed");

                return BadRequest("Transfer failed.");
            }
        }
    }
}
