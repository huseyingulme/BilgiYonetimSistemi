using Microsoft.AspNetCore.Mvc;

namespace BilgiYonetimSistemi.Controllers
{
    public class StudentController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
