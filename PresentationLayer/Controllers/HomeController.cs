using Microsoft.AspNetCore.Mvc;
using PresentationLayer.Models;
using RestSharp;
using System.Diagnostics;
using Newtonsoft.Json;
using API_Classes;
using System.Xml.Linq;
using Microsoft.AspNetCore.Mvc.RazorPages;
using static System.Net.WebRequestMethods;

namespace PresentationLayer.Controllers
{
    public class HomeController : Controller
    {
        RestClient restClient = new RestClient("http://localhost:5290");

        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult about()
        {
            return View();
        }

        public IActionResult UserProfileWindow(UserDataIntermed user)
        {
            return View("UserProfileWindow", user);
        }

        [HttpGet]       //creating user
        public IActionResult ModifyUserWindow()
        {
            return View("ModifyUserWindow", "Home"); // Specify just the view name

        }
        [HttpGet]       //modifying user
        public IActionResult ModifyUserWindowtoNewDetails(string username, string password)
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
                return View("ModifyUserWindowtoNewDetails", userProfile); // Specify just the view name
            }
            return null;
        }


        [HttpGet]
        public IActionResult AuditWindow(string username, string password, string mode, string accNo)
        {
            Debug.WriteLine("DDDDDDDDDDDDDDDDDDDDDDDDDD " + accNo);


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
            return null;

        }





        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        [Route("/Home/Index")]
        [HttpPost]
        public IActionResult Index(string username, string password)
        {
            Debug.WriteLine("FFFFFFFFFFFFFF " + username +  password);

            if(!username.Equals("") && !password.Equals(""))
            {
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

                    Debug.WriteLine( "###################### " + userProfile.username + userProfile.password + userProfile.isAdmin);

                    return UserProfileWindow(userProfile);
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.BadRequest)
                {
                    return BadRequest("Error: " + "Password is required.");
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    return NotFound("Error: " + "Parameter does not match any emails or usernames in the database.");
                }
                else
                {
                    return BadRequest("issue unknown");
                }
            }
            else
            {
                return BadRequest("USERNAME OR PASSWORD CANNOT BE EMPTY");
            }
        }
    }
}
