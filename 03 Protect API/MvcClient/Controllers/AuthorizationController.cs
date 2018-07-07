using Microsoft.AspNetCore.Mvc;

namespace MvcClient.Controllers
{
    public class AuthorizationController : Controller
    {
        public IActionResult AccessDenied()
        {
            return View();
        }
    }
}