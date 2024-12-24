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
            var userRole = HttpContext.Session.GetString("Role");

            if (!string.IsNullOrEmpty(userRole))
            {
                if (userRole == "Student")
                {
                    // Öğrenci rolü ile ana sayfaya yönlendir
                    return RedirectToAction("Login", "Student");
                }
                else if (userRole == "Advisor")
                {
                    // Danışman rolü ile ana sayfaya yönlendir
                    return RedirectToAction("Login", "Advisor");
                }
            }
            return View();
        }

        // Kullanıcı girişini kontrol eden aksiyon
        [HttpPost]
        public async Task<IActionResult> Login(string Username, string Password, string Role)
        {
            if (string.IsNullOrWhiteSpace(Username) || string.IsNullOrWhiteSpace(Password) || string.IsNullOrWhiteSpace(Role))
            {
                ViewBag.ErrorMessage = "Kullanıcı adı, şifre veya rol boş olamaz.";
                return View("Login");
            }

            // Kullanıcıyı kontrol et
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Username == Username && u.Role == Role);

            if (user != null)
            {
                // Şifre doğrulama (örnek: Hash'li şifre karşılaştırması)
                if (user.PasswordHash != Password) // Doğru bir şifreleme kontrolü ekleyin
                {
                    ViewBag.ErrorMessage = "Şifre yanlış.";
                    return View("Login");
                }

                // Session'a UserID ekle
                HttpContext.Session.SetString("UserID", user.UserID.ToString());

                if (Role == "Student")
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
                else if (Role == "Advisor")
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
