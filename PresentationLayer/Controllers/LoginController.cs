using API_Classes;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using RestSharp;
using System.Diagnostics;

namespace PresentationLayer.Controllers
{
    [Route("view/[controller]")]
    public class LoginController : Controller
    {
        RestClient restClient = new RestClient("http://localhost:5290");

        [HttpGet("defaultview")]
        public IActionResult DefaultView()
        {
            Debug.WriteLine("Entered LoginController to load DefaultView");
            return PartialView("Login");
        }

        [HttpPost("auth")]
        public IActionResult AuthenticateUser([FromBody] UserDataIntermed user)
        {
            Debug.WriteLine("Entered LoginController to return response");
            var returnObject = new { login = false, user = (UserDataIntermed)null };
            RestRequest request = new RestRequest("api/B_UserProfiles/{checkString}", Method.Post);
            request.AddUrlSegment("checkString", user.username);
            request.AddJsonBody(user);
            var response = restClient.Execute(request);
            if (response.IsSuccessful)
            {
                var result = JsonConvert.DeserializeObject<UserDataIntermed>(response.Content);
                returnObject = new 
                { 
                    login = true,
                    user = result
                };
            }
            return Json(returnObject);
        }

        [HttpPost("authview")]
        public IActionResult AuthView([FromBody] UserDataIntermed user)
        {
            Debug.WriteLine("Entered LoginController to load AuthView");
            if (user == null || string.IsNullOrEmpty(user.username) || string.IsNullOrEmpty(user.password))
            {
                Debug.WriteLine("User is null or username/password is empty");
                return PartialView("LoginErrorView");
            }
            RestRequest request = new RestRequest("api/B_UserProfiles/{checkString}", Method.Post);
            request.AddUrlSegment("checkString", user.username);
            request.AddJsonBody(user);
            var userRequest = restClient.Execute(request);
            if (userRequest.IsSuccessful)
            {
                var userProfile = JsonConvert.DeserializeObject<UserDataIntermed>(userRequest.Content);
                Debug.WriteLine($"User profile in AuthView: Username={userProfile.username}, Phone={userProfile.phoneNum}, Address={userProfile.address}, Email={userProfile.email}, Password={userProfile.password}, IsAdmin={userProfile.isAdmin}");

                // Retrieve the bank accounts for the authenticated user
                RestRequest bankRequest = new RestRequest("api/B_BankAccounts", Method.Get);
                bankRequest.AddQueryParameter("username", userProfile.username);
                var bankResponse = restClient.Execute(bankRequest);

                if (bankResponse.IsSuccessful)
                {
                    var bankAccounts = JsonConvert.DeserializeObject<List<BankDataIntermed>>(bankResponse.Content);
                    ViewBag.BankAccounts = bankAccounts; 
                    return PartialView("UserProfileWindow", userProfile);
                }
                else
                {
                    Debug.WriteLine("Failed to retrieve bank accounts.");
                    return PartialView("LoginErrorView");
                }
            }
            else
            {
                Debug.WriteLine("Failed to authenticate user.");
                return PartialView("LoginErrorView");
            }
        }

        [HttpGet("errorview")]
        public IActionResult Error()
        {
            Debug.WriteLine("Entered LoginController to load Error");
            return PartialView("LoginErrorView");
        }
    }
}
