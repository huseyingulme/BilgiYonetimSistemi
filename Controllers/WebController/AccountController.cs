using BilgiYonetimSistemi.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BilgiYonetimSistemi.Controllers.WebController
{
    public class AccountController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AccountController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Login
        public IActionResult Login()
        {
            return View();
        }

        // Kullanıcı girişini kontrol eden aksiyon
        [HttpPost]
        public async Task<IActionResult> Login(string username, string password, string role)
        {
            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password) || string.IsNullOrWhiteSpace(role))
            {
                ViewBag.ErrorMessage = "Kullanıcı adı, şifre veya rol boş olamaz.";
                return View("Login");
            }

            // Kullanıcıyı kontrol et
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Username == username && u.Role == role);

            if (user != null)
            {
                // Şifre doğrulama (örnek: Hash'li şifre karşılaştırması)
                if (user.PasswordHash != password) // Doğru bir şifreleme kontrolü ekleyin
                {
                    ViewBag.ErrorMessage = "Şifre yanlış.";
                    return View("Login");
                }

                // Session'a UserID ekle
                HttpContext.Session.SetString("UserID", user.UserID.ToString());

                if (role == "Student")
                {
                    // Öğrenci ID'sini oturuma ekle
                    if (user.RelatedID.HasValue)
                    {
                        HttpContext.Session.SetString("StudentID", user.RelatedID.Value.ToString());
                    }
                    else
                    {
                        ViewBag.ErrorMessage = "İlgili öğrenci bilgisi bulunamadı.";
                        return View("Login");
                    }

                    return RedirectToAction("StudentPanel", "Student");
                }
                else if (role == "Advisor")
                {
                    // Advisor ID'sini oturuma ekle
                    if (user.RelatedID.HasValue)
                    {
                        HttpContext.Session.SetString("AdvisorID", user.RelatedID.Value.ToString());
                    }
                    else
                    {
                        ViewBag.ErrorMessage = "İlgili danışman bilgisi bulunamadı.";
                        return View("Login");
                    }

                    return RedirectToAction("AdvisorPanel", "Advisor");
                }
            }
            else
            {
                ViewBag.ErrorMessage = "Kullanıcı adı veya rol yanlış.";
                return View("Login");
            }

            // Beklenmeyen durumlar için bir return eklenmeli
            ViewBag.ErrorMessage = "Bir hata oluştu. Lütfen tekrar deneyin.";
            return View("Login");
        }

        public IActionResult Logout()
        {
            // Oturumu kapat
            HttpContext.Session.Clear();

            // Kullanıcıyı giriş sayfasına yönlendir
            return RedirectToAction("Login", "Account");
        }
    }
}
