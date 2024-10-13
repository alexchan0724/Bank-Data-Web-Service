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
        /* public IActionResult Index()
        {
            return View();
        }
        [Route("/Home/Admin/getallAudit")]
        [HttpGet]
        public IActionResult getallAudit(string username, string password, string mode, string entry, string Admin)
        {

            var transactionRequest = new RestRequest($"api/B_Transactions/", Method.Get); // Get user-specific transactions
            var transactionResponse = restClient.Execute(transactionRequest);


            var transactionData = JsonConvert.DeserializeObject<List<TransactionDataIntermed>>(transactionResponse.Content);

            Debug.WriteLine("VVVVVVVV " + transactionData.Count);


            ViewBag.AuditLogs = transactionData;

            return View("~/Views/Home/AuditWindow.cshtml");
        }

        [Route("/Home/Admin/getAuditbyName/")]
        [HttpGet]
        public IActionResult getAuditbyName(string username)
        {
            Debug.WriteLine("GGGGGGGGGGG " + username);
            var AuditRequest = new RestRequest("api/B_Admin/B_AdminByUsername/{username}", Method.Get); // Get user-specific transactions
            AuditRequest.AddUrlSegment("username", username);

            var transactionResponse = restClient.Execute(AuditRequest);


            var transactionData = JsonConvert.DeserializeObject<List<TransactionDataIntermed>>(transactionResponse.Content);

            Debug.WriteLine("VVVVVVVV " + transactionData.Count);


            ViewBag.AuditLogs = transactionData;

            return View("~/Views/Home/AuditWindow.cshtml");
        }


        [Route("/Home/Admin/getAuditbyAccno/")]
        [HttpGet]
        public IActionResult getAuditbyAccno(string Accno)
        {
            Debug.WriteLine("GGGGGGGGGGG " + Accno);
            var AuditRequest = new RestRequest("api/B_Admin/B_AdminByAccno/{Accno}", Method.Get); // Get user-specific transactions
            AuditRequest.AddUrlSegment("Accno", Convert.ToInt32(Accno));

            var transactionResponse = restClient.Execute(AuditRequest);


            var transactionData = JsonConvert.DeserializeObject<List<TransactionDataIntermed>>(transactionResponse.Content);

            Debug.WriteLine("VVVVVVVV " + transactionData.Count);


            ViewBag.AuditLogs = transactionData;

            return View("~/Views/Home/AuditWindow.cshtml");
        } */
    }
}
