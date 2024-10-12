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
            return PartialView();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
