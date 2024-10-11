using API_Classes;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using RestSharp;
using System.Diagnostics;

namespace LocalBusinessWebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class B_AdminController : Controller
    {
        RestClient restClient = new RestClient("http://localhost:5012");

        [HttpGet("B_AdminByUsername/{username}")]
        public IActionResult AdminByUsername(string username)
        {
            Debug.WriteLine("LLLLLLLLLLLLLLLLLLLLLLLLLL" + username);
            var request = new RestRequest($"transaction/transactions/ByUsername/{username}", Method.Get);
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

        [HttpGet("B_AdminByAccno/{Accno}")]
        public IActionResult AdminByAccno(int Accno)
        {
            Debug.WriteLine("LLLLLLLLLLLLLLLLLLLLLLLLLL" + Accno);
            var request = new RestRequest($"transaction/transactions/ByAccountNumber/{Accno}", Method.Get);
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
    }
}
