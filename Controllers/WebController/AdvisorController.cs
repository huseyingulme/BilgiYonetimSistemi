using BilgiYonetimSistemi.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Text;

namespace BilgiYonetimSistemi.Controllers.WebController
{
    public class AdvisorController : Controller
    {
        private readonly HttpClient _httpClient;

        // IHttpClientFactory DI ile inject ediliyor
        public AdvisorController(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient();
        }

        public async Task<IActionResult> AdvisorPanel()
        {
            var advisorIdString = HttpContext.Session.GetString("AdvisorID");
            if (string.IsNullOrEmpty(advisorIdString))
            {
                // Eğer oturumda AdvisorID yoksa Login sayfasına yönlendir
                return RedirectToAction("Login", "Account");
            }

            if (!int.TryParse(advisorIdString, out int advisorId))
            {
                return BadRequest("Geçersiz Danışman ID'si.");
            }

            // API'den danışman bilgilerini çek
            var response = await _httpClient.GetAsync($"https://localhost:7227/api/Advisors/{advisorId}");
            if (!response.IsSuccessStatusCode)
            {
                ViewBag.ErrorMessage = "Danışman bilgileri yüklenemedi.";
                return View("Error");
            }

            var advisorJson = await response.Content.ReadAsStringAsync();
            var advisor = JsonConvert.DeserializeObject<dynamic>(advisorJson);

            // Danışman bilgilerini ViewBag ile View'a aktar
            ViewBag.Advisor = advisor;

            try
            {
                var responsee = await _httpClient.GetAsync($"https://localhost:7227/api/NonConfirmedSelections/Advisor/{advisorId}");

                if (responsee.IsSuccessStatusCode)
                {
                    var data = await responsee.Content.ReadAsStringAsync();
                    // Gelen JSON verilerini dinamik bir listeye çevir
                    var pendingCourses = JsonConvert.DeserializeObject<List<dynamic>>(data);

                    // ViewBag'e atayın
                    ViewBag.PendingCourses = pendingCourses;
                }
                else
                {
                    ViewBag.PendingCourses = null;
                }
            }
            catch (Exception ex)
            {
                ViewBag.PendingCourses = null;
                TempData["ErrorMessage"] = $"Ders listesi yüklenirken bir hata oluştu: {ex.Message}";
            }

            return View();
        }





        [HttpPost]
        public async Task<IActionResult> UpdateAdvisor(int id, string newEmail, string newPassword)
        {
            // Oturumdan AdvisorID ve UserID'yi alın
            var advisorIdString = HttpContext.Session.GetString("AdvisorID");
            var userIdString = HttpContext.Session.GetString("UserID");

            if (string.IsNullOrEmpty(advisorIdString) || !int.TryParse(advisorIdString, out int advisorId))
            {
                TempData["ErrorMessage"] = "Oturum bilgisi alınamadı. Lütfen tekrar giriş yapın.";
                return RedirectToAction("Login", "Account");
            }

            if (advisorId != id)
            {
                TempData["ErrorMessage"] = "Geçersiz işlem. ID eşleşmiyor.";
                return RedirectToAction("AdvisorPanel");
            }

            if (string.IsNullOrEmpty(newEmail) && string.IsNullOrEmpty(newPassword))
            {
                TempData["ErrorMessage"] = "En az bir alan doldurulmalıdır.";
                return RedirectToAction("AdvisorPanel");
            }

            // Danışman bilgilerini Advisors API'den al
            var advisorResponse = await _httpClient.GetAsync($"https://localhost:7227/api/Advisors/{id}");
            if (!advisorResponse.IsSuccessStatusCode)
            {
                TempData["ErrorMessage"] = "Danışman bilgileri alınamadı.";
                return RedirectToAction("AdvisorPanel");
            }

            var advisorJson = await advisorResponse.Content.ReadAsStringAsync();
            var advisor = JsonConvert.DeserializeObject<Advisors>(advisorJson);

            // Advisors API'de yalnızca email güncellemesi yapılır
            if (!string.IsNullOrEmpty(newEmail))
            {
                var updatedAdvisor = new
                {
                    AdvisorID = advisor.AdvisorID,
                    FullName = advisor.FullName,
                    Title = advisor.Title,
                    Department = advisor.Department,
                    Email = newEmail 
                };

                var advisorJsonContent = new StringContent(JsonConvert.SerializeObject(updatedAdvisor), System.Text.Encoding.UTF8, "application/json");
                var updateAdvisorResponse = await _httpClient.PutAsync($"https://localhost:7227/api/Advisors/{id}", advisorJsonContent);

                if (!updateAdvisorResponse.IsSuccessStatusCode)
                {
                    var errorContent = await updateAdvisorResponse.Content.ReadAsStringAsync();
                    TempData["ErrorMessage"] = $"Danışman bilgileri güncellenirken bir hata oluştu: {errorContent}";
                    return RedirectToAction("AdvisorPanel");
                }
            }

            // Kullanıcı bilgilerini Users API'den al
            if (!string.IsNullOrEmpty(userIdString))
            {
                var userResponse = await _httpClient.GetAsync($"https://localhost:7227/api/Users/{userIdString}");
                if (!userResponse.IsSuccessStatusCode)
                {
                    TempData["ErrorMessage"] = "Kullanıcı bilgileri alınamadı.";
                    return RedirectToAction("AdvisorPanel");
                }

                var userJson = await userResponse.Content.ReadAsStringAsync();
                var user = JsonConvert.DeserializeObject<dynamic>(userJson);

                // Users API'de email ve/veya şifre güncellemesi yapılır
                var updatedUser = new
                {
                    UserID = user.userID,
                    Username = user.username,
                    PasswordHash = !string.IsNullOrEmpty(newPassword) ? newPassword : user.passwordHash, // Şifreyi güncelle
                    Email = !string.IsNullOrEmpty(newEmail) ? newEmail : user.email, // E-posta güncelle
                    Role = user.role,
                    RelatedID = user.relatedID
                };

                var userJsonContent = new StringContent(JsonConvert.SerializeObject(updatedUser), System.Text.Encoding.UTF8, "application/json");
                var updateUserResponse = await _httpClient.PutAsync($"https://localhost:7227/api/Users/{userIdString}", userJsonContent);

                if (!updateUserResponse.IsSuccessStatusCode)
                {
                    var errorContent = await updateUserResponse.Content.ReadAsStringAsync();
                    TempData["ErrorMessage"] = $"Kullanıcı bilgileri güncellenirken bir hata oluştu: {errorContent}";
                    return RedirectToAction("AdvisorPanel");
                }
            }

            TempData["SuccessMessage"] = "Bilgiler başarıyla güncellendi.";
            return RedirectToAction("AdvisorPanel");
        }

        [HttpGet]
        public async Task<IActionResult> GetPendingCourses()
        {
            var response = await _httpClient.GetAsync("https://localhost:7227/api/NonConfirmedSelections");

            if (!response.IsSuccessStatusCode)
            {
                TempData["ErrorMessage"] = "Onay bekleyen dersler alınamadı.";
                return RedirectToAction("AdvisorPanel");
            }

            var json = await response.Content.ReadAsStringAsync();
            var pendingCourses = JsonConvert.DeserializeObject<List<dynamic>>(json);

            ViewBag.PendingCourses = pendingCourses;

            return View("PendingCourses");
        }
        [HttpPost]
        public async Task<IActionResult> RejectCourse(int id)
        {
            try
            {
                // NonConfirmedSelections API'den sil
                var response = await _httpClient.DeleteAsync($"https://localhost:7227/api/NonConfirmedSelections/{id}");

                if (response.IsSuccessStatusCode)
                {
                    TempData["RejectCourse"] = "Ders reddedildi.";
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    TempData["RejectCourse"] = $"Ders reddedilirken bir hata oluştu: {errorContent}";
                }
            }
            catch (Exception ex)
            {
                TempData["RejectCourse"] = $"Bir hata oluştu: {ex.Message}";
            }

            return RedirectToAction("AdvisorPanel");
        }


        [HttpPost]
        public async Task<IActionResult> ApproveCourse(int id, int studentID, int courseID)
        {
            try
            {
                // Yeni seçim oluştur
                var approvedSelection = new
                {
                    CourseID = courseID,
                    StudentID = studentID,
                    SelectionDate = DateTime.Now,
                    IsApproved = true
                };

                // API'ye POST isteği gönder
                var content = new StringContent(JsonConvert.SerializeObject(approvedSelection), Encoding.UTF8, "application/json");
                var response = await _httpClient.PostAsync("https://localhost:7227/api/StudentCourseSelections", content);

                if (response.IsSuccessStatusCode)
                {
                    // Onaylanan seçimi NonConfirmedSelections'dan sil
                    await _httpClient.DeleteAsync($"https://localhost:7227/api/NonConfirmedSelections/{id}");
                    TempData["ApproveCourse"] = "Ders başarıyla onaylandı.";
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    TempData["ApproveCourse"] = $"Ders onaylanırken bir hata oluştu: {errorContent}";
                }
            }
            catch (Exception ex)
            {
                TempData["ApproveCourse"] = $"Bir hata oluştu: {ex.Message}";
            }

            return RedirectToAction("AdvisorPanel");
        }

    }

}
