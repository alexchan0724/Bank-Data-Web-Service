using API_Classes;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using RestSharp;
using System.Diagnostics;

namespace PresentationLayer.Controllers
{
    public class AdminAuditController : Controller
    {
        RestClient restClient = new RestClient("http://localhost:5290");

        public IActionResult Index()
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
        }
    }
}
