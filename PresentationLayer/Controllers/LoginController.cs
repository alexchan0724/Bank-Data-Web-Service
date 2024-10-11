using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace PresentationLayer.Controllers
{
    [Route("view/[controller]")]
    public class LoginController : Controller
    {
        [HttpGet("defaultview")]
        public IActionResult DefaultView()
        {
            Debug.WriteLine("Entered LoginController to load DefaultView");
            return PartialView("Login");
        }
    }
}
