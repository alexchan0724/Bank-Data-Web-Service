using API_Classes;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using RestSharp;
using System.Diagnostics;

namespace PresentationLayer.Controllers
{
    [Route("admin/[controller]")]
    public class AdminAuditController : Controller
    {
        RestClient restClient = new RestClient("http://localhost:5290");

        public IActionResult Index()
        {
            return View();
        }
        [Route("Admin/allTransaction")]
        [HttpGet]
        public IActionResult getallAudit()
        {

            var transactionRequest = new RestRequest($"api/B_Transactions/", Method.Get); // Get user-specific transactions
            var transactionResponse = restClient.Execute(transactionRequest);


            var transactionData = JsonConvert.DeserializeObject<List<TransactionDataIntermed>>(transactionResponse.Content);

            Debug.WriteLine("VVVVVVVV " + transactionData.Count);


            ViewBag.AuditLogs = transactionData;

            return PartialView("AuditWindowAll");
        }
    }
}
