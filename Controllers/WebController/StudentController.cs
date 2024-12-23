using BilgiYonetimSistemi.Data;
using BilgiYonetimSistemi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Text;

namespace BilgiYonetimSistemi.Controllers.WebController
{
    public class StudentController : Controller
    {
        private readonly HttpClient _httpClient;
        private readonly ApplicationDbContext _repositoryDbContext;

        public StudentController(IHttpClientFactory httpClientFactory, ApplicationDbContext repositoryDbContext)
        {
            _httpClient = httpClientFactory.CreateClient();
            _httpClient.Timeout = TimeSpan.FromMinutes(5);

            _repositoryDbContext = repositoryDbContext;
        }


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

            try
            {
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

                var coursesJson = await response.Content.ReadAsStringAsync();
                var courses = JsonConvert.DeserializeObject<List<dynamic>>(coursesJson);
 
                ViewBag.Courses = courses;  
                return View("CourseSelection");  
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
            int a;
            string courseQuotaApiUrl = "https://localhost:7227/api/coursequotas/";
 
            foreach (var courseId in selectedCourseIds)
            {
 
                var response = await _httpClient.GetAsync($"{courseQuotaApiUrl}{courseId}");

                if (!response.IsSuccessStatusCode)
                {
                     TempData["ErrorMessage"] = "Kontenjan dolmuş, lütfen başka bir ders seçin.";
                    return RedirectToAction("PickLessons", "Student");
                }

                var quotaResponse = await response.Content.ReadFromJsonAsync<CourseQuotaResponse>();

                if (quotaResponse != null)
                {
                    int quota = quotaResponse.Quota;  
                    int remainingQuota = quotaResponse.RemainingQuota;
                    a = quotaResponse.RemainingQuota; 
 
                    if (remainingQuota <= 0)
                    {
                        TempData["ErrorMessage"] = "Kontenjan dolmuş, lütfen başka bir ders seçin.";
                        return RedirectToAction("PickLessons", "Student");
                    }
                }
            }

            // Kontenjanlar uygun, NonConfirmedSelections API'sine veri gönder
            foreach (var courseId in selectedCourseIds)
            {
                var nonConfirmedSelection = new NonConfirmedSelections
                {
                    StudentId = int.Parse(HttpContext.Session.GetString("StudentID")),
                    CourseId = courseId,
                    SelectedAt = DateTime.Now
                };


                // NonConfirmedSelections API'sine veri gönderiyoruz
                var response = await _httpClient.PostAsJsonAsync("https://localhost:7227/api/nonconfirmedselections", nonConfirmedSelection);

                if (!response.IsSuccessStatusCode)
                {
                    // API'ye veri gönderme başarısızsa kullanıcıyı bilgilendir
                    TempData["ErrorMessage"] = "Seçiminiz kaydedilemedi, tekrar deneyin.";
                    return RedirectToAction("PickLessons", "Student");
                }
            }

            // Tüm seçimler başarılıysa, kontenjanı güncelle
            foreach (var courseId in selectedCourseIds)
            {
                // Kontenjan bilgisini almak için API'yi çağırıyoruz
                var response = await _httpClient.GetAsync($"{courseQuotaApiUrl}{courseId}");

                if (!response.IsSuccessStatusCode)
                {
                    TempData["ErrorMessage"] = "Kontenjan bilgisi alınamadı, tekrar deneyin.";
                    return RedirectToAction("PickLessons", "Student");
                }

                // Gelen yanıtı `CourseQuotaResponse` modeline deserialize ediyoruz
                var quotaResponse = await response.Content.ReadFromJsonAsync<CourseQuotaResponse>();

                if (quotaResponse != null)
                {
                    // Kontenjanı bir azaltıyoruz
                    var courseQuota = new
                    {
                        RemainingQuota = quotaResponse.RemainingQuota - 1  // Güncellenmiş kontenjan değeri
                    };

                    // JSON verisine dönüştürüyoruz
                    var content = new StringContent(
                        JsonConvert.SerializeObject(courseQuota),  // JSON'a dönüştürülmüş veri
                        Encoding.UTF8,
                        "application/json"
                    );

                    // PATCH isteğini gönderiyoruz
                    var updateQuotaResponse = await _httpClient.PatchAsync($"{courseQuotaApiUrl}coursequotas/{courseId}", content);

                    if (!updateQuotaResponse.IsSuccessStatusCode)
                    {
                        TempData["ErrorMessage"] = "Kontenjan güncellenemedi, tekrar deneyin.";
                        return RedirectToAction("PickLessons", "Student");
                    }
                }
            }
            foreach (var courseId in selectedCourseIds)
            {
                int studentId = int.Parse(HttpContext.Session.GetString("StudentID"));
                string courseSelectionHistoryApiUrl = $"https://localhost:7227/api/CourseSelectionHistories";
                // Öğrencinin ders seçim bilgisi
                var courseSelectionHistory = new CourseSelectionHistory
                {
                    StudentID = studentId,
                    SelectionDate = DateTime.Now
                };

                // API'ye veri gönderme
                var response = await _httpClient.PostAsJsonAsync(courseSelectionHistoryApiUrl, courseSelectionHistory);

                if (!response.IsSuccessStatusCode)
                {
                    // API'ye veri gönderme başarısızsa kullanıcıyı bilgilendir
                    TempData["ErrorMessage"] = "Seçiminiz kaydedilemedi, tekrar deneyin.";
                    return RedirectToAction("PickLessons", "Student");
                }
            }



            // Her şey başarılıysa, başarı mesajı ve yönlendirme
            TempData["SuccessMessage"] = "Ders seçiminiz başarıyla kaydedildi!";
            return RedirectToAction("ConfirmedSelected", "Student");
        }

        [HttpGet]
        public async Task<IActionResult> StudentCourses(int studentId)
        {
            try
            {
                // API'ye istek gönder
                var response = await _httpClient.GetAsync($"https://localhost:7227/api/StudentCourseSelections/{studentId}");
                if (response.IsSuccessStatusCode)
                {
                    // API'den gelen yanıtı deserialize et
                    var responseData = await response.Content.ReadAsStringAsync();
                    var courses = JsonConvert.DeserializeObject<List<Course>>(responseData);

                    // Eğer dersler mevcutsa ViewBag'e ata
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
         
        public async Task<IActionResult> ApprovedCourses()
        {
            var apiUrl = "https://localhost:7227/api/StudentCourseSelections";
            var response = await _httpClient.GetAsync(apiUrl);

            if (!response.IsSuccessStatusCode)
            {
                // Hata durumunda bir mesaj dönebilirsiniz.
                return View("Error");
            }

            var jsonString = await response.Content.ReadAsStringAsync();
            var courses = JsonConvert.DeserializeObject<List<StudentCourseSelections>>(jsonString);

            // Sadece onaylanan dersleri filtrele.
            var approvedCourses = courses.Where(c => c.IsApproved).ToList();

            return View(approvedCourses);
        }
    }
}