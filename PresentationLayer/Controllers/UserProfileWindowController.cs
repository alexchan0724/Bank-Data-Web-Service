using Microsoft.AspNetCore.Mvc;
using API_Classes;
using System.Net;
namespace PresentationLayer.Controllers
{
    public class UserProfileWindowController : Controller
    {
        UserDataIntermed user;
        public IActionResult UserProfileWindow(UserDataIntermed User)
        {
            user = User;
            return View();

        }

        [HttpGet]
        public IActionResult getUserProfile()
        {
            ViewBag.Username = user.username;
            ViewBag.Email = user.email;
            ViewBag.Address = user.address;
            ViewBag.PhoneNo = user.phoneNum;
            return Ok(user);
        }

    }
}
