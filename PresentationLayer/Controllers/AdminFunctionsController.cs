using API_Classes;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using RestSharp;
using System.Diagnostics;

namespace PresentationLayer.Controllers
{
    [Route("admin/[controller]")]

    public class AdminFunctionsController : Controller
    {
        RestClient restClient = new RestClient("http://localhost:5290");

        [HttpGet("manageUsers")]
        public IActionResult ManageUsers()
        {
            Debug.WriteLine("Entered AdminFunctionsController to load ManageUsers");
            ViewBag.Users = new List<UserDataIntermed>(); // Initially leave users List empty

            return PartialView("ManageUsers");
        }

        [HttpGet("searchUsers/{searchString}")] // After admin has tried to search for a user
        public IActionResult SearchUsers(string searchString)
        {
            Debug.WriteLine("Entered AdminFunctionsController to search for users");
            var request = new RestRequest($"api/B_Admin/searchUsers/{searchString}", Method.Get);

            var response = restClient.Execute(request);

            if (response.IsSuccessful)
            {
                var users = JsonConvert.DeserializeObject<List<UserDataIntermed>>(response.Content);
                return Json(users); // Return the list of users as JSON
            }
            else
            {
                return BadRequest("Failed to retrieve users."); // Return a BadRequest with an error message
            }
        }

        [Route("displayUsers")] // After admin has selected a name
        [HttpPost]
        public IActionResult ManageUsersWithAccounts([FromBody] List<UserDataIntermed>users)
        {
            Debug.WriteLine("Entered AdminFunctionsController to load ManageUsers");
            ViewBag.Users = users; // Initially leave users List empty

            return PartialView("ManageUsers");
        }

        [Route("deleteUsers")]
        [HttpPost]
        public IActionResult DeleteUsers([FromBody] string usernameToDelete)
        {
            Debug.WriteLine("Entered AdminFunctionsController to delete users");
            var request = new RestRequest($"api/B_UserProfiles/{usernameToDelete}", Method.Delete);

            var response = restClient.Execute(request);

            if (response.IsSuccessful)
            {
                ViewBag.Message = "User has been deleted successfully.";
                return PartialView("ManageUsers");
            }
            else
            {
                return BadRequest(response.Content);
            }
        }

        [Route("logs")]
        [HttpGet]
        public IActionResult Logs()
        {
            Debug.WriteLine("Entered AdminFunctionsController to load Logs");
            var request = new RestRequest("api/B_Admin/auditLogs", Method.Get);
            var response = restClient.Execute(request);
            if (response.IsSuccessful) {
                var logEntries = JsonConvert.DeserializeObject<List<LogDataIntermed>>(response.Content);
                ViewBag.LogEntries = logEntries;
                return PartialView("Logs", logEntries);
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

        [Route("allTransactions")]
        [HttpGet]
        public IActionResult GetAllTransactions()
        {
            var request = new RestRequest("api/B_Transactions", Method.Get);
            var response = restClient.Execute(request);
            if (response.IsSuccessful)
            {
                var transactions = JsonConvert.DeserializeObject<List<TransactionDataIntermed>>(response.Content);
                ViewBag.Transactions = transactions;
                return PartialView("AuditTransactions");
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

        [Route("filterTransactions")]
        [HttpPost]
        public IActionResult FilterTransactions([FromBody] FilterTransactionsIntermed filter)
        {
            var request = new RestRequest("api/B_Admin/filterTransactions", Method.Post);
            request.AddJsonBody(filter);
            var response = restClient.Execute(request);
            if (response.IsSuccessful)
            {
                var transactions = JsonConvert.DeserializeObject<List<TransactionDataIntermed>>(response.Content);
                ViewBag.Transactions = transactions;
                return PartialView("AuditTransactions");
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
