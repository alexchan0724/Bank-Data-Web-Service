﻿using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using API_Classes;
using Newtonsoft.Json;
using RestSharp;
using System.Security.AccessControl;


namespace LocalBusinessWebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class B_TransactionsController : Controller
    {
        RestClient restClient = new RestClient("http://localhost:5012");

        //-------------------------------------------------------------------

        [HttpGet]
        public IActionResult GetTransactions()
        {
            var request = new RestRequest("transaction/Transactions", Method.Get);
            var response = restClient.Execute(request);

            if (response.IsSuccessful)
            {
                var transactions = JsonConvert.DeserializeObject<List<TransactionDataIntermed>>(response.Content);
                return Ok(transactions);
            }
            else if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return NotFound("No transactions have been found in the database.");
            }
            else
            {
                return BadRequest("Failed to load audit logs.");
            }
        }

        [HttpGet("{username}")]
        public IActionResult GetTransactionByName(string username)
        {
            var request = new RestRequest($"transaction/Transactions/{username}", Method.Get); // Get user specific transactions
            request.AddUrlSegment("username", username);
            var response = restClient.Execute(request);
            if (response.IsSuccessful)
            {
                var transactions = JsonConvert.DeserializeObject<List<TransactionDataIntermed>>(response.Content);
                return Ok(transactions);
            }
            return NotFound(response.Content);
        }

        [HttpPost("Deposit")]
        public IActionResult Deposit([FromBody] TransactionDataIntermed transaction)
        {
            var request = new RestRequest($"transaction/Transactions/Deposit", Method.Post); // Get user specific transactions
            request.RequestFormat = RestSharp.DataFormat.Json;
            request.AddJsonBody(transaction);
            var response = restClient.Execute(request);
            if (response.IsSuccessful)
            {
                return Ok(response.Content);
            }
            else
            {
                return BadRequest(response.Content);
            }
        }

        [HttpPost("Withdraw")]
        public IActionResult Withdraw([FromBody] TransactionDataIntermed transaction)
        {
            var request = new RestRequest($"transaction/Transactions/Withdraw", Method.Post); // Get user specific transactions
            request.RequestFormat = RestSharp.DataFormat.Json;
            request.AddJsonBody(transaction);
            var response = restClient.Execute(request);
            if (response.IsSuccessful)
            {
                return Ok(response.Content);
            }
            else
            {
                return BadRequest(response.Content);
            }
        }
    }
}
