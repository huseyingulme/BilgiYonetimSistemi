using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using BilgiYonetimSistemi.Models;
using System.Linq;

namespace BilgiYonetimSistemi.Controllers.Web
{
    public class StudentController : Controller
    {
        private readonly HttpClient _httpClient;

        public StudentController(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient();
        }

        // Öğrenci Panelini Görüntüleme
        public async Task<IActionResult> StudentPanel()
        {
            var studentIdString = HttpContext.Session.GetString("StudentID");

            if (string.IsNullOrEmpty(studentIdString))
            {
                return RedirectToAction("Login", "Account");
            }

            if (!int.TryParse(studentIdString, out int studentId))
            {
                return BadRequest("Geçersiz öğrenci ID'si.");
            }

            var studentResponse = await _httpClient.GetAsync($"https://localhost:7227/api/Students/{studentId}");
            if (!studentResponse.IsSuccessStatusCode)
            {
                ViewBag.ErrorMessage = "Öğrenci bilgileri yüklenemedi.";
                return View("Error");
            }

            var studentJson = await studentResponse.Content.ReadAsStringAsync();
            var student = JsonConvert.DeserializeObject<JObject>(studentJson);

            // Danışman bilgileri
            var advisor = student["advisor"];
            ViewBag.Student = student;
            ViewBag.Advisor = advisor != null
                ? new
                {
                    FullName = advisor["fullName"]?.ToString(),
                    Title = advisor["title"]?.ToString(),
                    Department = advisor["department"]?.ToString()
                }
                : null;

            // Dersler
            var coursesResponse = await _httpClient.GetAsync("https://localhost:7227/api/Courses");
            if (!coursesResponse.IsSuccessStatusCode)
            {
                ViewBag.ErrorMessage = "Ders bilgileri yüklenemedi.";
                return View("StudentPanel");
            }

            var courseJson = await coursesResponse.Content.ReadAsStringAsync();
            var courses = JsonConvert.DeserializeObject<List<dynamic>>(courseJson);
            ViewBag.Courses = courses;

            // Seçim işlemleri
            var nonConfirmedResponse = await _httpClient.GetAsync($"https://localhost:7227/api/NonConfirmedSelections/Student/{studentId}");
            var nonConfirmedSelections = await HandleCourseSelections(nonConfirmedResponse, studentId);

            if (nonConfirmedSelections != null)
            {
                ViewBag.SelectionMessage = "Ders seçiminiz danışman onayına gönderildi!";
                ViewBag.AllowCourseSelection = false;
                return View("StudentPanel");
            }

            // Onaylanmış dersler
            var approvedResponse = await _httpClient.GetAsync($"https://localhost:7227/api/StudentCourseSelections/{studentId}");
            var approvedSelections = await HandleApprovedCourseSelections(approvedResponse);

            if (approvedSelections != null)
            {
                ViewBag.Scs = approvedSelections;
                ViewBag.SelectionMessage = "Ders seçim işleminiz tamamlandı.";
                ViewBag.AllowCourseSelection = false;
                return View("StudentPanel");
            }

            // Seçim yapılmamışsa, ders seçimine izin ver
            ViewBag.AllowCourseSelection = true;
            return View("StudentPanel");
        }

        private async Task<List<dynamic>> HandleCourseSelections(HttpResponseMessage nonConfirmedResponse, int studentId)
        {
            if (!nonConfirmedResponse.IsSuccessStatusCode) return null;

            var nonConfirmedData = await nonConfirmedResponse.Content.ReadAsStringAsync();
            var nonConfirmedSelections = JsonConvert.DeserializeObject<List<dynamic>>(nonConfirmedData);

            return nonConfirmedSelections?.Any() ?? false ? nonConfirmedSelections : null;
        }

        private async Task<List<dynamic>> HandleApprovedCourseSelections(HttpResponseMessage approvedResponse)
        {
            if (!approvedResponse.IsSuccessStatusCode) return null;

            var approvedData = await approvedResponse.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<List<dynamic>>(approvedData);
        }

        [HttpGet]
        public async Task<IActionResult> CourseSelection()
        {
            try
            {
                var response = await _httpClient.GetAsync("https://localhost:7227/api/Courses");
                if (!response.IsSuccessStatusCode)
                {
                    ViewBag.ErrorMessage = "API'den ders bilgileri alınamadı.";
                    return View("CourseSelection");
                }

                var coursesJson = await response.Content.ReadAsStringAsync();
                var courses = JsonConvert.DeserializeObject<List<dynamic>>(coursesJson);

                if (courses == null || !courses.Any())
                {
                    ViewBag.ErrorMessage = "Ders listesi bulunamadı.";
                    return View("CourseSelection");
                }

                ViewBag.Courses = courses;
                return View("CourseSelection");
            }
            catch (Exception ex)
            {
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
                TempData["CourseSelectionMessage"] = "Oturum bilgisi alınamadı.";
                return RedirectToAction("Login", "Account");
            }

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
                    TempData["CourseSelectionMessage"] = "Seçiminiz kaydedilemedi.";
                    return RedirectToAction("StudentPanel");
                }
            }

            TempData["CourseSelectionMessage"] = "Ders seçiminiz danışman onayına gönderildi!";
            return RedirectToAction("StudentPanel");
        }

        [HttpGet]
        public async Task<IActionResult> StudentCourses(int studentId)
        {
            try
            {
                var response = await _httpClient.GetAsync($"https://localhost:7227/api/StudentCourseSelections/{studentId}");
                if (!response.IsSuccessStatusCode)
                {
                    ViewBag.StudentCourses = null;
                    TempData["ErrorMessage"] = "Ders bilgileri alınamadı.";
                    return RedirectToAction("StudentPanel");
                }

                var responseData = await response.Content.ReadAsStringAsync();
                var courses = JsonConvert.DeserializeObject<List<Course>>(responseData);

                ViewBag.StudentCourses = courses?.Any() ?? false ? courses : null;
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
                TempData["ErrorMessage"] = "Oturum bilgisi alınamadı.";
                return RedirectToAction("Login", "Account");
            }

            if (studentId != id)
            {
                TempData["ErrorMessage"] = "Geçersiz işlem. ID eşleşmiyor.";
                return RedirectToAction("StudentPanel");
            }

            var studentResponse = await _httpClient.GetAsync($"https://localhost:7227/api/Students/{id}");
            if (!studentResponse.IsSuccessStatusCode)
            {
                TempData["ErrorMessage"] = "Öğrenci bilgileri alınamadı.";
                return RedirectToAction("StudentPanel");
            }

            var studentJson = await studentResponse.Content.ReadAsStringAsync();
            var student = JsonConvert.DeserializeObject<JObject>(studentJson);

            var updatedAdvisorID = advisorID ?? student["advisor"]?["advisorID"]?.ToObject<int?>();

            var updatedStudent = new
            {
                StudentID = id,
                FirstName = student["firstName"]?.ToString(),
                LastName = student["lastName"]?.ToString(),
                Email = string.IsNullOrEmpty(newEmail) ? student["email"]?.ToString() : newEmail,
                AdvisorID = updatedAdvisorID,
                NonConfirmedSelections = student["nonConfirmedSelections"] ?? new JArray()
            };

            var jsonContent = new StringContent(JsonConvert.SerializeObject(updatedStudent), System.Text.Encoding.UTF8, "application/json");

            var updateResponse = await _httpClient.PutAsync($"https://localhost:7227/api/Students/{id}", jsonContent);

            if (!updateResponse.IsSuccessStatusCode)
            {
                var errorContent = await updateResponse.Content.ReadAsStringAsync();
                TempData["ErrorMessage"] = $"Öğrenci bilgileri güncellenirken bir hata oluştu: {errorContent}";
                return RedirectToAction("StudentPanel");
            }

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
