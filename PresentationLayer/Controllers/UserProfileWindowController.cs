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


            RestRequest request = new RestRequest("api/B_UserProfiles/{checkString}", Method.Post);
            request.AddUrlSegment("checkString", username);
            UserDataIntermed a = new UserDataIntermed();
            a.profilePicture = null;
            a.username = username;
            a.password = password;
            a.email = null;
            a.address = null;
            a.isAdmin = 0;


            request.AddJsonBody(a);

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
            RestRequest getUserRequest = new RestRequest("api/B_UserProfiles/{checkString}", Method.Post);
            getUserRequest.AddUrlSegment("checkString", username);
            UserDataIntermed a = new UserDataIntermed();
            a.profilePicture = null;
            a.username = username;
            a.password = password;
            a.email = null;
            a.address = null;
            a.isAdmin = 0;


            getUserRequest.AddJsonBody(a);

            var response = restClient.Execute(getUserRequest);
            if (response.IsSuccessful)
            {
                Debug.WriteLine("Response was successful");
                var userProfile = JsonConvert.DeserializeObject<UserDataIntermed>(response.Content);

                Debug.WriteLine("!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!");


                RestRequest getAccountRequest = new RestRequest($"api/B_BankAccounts/{Accnum}", Method.Get);
                getAccountRequest.AddQueryParameter("username", userProfile.username);
                RestResponse accountResponse = restClient.Execute(getAccountRequest);

                if (accountResponse.IsSuccessful)
                {
                    Debug.WriteLine("BBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBB");
                }
                UserDataIntermed user = userProfile;
                BankDataIntermed account = JsonConvert.DeserializeObject<BankDataIntermed>(accountResponse.Content);

                ViewBag.user = user;
                ViewBag.account = account;

                Debug.WriteLine("SSSSSSSSSSSS " + account.accountNumber + "kms" + account.email + "kms" + account.description);



                return View("~/Views/Home/BankAccountWindow.cshtml");
            }
            else
            {
                return null;

            }



        }

    }
}
