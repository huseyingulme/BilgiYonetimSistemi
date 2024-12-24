using BilgiYonetimSistemi.Data;
using BilgiYonetimSistemi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Text;

namespace BilgiYonetimSistemi.Controllers.WebController
{
    public class StudentController : Controller
    {
        private readonly HttpClient _httpClient;
        private readonly ApplicationDbContext _context;

        public StudentController(HttpClient httpClient, ApplicationDbContext context)
        {
            _httpClient = httpClient;
            _context = context;
        }

        // Öğrenci Panelini Görüntüleme
        public async Task<IActionResult> StudentPanel()
        {
            var studentIdString = HttpContext.Session.GetString("StudentID");

            if (string.IsNullOrEmpty(studentIdString))
            {
                // Oturum yoksa giriş sayfasına yönlendirme
                return RedirectToAction("Login", "Account");
            }

            if (!int.TryParse(studentIdString, out int studentId))
            {
                return BadRequest("Geçersiz öğrenci ID'si.");
            }

            // Öğrenci bilgilerini API'den çek
            var response = await _httpClient.GetAsync($"https://localhost:7227/api/Students/{studentId}");
            if (!response.IsSuccessStatusCode)
            {
                ViewBag.ErrorMessage = "Öğrenci bilgileri yüklenemedi.";
                return View("Error");
            }

            var studentJson = await response.Content.ReadAsStringAsync();
            var student = JsonConvert.DeserializeObject<JObject>(studentJson);

            // Danışman bilgilerini al
            var advisor = student["advisor"];
            ViewBag.Student = student;
            if (advisor != null)
            {
                ViewBag.Advisor = new
                {
                    FullName = advisor["fullName"]?.ToString(),
                    Title = advisor["title"]?.ToString(),
                    Department = advisor["department"]?.ToString()
                };
            }
            else
            {
                ViewBag.Advisor = null;
            }
               // Dersleri API'den çek
               var courseResponse = await _httpClient.GetAsync("https://localhost:7227/api/Courses");
               if (!courseResponse.IsSuccessStatusCode)
               {
                   ViewBag.ErrorMessage = "Ders bilgileri yüklenemedi. Lütfen tekrar deneyin.";
                   return View("StudentPanel");
               }
               var courseJson = await courseResponse.Content.ReadAsStringAsync();
               var courses = JsonConvert.DeserializeObject<List<dynamic>>(courseJson);

               // Dersleri ViewBag'e aktar
               ViewBag.Courses = courses; 


            try
            {
                // Öğrencinin onay bekleyen seçimlerini kontrol et
                var nonConfirmedResponse = await _httpClient.GetAsync($"https://localhost:7227/api/NonConfirmedSelections/Student/{studentId}");
                if (nonConfirmedResponse.IsSuccessStatusCode)
                {
                    var nonConfirmedData = await nonConfirmedResponse.Content.ReadAsStringAsync();
                    var nonConfirmedSelections = JsonConvert.DeserializeObject<List<dynamic>>(nonConfirmedData);

                    if (nonConfirmedSelections != null && nonConfirmedSelections.Any())
                    {
                        ViewBag.SelectionMessage = "Ders seçiminiz başarıyla danışman onayına gönderildi! Ders seçiminiz danışman onayı bekliyor.";
                        ViewBag.AllowCourseSelection = false;
                        return View("StudentPanel");
                    }
                }

                // Öğrencinin onaylanmış ders seçimlerini kontrol et
                var approvedResponse = await _httpClient.GetAsync($"https://localhost:7227/api/StudentCourseSelections/{studentId}");
                if (approvedResponse.IsSuccessStatusCode)
                {
                    var approvedData = await approvedResponse.Content.ReadAsStringAsync();
                    var approvedSelections = JsonConvert.DeserializeObject<List<dynamic>>(approvedData);

                    if (approvedSelections != null && approvedSelections.Any())
                    {
                        ViewBag.Scs = approvedSelections;
                        ViewBag.SelectionMessage = "Ders seçim işleminiz tamamlanmıştır.";
                        ViewBag.AllowCourseSelection = false;
                        return View("StudentPanel");
                    }
                }

                // Ders seçimi yapılmamış, seçim yapmaya izin ver
                var coursesResponse = await _httpClient.GetAsync($"https://localhost:7227/api/Courses");
                if (coursesResponse.IsSuccessStatusCode)
                {
                    var coursesData = await coursesResponse.Content.ReadAsStringAsync();
                    var availableCourses = JsonConvert.DeserializeObject<List<dynamic>>(coursesData);
                    ViewBag.Courses = availableCourses;
                }

                ViewBag.SelectionMessage = null;
                ViewBag.AllowCourseSelection = true;
                return View("StudentPanel");
            }
            catch (Exception ex)
            {
                ViewBag.SelectionMessage = $"Bir hata oluştu: {ex.Message}";
                ViewBag.AllowCourseSelection = false;
                return View("StudentPanel");
            }
        }

        [HttpGet]
        public async Task<IActionResult> CourseSelection()
        {
            try
            {
                // API'den dersleri al
                var response = await _httpClient.GetAsync("https://localhost:7227/api/Courses");

                if (!response.IsSuccessStatusCode)
                {
                    ViewBag.ErrorMessage = "API'den ders bilgileri alınamadı.";
                    return View("CourseSelection");
                }

                // JSON yanıtını al ve deserialize et
                var coursesJson = await response.Content.ReadAsStringAsync();
                var courses = JsonConvert.DeserializeObject<List<dynamic>>(coursesJson);

                if (courses == null || !courses.Any())
                {
                    ViewBag.ErrorMessage = "Ders listesi bulunamadı.";
                    return View("CourseSelection");
                }

                ViewBag.Courses = courses; // Ders listesini ViewBag'e aktar
                return View("CourseSelection"); // Ders seçimi sayfasını göster
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Hata: {ex.Message}");
                ViewBag.ErrorMessage = "Bir hata oluştu.";
                return View("CourseSelection");
            }
        }
        [HttpPost]
        public async Task<IActionResult> SubmitCourseSelections(List<int> selectedCourseIds)
        {
            if (selectedCourseIds == null || selectedCourseIds.Count == 0)
            {
                TempData["CourseSelectionMessage"] = "Lütfen en az bir ders seçin.";
                return RedirectToAction("StudentPanel");
            }

            var studentIdString = HttpContext.Session.GetString("StudentID");
            if (string.IsNullOrEmpty(studentIdString) || !int.TryParse(studentIdString, out int studentId))
            {
                TempData["CourseSelectionMessage"] = "Oturum bilgisi alınamadı. Lütfen tekrar giriş yapın.";
                return RedirectToAction("Login", "Account");
            }

            // Derslerin veritabanına veya API'ye kaydedilmesi
            foreach (var courseId in selectedCourseIds)
            {
                var nonConfirmedSelection = new
                {
                    StudentId = studentId,
                    CourseId = courseId,
                    SelectedAt = DateTime.Now
                };

                var response = await _httpClient.PostAsJsonAsync("https://localhost:7227/api/NonConfirmedSelections", nonConfirmedSelection);

                if (!response.IsSuccessStatusCode)
                {
                    TempData["CourseSelectionMessage"] = "Seçiminiz kaydedilemedi. Lütfen tekrar deneyin.";
                    return RedirectToAction("StudentPanel");
                }
            }

            TempData["CourseSelectionMessage"] = "Ders seçiminiz başarıyla danışman onayına gönderildi!";
            return RedirectToAction("StudentPanel");
        }


        [HttpGet]
        public async Task<IActionResult> StudentCourses(int studentId)
        {
            try
            {
                var response = await _httpClient.GetAsync($"https://localhost:7227/api/StudentCourseSelections/{studentId}");
                if (response.IsSuccessStatusCode)
                {
                    var responseData = await response.Content.ReadAsStringAsync();
                    var courses = JsonConvert.DeserializeObject<List<Course>>(responseData);

                    if (courses != null && courses.Any())
                    {
                        ViewBag.StudentCourses = courses;
                    }
                    else
                    {
                        ViewBag.StudentCourses = null;
                    }
                }
                else
                {
                    ViewBag.StudentCourses = null;
                    TempData["ErrorMessage"] = "Ders bilgileri alınamadı.";
                }
            }
            catch (Exception ex)
            {
                ViewBag.StudentCourses = null;
                TempData["ErrorMessage"] = $"Bir hata oluştu: {ex.Message}";
            }

            return RedirectToAction("StudentPanel");
        }

        [HttpPost]
        public async Task<IActionResult> UpdateStudent(int id, string newEmail, string newPassword, int? advisorID)
        {
            var studentIdString = HttpContext.Session.GetString("StudentID");
            var userIdString = HttpContext.Session.GetString("UserID");

            if (string.IsNullOrEmpty(studentIdString) || !int.TryParse(studentIdString, out int studentId))
            {
                TempData["ErrorMessage"] = "Oturum bilgisi alınamadı. Lütfen tekrar giriş yapın.";
                return RedirectToAction("Login", "Account");
            }

            if (studentId != id)
            {
                TempData["ErrorMessage"] = "Geçersiz işlem. ID eşleşmiyor.";
                return RedirectToAction("StudentPanel");
            }

            // Students API'den mevcut öğrenci bilgilerini al
            var response = await _httpClient.GetAsync($"https://localhost:7227/api/Students/{id}");
            if (!response.IsSuccessStatusCode)
            {
                TempData["ErrorMessage"] = "Öğrenci bilgileri alınamadı.";
                return RedirectToAction("StudentPanel");
            }

            var studentJson = await response.Content.ReadAsStringAsync();
            var student = JsonConvert.DeserializeObject<JObject>(studentJson);

            // `StudentCourseSelections` ve `NonConfirmedSelections` alanlarını kontrol et
            var studentCourseSelections = student["studentCourseSelections"] ?? new JArray();
            var nonConfirmedSelections = student["nonConfirmedSelections"] ?? new JArray();

            // AdvisorID'yi kontrol et ve mevcut değeri koru
            var updatedAdvisorID = advisorID ?? student["advisor"]?["advisorID"]?.ToObject<int?>();

            // Güncellenmiş JSON'u oluştur
            var updatedStudent = new
            {
                StudentID = id,
                FirstName = student["firstName"]?.ToString(),
                LastName = student["lastName"]?.ToString(),
                Email = string.IsNullOrEmpty(newEmail) ? student["email"]?.ToString() : newEmail,
                AdvisorID = updatedAdvisorID,
                StudentCourseSelections = studentCourseSelections, // Zorunlu alanı ekle
                NonConfirmedSelections = nonConfirmedSelections   // Zorunlu alanı ekle
            };

            var jsonContent = new StringContent(JsonConvert.SerializeObject(updatedStudent), System.Text.Encoding.UTF8, "application/json");

            // Students API'ye PUT isteği gönder
            var updateResponse = await _httpClient.PutAsync($"https://localhost:7227/api/Students/{id}", jsonContent);

            if (!updateResponse.IsSuccessStatusCode)
            {
                var errorContent = await updateResponse.Content.ReadAsStringAsync();
                TempData["ErrorMessage"] = $"Öğrenci bilgileri güncellenirken bir hata oluştu: {errorContent}";
                return RedirectToAction("StudentPanel");
            }

            // Users API'de email ve/veya şifre güncellemesi
            if (!string.IsNullOrEmpty(userIdString) && (!string.IsNullOrEmpty(newEmail) || !string.IsNullOrEmpty(newPassword)))
            {
                var userResponse = await _httpClient.GetAsync($"https://localhost:7227/api/Users/{userIdString}");
                if (!userResponse.IsSuccessStatusCode)
                {
                    TempData["ErrorMessage"] = "Kullanıcı bilgileri alınamadı.";
                    return RedirectToAction("StudentPanel");
                }

                var userJson = await userResponse.Content.ReadAsStringAsync();
                var user = JsonConvert.DeserializeObject<JObject>(userJson);

                var updatedUser = new
                {
                    UserID = user["userID"],
                    Username = user["username"]?.ToString(),
                    PasswordHash = !string.IsNullOrEmpty(newPassword) ? newPassword : user["passwordHash"]?.ToString(),
                    Email = !string.IsNullOrEmpty(newEmail) ? newEmail : user["email"]?.ToString(),
                    Role = user["role"]?.ToString(),
                    RelatedID = user["relatedID"]
                };

                var userJsonContent = new StringContent(JsonConvert.SerializeObject(updatedUser), System.Text.Encoding.UTF8, "application/json");

                var updateUserResponse = await _httpClient.PutAsync($"https://localhost:7227/api/Users/{userIdString}", userJsonContent);

                if (!updateUserResponse.IsSuccessStatusCode)
                {
                    var errorContent = await updateUserResponse.Content.ReadAsStringAsync();
                    TempData["ErrorMessage"] = $"Kullanıcı bilgileri güncellenirken bir hata oluştu: {errorContent}";
                    return RedirectToAction("StudentPanel");
                }
            }

            TempData["SuccessMessage"] = "Bilgiler başarıyla güncellendi.";
            return RedirectToAction("StudentPanel");
        }
    }
}
