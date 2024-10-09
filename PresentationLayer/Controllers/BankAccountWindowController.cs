using API_Classes;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using RestSharp;
using System.Diagnostics;
using System.Xml.Linq;

namespace PresentationLayer.Controllers
{
    public class BankAccountWindowController : Controller
    {

        RestClient restClient = new RestClient("http://localhost:5290");

        [Route("/Home/BankAccountWindow/Deposit")]
        [HttpPost]
        public IActionResult Deposit(string username, string password, string accNo, string description, int oldBalance, string newBalance)
        {


            Debug.WriteLine("#################" + accNo + "#" + description + "#" + newBalance + "#" + oldBalance);
            DateTime date = DateTime.Now;

            TransactionDataIntermed newTransaction = new TransactionDataIntermed(Convert.ToInt32(accNo), description, Convert.ToInt32(newBalance), date);
            if (Convert.ToInt32(newBalance) > 0)
            {
                RestRequest request = new RestRequest($"api/B_Transactions/Deposit", Method.Post);
                request.RequestFormat = RestSharp.DataFormat.Json;
                request.AddJsonBody(newTransaction);
                RestResponse response = restClient.Execute(request);
            }
            else if (Convert.ToInt32(newBalance) < 0)
            {
                Debug.WriteLine("NNNNNNNNNNNNN " + Convert.ToInt32(newBalance));
                RestRequest request = new RestRequest($"api/B_Transactions/Withdraw", Method.Post);
                request.RequestFormat = RestSharp.DataFormat.Json;
                request.AddJsonBody(newTransaction);
                RestResponse response = restClient.Execute(request);
            }
            RestRequest userRequest = new RestRequest("api/B_UserProfiles/{checkString}", Method.Post);
            userRequest.AddUrlSegment("checkString", username);
            UserDataIntermed a = new UserDataIntermed();
            a.profilePicture = null;
            a.username = username;
            a.password = password;
            a.email = null;
            a.address = null;
            a.isAdmin = 0;


            userRequest.AddJsonBody(a);
            var lebron = restClient.Execute(userRequest);
            var user = JsonConvert.DeserializeObject<UserDataIntermed>(lebron.Content);


            return View("~/Views/Home/UserProfileWindow.cshtml", user);
        }
    }
}
