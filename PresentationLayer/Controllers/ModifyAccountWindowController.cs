using API_Classes;
using Microsoft.AspNetCore.Mvc;
using RestSharp;
using System.Diagnostics;
using Newtonsoft.Json;

namespace PresentationLayer.Controllers
{
    public class ModifyAccountWindowController : Controller
    {
        [Route ("/Home/ModifyAccountWindow/addAccount")]
        [HttpPost]
        public IActionResult addAccount(string AccNum, string Pin, string description, string username, string password)
        {
            RestRequest request = new RestRequest("api/B_UserProfiles/{checkString}", Method.Get);
            request.AddUrlSegment("checkString", username);
            request.AddParameter("password", password);
            RestClient restClient = new RestClient("http://localhost:5290");

            var lebron = restClient.Execute(request);
            Debug.WriteLine("Response was successful");
            var user = JsonConvert.DeserializeObject<UserDataIntermed>(lebron.Content);
            // Create a BankAccount object with form data
            BankDataIntermed bankAccount = new BankDataIntermed();
            bankAccount.accountNumber = Convert.ToInt32(AccNum);
            bankAccount.pin = Convert.ToInt32(Pin);
            bankAccount.description = description;


            bankAccount.balance = 0;
            bankAccount.email = user.email;
            bankAccount.username = user.username;
            request = new RestRequest("api/B_BankAccounts", Method.Post);
            // Set the request format to JSON and add the bank account as the body
            request.RequestFormat = RestSharp.DataFormat.Json;
            request.AddJsonBody(bankAccount);
            RestResponse response = restClient.Execute(request);
            if (response.IsSuccessful)
            {
                Debug.WriteLine("ACCOUNT MADE SUCCESSFULLY");
            }
            return View("~/Views/Home/UserProfileWindow.cshtml", user);
        }
    }
}
