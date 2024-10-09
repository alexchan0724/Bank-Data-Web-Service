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

        [HttpGet]
        public IActionResult ModifyUserWindow()
        {
            return View("ModifyUserWindow", "Home"); // Specify just the view name

        }





        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        [HttpPost]
        public IActionResult Index(string SUName, [FromBody] UserDataIntermed user)
        {
            if(!SUName.Equals("") && !user.password.Equals(""))
            {
                RestRequest request = new RestRequest("api/B_UserProfiles/{checkString}", Method.Post);
                request.AddUrlSegment("checkString", SUName);
                request.AddJsonBody("password", user.password);

                var response = restClient.Execute(request);
                if (response.IsSuccessful)
                {
                    Debug.WriteLine("Response was successful");
                    var userProfile = JsonConvert.DeserializeObject<UserDataIntermed>(response.Content);
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
