using Microsoft.AspNetCore.Mvc;

namespace BilgiYonetimSistemi.Controllers
{
    public class TranscriptsController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
