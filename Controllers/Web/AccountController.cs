using BilgiYonetimSistemi.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;

namespace BilgiYonetimSistemi.Controllers.Web
{
    public class AccountController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AccountController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Login()
        {
            return View();
        }

        public IActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost]
        public IActionResult SendResetLink(string email)
        {
            var user = _context.Users.FirstOrDefault(u => u.Email == email);

            if (user != null)
            {
                ViewBag.Message = "Şifre sıfırlama bağlantısı e-posta adresinize gönderildi.";
            }
            else
            {
                ViewBag.ErrorMessage = "E-posta adresi ile eşleşen bir kullanıcı bulunamadı.";
            }

            return View("ForgotPassword");
        }

        [HttpPost]
        public async Task<IActionResult> LoginUser(string username, string password, string role)
        {
            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password) || string.IsNullOrWhiteSpace(role))
            {
                ViewBag.ErrorMessage = "Kullanıcı adı, şifre veya rol boş olamaz.";
                return View("Login");
            }

            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Username == username && u.Role == role);

            if (user != null)
            {
                if (user.PasswordHash != password)
                {
                    ViewBag.ErrorMessage = "Şifre yanlış.";
                    return View("Login");
                }

                HttpContext.Session.SetString("UserID", user.UserID.ToString());

                if (role == "Student")
                {
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

            ViewBag.ErrorMessage = "Geçersiz kullanıcı adı, şifre veya rol.";
            return View("Login");
        }
    }
}
