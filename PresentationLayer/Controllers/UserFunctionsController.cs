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
            Debug.WriteLine("Entered UserFunctionsController to AuthenticateUser");
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
            return PartialView("CreateAccount", user);
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

        [Route("addNewAccount")]
        [HttpPost]
        public IActionResult AddNewAccount([FromBody] BankDataIntermed account)
        {
            Debug.WriteLine("Entered AddNewUser in UserFunctionsController");
            Debug.WriteLine("Account details:" + account.balance + " " + account.description + " " + account.email + " " + account.pin + " " + account.username + " " + account.accountNumber);
            RestRequest request = new RestRequest("api/B_BankAccounts", Method.Post);
            request.AddJsonBody(account);
            var response = restClient.Execute(request);
            if (response.IsSuccessful)
            {
                return PartialView("UserSuccess");
            }
            return null;
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
                return PartialView("AuditWindowAll", user);
            }
            return null;
        }

        [Route("auditAccount")]
        [HttpPost]
        public IActionResult getAccountTransactions([FromBody]UserRequest userRequest)
        {
            Debug.WriteLine("Entered getAccountTransactions method in UserProfileWindow");
            Debug.WriteLine("Account number in getAccountTransactions: " + userRequest.AccountNumber);
            RestRequest request = new RestRequest("api/B_Transactions/byAccount/", Method.Get);
            request.AddQueryParameter("accountNumber", userRequest.AccountNumber); // AccountNumber is already a string
            var response = restClient.Execute(request);
            if (response.IsSuccessful)
            {
                var result = JsonConvert.DeserializeObject<List<TransactionDataIntermed>>(response.Content);
                ViewBag.AccountNumber = userRequest.AccountNumber;
                ViewBag.AuditLogs = result;
                return PartialView("AuditWindow", userRequest.User);
            }
            return null;
        }

        [Route("auditAccountOrdered")]
        [HttpPost]
        public IActionResult getAccountTransactionsOrdered([FromBody]UserRequest userRequest)
        {
            Debug.WriteLine("Entered getAccountTransactionsOrdered method in UserProfileWindow");
            Debug.WriteLine("Account number in getAccountTransactionsOrdered: " + userRequest.AccountNumber);
            RestRequest request = new RestRequest("api/B_Transactions/byAccountOrdered/", Method.Get);
            request.AddQueryParameter("accountNumber", userRequest.AccountNumber); // AccountNumber is already a string
            var response = restClient.Execute(request);
            if (response.IsSuccessful)
            {
                var result = JsonConvert.DeserializeObject<List<TransactionDataIntermed>>(response.Content);
                ViewBag.AuditLogs = result;
                ViewBag.AccountNumber = userRequest.AccountNumber;
                return PartialView("AuditWindow", userRequest.User);
            }
            return null;
        }

        [Route("modifyUser")]
        [HttpPost]
        public IActionResult ModifyUser([FromBody]UserDataIntermed user)
        {
            Debug.WriteLine("Entered ModifyUser in UserFunctionsController");
            return PartialView("ModifyUserWindow", user);
        }

        [Route("createUser")]
        [HttpGet]
        public IActionResult CreateUser()
        {
            Debug.WriteLine("Entered CreateUser in UserFunctionsController");
            return PartialView("CreateUserWindow", "Home"); // Specify just the view name
        }

        [Route("addNewUser")]
        [HttpPost]
        public IActionResult AddNewUser([FromBody]UserDataIntermed user)
        {
            Debug.WriteLine("Entered AddNewUser in UserFunctionsController");
            RestRequest request = new RestRequest("api/B_UserProfiles", Method.Post);
            request.AddJsonBody(user);
            var response = restClient.Execute(request);
            if (response.IsSuccessful)
            {
                return PartialView("UserSuccess");
            }
            return null;
        }

        [Route("sendModifyRequest")]
        [HttpPost]
        public IActionResult SendModifyRequest([FromBody]UserRequest userRequest)
        {
            Debug.WriteLine("Entered SendModifyRequest in UserFunctionsController");
            RestRequest request = new RestRequest("api/B_UserProfiles", Method.Put);
            request.AddJsonBody(userRequest.User);
            request.AddQueryParameter("oldUsername", userRequest.OldUsername);
            request.AddQueryParameter("oldEmail", userRequest.OldEmail);
            var response = restClient.Execute(request);
            if (response.IsSuccessful)
            {
                return PartialView("UserSuccess");
            }
            return null;
        }
    }
}
