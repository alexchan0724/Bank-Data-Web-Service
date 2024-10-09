using Microsoft.AspNetCore.Mvc;
using API_Classes;
using System.Net;
using Newtonsoft.Json;
using RestSharp;
using System.Diagnostics;
namespace PresentationLayer.Controllers
{
    public class UserProfileWindowController : Controller
    {
        RestClient restClient = new RestClient("http://localhost:5290");

        public IActionResult UserProfileWindow(UserDataIntermed User)
        {
            return View();
        }
        [Route("/Home/UserProfileWindow/createAccount")]
        [HttpPost]
        public IActionResult createAccount(string username, string password)
        {
            Debug.WriteLine("@@@@@@@@@@@@@@@@@@@@@@" + username, password);
            RestRequest request = new RestRequest("api/B_UserProfiles/{checkString}", Method.Get);
            request.AddUrlSegment("checkString", username);
            request.AddParameter("password", password);

            var response = restClient.Execute(request);
            if (response.IsSuccessful)
            {
                Debug.WriteLine("Response was successful");
                var userProfile = JsonConvert.DeserializeObject<UserDataIntermed>(response.Content);
                return View("~/Views/Home/ModifyAccountWindow.cshtml", userProfile);
            }
            else
            {
                return null;

            }
        }

        [Route("/Home/UserProfileWindow/getAccount")]
        [HttpPost]
        public IActionResult getAccount(string username, string password, string Accnum)
        {
            RestRequest request = new RestRequest("api/B_UserProfiles/{checkString}", Method.Get);
            request.AddUrlSegment("checkString", username);
            request.AddParameter("password", password);

            var response = restClient.Execute(request);
            if (response.IsSuccessful)
            {
                Debug.WriteLine("Response was successful");
                var userProfile = JsonConvert.DeserializeObject<UserDataIntermed>(response.Content);

                RestRequest request1 = new RestRequest($"api/B_BankAccounts/{Accnum}", Method.Get);
                request.AddUrlSegment("accountNumber", Accnum);
                request.AddQueryParameter("username", userProfile.username);
                RestResponse response1 = restClient.Execute(request);

                ViewBag.user = JsonConvert.DeserializeObject<UserDataIntermed>(response1.Content);
                ViewBag.Accnum = Accnum;

                return View("~/Views/Home/BankAccountWindow.cshtml");
            }
            else
            {
                return null;

            }



        }

    }
}
