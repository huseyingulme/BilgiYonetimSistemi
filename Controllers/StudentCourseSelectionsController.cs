using BilgiYonetimSistemi.Data;
using BilgiYonetimSistemi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BilgiYonetimSistemi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CourseSelectionsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public CourseSelectionsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/CourseSelections
        [HttpGet]
        public async Task<ActionResult<IEnumerable<object>>> TumDersSecimleriniGetir()
        {
            var ogrencilerVeDersler = await _context.Students
                .Include(s => s.CourseSelection)
                    .ThenInclude(cs => cs.Course)
                .Select(s => new
                {
                    s.StudentID,
                    s.FirstName,
                    s.LastName,
                    Dersler = s.CourseSelection.Select(cs => new
                    {
                        cs.CourseID,
                        cs.SelectionDate,
                        cs.Course.CourseName
                    }).ToList()
                })
                .ToListAsync();

            if (ogrencilerVeDersler == null || !ogrencilerVeDersler.Any())
            {
                return NotFound(new { Mesaj = "Öğrenci veya ders seçimi bulunamadı." });
            }

            return Ok(ogrencilerVeDersler);
        }

        // GET: api/CourseSelections/Student/5
        [HttpGet("Student/{studentId}")]
        public async Task<ActionResult<IEnumerable<object>>> OgrenciDersSecimleriniGetir(int studentId)
        {
            var secimler = await _context.CourseSelection
                .Where(cs => cs.StudentID == studentId)
                .Select(cs => new
                {
                    cs.Student.StudentID,
                    Ders = new
                    {
                        cs.Course.CourseCode,
                        cs.Course.CourseName,
                        cs.Course.Department,
                        cs.Course.Credit
                    }
                })
                .ToListAsync();

            if (secimler == null || !secimler.Any())
            {
                return NotFound(new { Mesaj = "Bu öğrenciye ait ders seçimi bulunamadı." });
            }

            return Ok(secimler);
        }

        // POST: api/CourseSelections
        [HttpPost]
        public async Task<ActionResult<CourseSelection>> DersSecimiEkle(CourseSelection courseSelection)
        {
            _context.CourseSelection.Add(courseSelection);
            await _context.SaveChangesAsync();

            return CreatedAtAction("OgrenciDersSecimleriniGetir", new { studentId = courseSelection.StudentID }, courseSelection);
        }

        // DELETE: api/CourseSelections/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DersSecimiSil(int id)
        {
            var courseSelection = await _context.CourseSelection.FindAsync(id);
            if (courseSelection == null)
            {
                return NotFound(new { Mesaj = "Silinecek ders seçimi bulunamadı." });
            }

            _context.CourseSelection.Remove(courseSelection);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // GET: api/CourseSelections/History
        [HttpGet("History")]
        public async Task<ActionResult<IEnumerable<object>>> DersSecimGecmisiniGetir()
        {
            var secimGecmisi = await _context.CourseSelectionHistory
                .Include(csh => csh.Student)
                .Select(csh => new
                {
                    csh.StudentID,
                    csh.SelectionDate,
                    OgrenciAdi = csh.Student.FirstName,
                    OgrenciSoyadi = csh.Student.LastName
                })
                .ToListAsync();

            return Ok(secimGecmisi);
        }

        // GET: api/CourseSelections/History/5
        [HttpGet("History/{id}")]
        public async Task<ActionResult<object>> OgrenciDersSecimGecmisi(int id)
        {
            var secimGecmisi = await _context.CourseSelectionHistory
                .Include(csh => csh.Student)
                .Where(csh => csh.StudentID == id)
                .Select(csh => new
                {
                    csh.StudentID,
                    csh.SelectionDate,
                    OgrenciAdi = csh.Student.FirstName,
                    OgrenciSoyadi = csh.Student.LastName
                })
                .FirstOrDefaultAsync();

            if (secimGecmisi == null)
            {
                return NotFound(new { Mesaj = "Seçim geçmişi bulunamadı." });
            }

            return Ok(secimGecmisi);
        }

        // GET: api/CourseSelections/NonConfirmed
        [HttpGet("NonConfirmed")]
        public async Task<ActionResult<IEnumerable<object>>> OnaylanmayanDersSecimleriGetir()
        {
            var onaylanmayanSecimler = await _context.NonConfirmedSelections
                .Include(ns => ns.Student)
                .Include(ns => ns.Course)
                .Select(ns => new
                {
                    ns.Id,
                    ns.Student.StudentID,
                    ns.Student.FirstName,
                    ns.Student.LastName,
                    ns.Course.CourseName,
                    ns.Course.CourseID
                })
                .ToListAsync();

            return Ok(onaylanmayanSecimler);
        }

        // GET: api/CourseSelections/NonConfirmed/Advisor/{advisorId}
        [HttpGet("NonConfirmed/Advisor/{advisorId}")]
        public async Task<ActionResult<IEnumerable<object>>> DanismanaGoreOnaylanmayanSecimler(int advisorId)
        {
            var onaylanmayanSecimler = await _context.NonConfirmedSelections
                .Include(ns => ns.Student)
                .Include(ns => ns.Course)
                .Where(ns => ns.Student.AdvisorID == advisorId)
                .Select(ns => new
                {
                    ns.Id,
                    ns.Student.StudentID,
                    ns.Student.FirstName,
                    ns.Student.LastName,
                    ns.Course.CourseName,
                    ns.Course.CourseID
                })
                .ToListAsync();

            return Ok(onaylanmayanSecimler);
        }

        // GET: api/CourseSelections/NonConfirmed/Student/{studentId}
        [HttpGet("NonConfirmed/Student/{studentId}")]
        public async Task<IActionResult> OgrenciyeGoreOnaylanmayanSecimler(int studentId)
        {
            var onaylanmayanSecimler = await _context.NonConfirmedSelections
                .Include(ns => ns.Course)
                .Where(ns => ns.StudentId == studentId)
                .Select(ns => new
                {
                    ns.Id,
                    ns.StudentId,
                    OgrenciAdi = ns.Student.FirstName,
                    OgrenciSoyadi = ns.Student.LastName,
                    DersAdi = ns.Course.CourseName,
                    DersID = ns.Course.CourseID
                })
                .ToListAsync();

            if (onaylanmayanSecimler == null || !onaylanmayanSecimler.Any())
            {
                return NotFound(new { Mesaj = "Bu öğrenciye ait onaylanmayan ders seçimi bulunamadı." });
            }

            return Ok(onaylanmayanSecimler);
        }

        // POST: api/CourseSelections/NonConfirmed
        [HttpPost("NonConfirmed")]
        public async Task<ActionResult<NonConfirmedSelections>> OnaylanmayanSecimEkle(NonConfirmedSelections nonConfirmedSelections)
        {
            _context.NonConfirmedSelections.Add(nonConfirmedSelections);
            await _context.SaveChangesAsync();

            return CreatedAtAction("OnaylanmayanDersSecimleriGetir", new { id = nonConfirmedSelections.Id }, nonConfirmedSelections);
        }

        // DELETE: api/CourseSelections/NonConfirmed/5
        [HttpDelete("NonConfirmed/{id}")]
        public async Task<IActionResult> OnaylanmayanSecimSil(int id)
        {
            var onaylanmayanSecim = await _context.NonConfirmedSelections.FindAsync(id);
            if (onaylanmayanSecim == null)
            {
                return NotFound(new { Mesaj = "Silinecek onaylanmayan ders seçimi bulunamadı." });
            }

            _context.NonConfirmedSelections.Remove(onaylanmayanSecim);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
