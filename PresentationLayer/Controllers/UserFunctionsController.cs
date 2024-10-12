using Microsoft.AspNetCore.Mvc;
using API_Classes;
using System.Net;
using Newtonsoft.Json;
using RestSharp;
using System.Diagnostics;
namespace PresentationLayer.Controllers
{
    [Route("user/[controller]")]
    public class UserFunctionsController : Controller
    {
        RestClient restClient = new RestClient("http://localhost:5290");

        public IActionResult UserProfileWindow(UserDataIntermed User)
        {
            return View();
        }

        [HttpPost("auth")]
        public IActionResult AuthenticateUser([FromBody] UserRequest userRequest)
        {
            Debug.WriteLine("Entered UserProfileWindowController to return response");
            var returnObject = new 
            { 
                login = false, 
                user = (UserDataIntermed)null, 
                accountNumber = userRequest.AccountNumber 
            };
            RestRequest request = new RestRequest("api/B_UserProfiles/{checkString}", Method.Post);
            request.AddUrlSegment("checkString", userRequest.User.username);
            request.AddJsonBody(userRequest.User);
            var response = restClient.Execute(request);
            var result = JsonConvert.DeserializeObject<UserDataIntermed>(response.Content);
            if (response.IsSuccessful)
            {
                returnObject = new
                {
                    login = true,
                    user = result,
                    accountNumber = userRequest.AccountNumber
                };
            }
            return Json(returnObject);
        }

        [Route("createAccount")]
        [HttpPost]
        public IActionResult createAccount([FromBody]UserDataIntermed user)
        {
            Debug.WriteLine("Entered createAccount method in UserProfileWindow");
            return PartialView("ModifyAccountWindow", user);
        }

        [Route("getAccount")]
        [HttpPost]
        public IActionResult getAccount([FromBody]UserRequest userRequest)
        {
            Debug.WriteLine("Account number in UserFunctionsController: " + userRequest.AccountNumber);
            Debug.WriteLine("Username in UserFunctionsController: " + userRequest.User.username);
            RestRequest getAccountRequest = new RestRequest($"api/B_BankAccounts/{userRequest.AccountNumber}", Method.Get);    
            getAccountRequest.AddQueryParameter("username", userRequest.User.username);
                RestResponse accountResponse = restClient.Execute(getAccountRequest);

                if (accountResponse.IsSuccessful)
                {
                    Debug.WriteLine("BBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBB");
                }
                BankDataIntermed account = JsonConvert.DeserializeObject<BankDataIntermed>(accountResponse.Content);

                ViewBag.user = userRequest.User;
                ViewBag.account = account;

                Debug.WriteLine("SSSSSSSSSSSS " + account.accountNumber + "kms" + account.email + "kms" + account.description);

                return PartialView("BankAccountWindow");
        }

        [Route("auditAll")]
        [HttpPost]
        public IActionResult getAllTransactions([FromBody]UserDataIntermed user)
        {
            Debug.WriteLine("Entered getAllTransactions method in UserProfileWindow");
            RestRequest request = new RestRequest("api/B_Transactions/byUser/", Method.Get);
            request.AddQueryParameter("username", user.username);
            var response = restClient.Execute(request);
            var result = JsonConvert.DeserializeObject<List<TransactionDataIntermed>>(response.Content);
            if (response.IsSuccessful)
            {
                ViewBag.AuditLogs = result;
                return PartialView("AuditWindow");
            }
            return null;
        }


        [HttpGet]
        public IActionResult AuditWindow(string username, string password, string mode, string accNo, string Admin)
        {
            Debug.WriteLine("DDDDDDDDDDDDDDDDDDDDDDDDDD " + accNo);

            if (Convert.ToInt32(Admin) == 0)
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

                var userRequest = restClient.Execute(getUserRequest);
                if (userRequest.IsSuccessful)
                {
                    var userProfile = JsonConvert.DeserializeObject<UserDataIntermed>(userRequest.Content);
                    ViewBag.user = userProfile;

                    if (Convert.ToInt32(mode) == 0)
                    {
                        var transactionRequest = new RestRequest($"api/B_Transactions/ByUsername/{username}", Method.Get); // Get user-specific transactions
                        transactionRequest.AddUrlSegment("username", userProfile.username);
                        var transactionResponse = restClient.Execute(transactionRequest);


                        var transactionData = JsonConvert.DeserializeObject<List<TransactionDataIntermed>>(transactionResponse.Content);

                        Debug.WriteLine("VVVVVVVV " + transactionData.Count);


                        ViewBag.AuditLogs = transactionData;

                        return View("AuditWindow");
                    }
                    else
                    {
                        Debug.WriteLine("DDDDDDDDDDDDDDDDDDDDDDDDDD " + accNo);

                        var transactionRequest = new RestRequest($"api/B_Transactions/ByAccountNumber/{accNo}", Method.Get); // Get user-specific transactions
                        transactionRequest.AddUrlSegment("accNo", Convert.ToInt32(accNo));
                        var transactionResponse = restClient.Execute(transactionRequest);


                        var transactionData = JsonConvert.DeserializeObject<List<TransactionDataIntermed>>(transactionResponse.Content);

                        Debug.WriteLine("VVVVVVVV " + transactionData.Count);


                        ViewBag.AuditLogs = transactionData;

                        return View("AuditWindow");
                    }
                }
            }
            return null;
        }
    }
}
