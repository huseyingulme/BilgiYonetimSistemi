using Microsoft.AspNetCore.Mvc;

namespace BilgiYonetimSistemi.Controllers
{
    public class UsersController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
