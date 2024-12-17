using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BilgiYonetimSistemi.Data;
using BilgiYonetimSistemi.Models;

namespace BilgiYonetimSistemi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StudentsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public StudentsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/Students
        [HttpGet]
        public async Task<IActionResult> TumOgrencileriGetir()
        {
            var studentsWithAdvisorAndCourses = await _context.Students
                .Include(s => s.Advisors) // Danışmanı dahil et
                .Include(s => s.CourseSelection) // Seçilen dersleri dahil et
                .ThenInclude(cs => cs.Course) // Kurs bilgilerini dahil et
                .Select(s => new
                {
                    s.StudentID,
                    s.FirstName,
                    s.LastName,
                    s.Email,
                    Advisor = new
                    {
                        s.Advisors.FullName,
                        s.Advisors.Title,
                        s.Advisors.Department
                    },
                    Courses = s.CourseSelection.Select(cs => new
                    {
                        cs.CourseID,
                        cs.Course.CourseName,
                        cs.SelectionDate
                    }).ToList()
                })
                .ToListAsync();

            if (studentsWithAdvisorAndCourses.Count == 0)
            {
                return NotFound(new { Message = "Herhangi bir öğrenci bulunamadı." });
            }

            return Ok(studentsWithAdvisorAndCourses);
        }

        // GET: api/Students/5
        [HttpGet("{id}")]
        public async Task<IActionResult> OgrenciGetir(int id)
        {
            var studentWithAdvisorAndCourses = await _context.Students
                .Where(s => s.StudentID == id)
                .Include(s => s.Advisors)
                .Include(s => s.CourseSelection)
                .ThenInclude(cs => cs.Course)
                .Select(s => new
                {
                    s.StudentID,
                    s.FirstName,
                    s.LastName,
                    s.Email,
                    Advisor = new
                    {
                        s.Advisors.FullName,
                        s.Advisors.Title,
                        s.Advisors.Department
                    },
                    Courses = s.CourseSelection.Select(cs => new
                    {
                        cs.CourseID,
                        cs.Course.CourseName,
                        cs.SelectionDate
                    }).ToList()
                })
                .FirstOrDefaultAsync();

            if (studentWithAdvisorAndCourses == null)
            {
                return NotFound(new { Message = $"ID {id} ile eşleşen öğrenci bulunamadı." });
            }

            return Ok(studentWithAdvisorAndCourses);
        }

        // PUT: api/Students/5
        [HttpPut("{id}")]
        public async Task<IActionResult> OgrenciGuncelle(int id, [FromBody] Student guncelOgrenci)
        {
            if (id != guncelOgrenci.StudentID)
            {
                return BadRequest(new { Message = "Öğrenci ID'si eşleşmiyor." });
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.Entry(guncelOgrenci).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await OgrenciVarMi(id))
                {
                    return NotFound(new { Message = $"ID {id} ile öğrenci bulunamadı." });
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Students
        [HttpPost]
        public async Task<IActionResult> OgrenciEkle([FromBody] Student yeniOgrenci)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.Students.Add(yeniOgrenci);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(OgrenciGetir), new { id = yeniOgrenci.StudentID }, yeniOgrenci);
        }

        // DELETE: api/Students/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> OgrenciSil(int id)
        {
            var ogrenci = await _context.Students
                .Include(s => s.CourseSelection) // İlişkili ders seçimlerini dahil et
                .FirstOrDefaultAsync(s => s.StudentID == id);

            if (ogrenci == null)
            {
                return NotFound(new { Message = $"ID {id} ile öğrenci bulunamadı." });
            }

            // Önce ilişkili ders seçimlerini sil
            _context.CourseSelection.RemoveRange(ogrenci.CourseSelection);

            // Öğrenciyi sil
            _context.Students.Remove(ogrenci);
            await _context.SaveChangesAsync();

            return Ok(new { Message = $"Öğrenci {id} başarıyla silindi." });
        }

        private async Task<bool> OgrenciVarMi(int id)
        {
            return await _context.Students.AnyAsync(s => s.StudentID == id);
        }
    }
}
