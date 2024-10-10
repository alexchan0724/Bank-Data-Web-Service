using Microsoft.AspNetCore.Mvc;

namespace PresentationLayer.Controllers
{
    public class AuditController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
