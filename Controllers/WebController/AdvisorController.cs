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

            // advisorIdString'i int'e dönüştür
            if (!int.TryParse(advisorIdString, out int advisorId))
            {
                ViewBag.ErrorMessage = "Danışman ID hatalı.";
                return View("Error");
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

            // Dersleri API'den çek
            var courseResponse = await _httpClient.GetAsync("https://localhost:7227/api/Courses");
            if (!courseResponse.IsSuccessStatusCode)
            {
                ViewBag.ErrorMessage = "Ders bilgileri yüklenemedi. Lütfen tekrar deneyin.";
                return View("AdvisorPanel");
            }

            var courseJson = await courseResponse.Content.ReadAsStringAsync();
            var courses = JsonConvert.DeserializeObject<List<dynamic>>(courseJson);

            // Sadece seçmeli dersleri filtrele (isMandatory = false)
            var electiveCourses = courses.Where(course => course.isMandatory == false).ToList();

            // Kota bilgilerini ekle
            var coursesWithQuota = new List<dynamic>();
            foreach (var course in electiveCourses)
            {
                var quotaResponse = await _httpClient.GetAsync($"https://localhost:7227/api/CourseQuotas/{course.courseID}");
                if (quotaResponse.IsSuccessStatusCode)
                {
                    var quotaJson = await quotaResponse.Content.ReadAsStringAsync();
                    var quotaData = JsonConvert.DeserializeObject<dynamic>(quotaJson);

                    // Kota bilgilerini ders objesine ekle
                    coursesWithQuota.Add(new
                    {
                        course.courseID,
                        course.courseName,
                        course.courseCode,
                        course.credit,
                        quota = quotaData.quota,
                        remainingQuota = quotaData.remainingQuota
                    });
                }
                else
                {
                    // Kota bilgisi alınamadığında varsayılan değerler ekle
                    coursesWithQuota.Add(new
                    {
                        course.courseID,
                        course.courseName,
                        course.courseCode,
                        course.credit,
                        quota = "-",
                        remainingQuota = "-"
                    });
                }
            }

            // ViewBag'e kota bilgileriyle birlikte seçmeli dersleri ekle
            ViewBag.Courses = coursesWithQuota;

            return View();
        }

        [HttpGet]
        public async Task<IActionResult> GetPendingCourses()
        {
            var apiUrl = "https://localhost:7227/api/NonConfirmedSelections";
            var response = await _httpClient.GetAsync(apiUrl);

            if (response.IsSuccessStatusCode)
            {
                var data = await response.Content.ReadAsStringAsync();
                var pendingCourses = JsonConvert.DeserializeObject<List<dynamic>>(data);
                ViewBag.PendingCourses = pendingCourses;
                return View("PendingCourses");
            }

            TempData["ErrorMessage"] = "Onay bekleyen dersler alınamadı.";
            return RedirectToAction("AdvisorPanel");
        }

        [HttpPost]
        public async Task<IActionResult> RejectCourse(int id)
        {
            try
            {
                var apiUrl = $"https://localhost:7227/api/NonConfirmedSelections/{id}";
                var response = await _httpClient.DeleteAsync(apiUrl);

                if (response.IsSuccessStatusCode)
                {
                    TempData["RejectCourse"] = "Ders reddedildi.";
                }
                else
                {
                    var errorMessage = await response.Content.ReadAsStringAsync();
                    TempData["RejectCourse"] = $"Ders reddedilirken bir hata oluştu: {errorMessage}";
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
                var approvedSelection = new
                {
                    CourseID = courseID,
                    StudentID = studentID,
                    SelectionDate = DateTime.Now,
                    IsApproved = true
                };

                var apiUrl = "https://localhost:7227/api/StudentCourseSelections";
                var content = new StringContent(JsonConvert.SerializeObject(approvedSelection), Encoding.UTF8, "application/json");
                var response = await _httpClient.PostAsync(apiUrl, content);

                if (response.IsSuccessStatusCode)
                {
                    var deleteUrl = $"https://localhost:7227/api/NonConfirmedSelections/{id}";
                    await _httpClient.DeleteAsync(deleteUrl);
                    TempData["ApproveCourse"] = "Ders başarıyla onaylandı.";
                }
                else
                {
                    var errorMessage = await response.Content.ReadAsStringAsync();
                    TempData["ApproveCourse"] = $"Ders onaylanırken bir hata oluştu: {errorMessage}";
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
