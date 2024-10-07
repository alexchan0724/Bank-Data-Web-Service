using Microsoft.AspNetCore.Mvc;

namespace PresentationLayer.Controllers
{
    public class UserProfileWindowController : Controller
    {
        public IActionResult UserProfileWindow()
        {
            return View();
        }


    }
}
