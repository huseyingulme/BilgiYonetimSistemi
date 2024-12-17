using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BilgiYonetimSistemi.Data;
using BilgiYonetimSistemi.Models;

namespace BilgiYonetimSistemi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CoursesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public CoursesController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> TumDersleriGetir()
        {
            var courses = await _context.Courses
                .Include(c => c.CourseSelection)  // CourseSelection ilişkisini dahil ediyoruz
                .Select(c => new
                {
                    c.CourseID,
                    c.CourseCode,
                    c.CourseName,
                    c.IsMandatory,
                    c.Credit,
                    c.Department,
                    Students = c.CourseSelection.Select(cs => new
                    {
                        cs.StudentID,
                        cs.SelectionDate
                    }).ToList()  // Öğrencilerin sadece ID ve seçim tarihlerini alıyoruz
                })
                .ToListAsync();

            if (!courses.Any())
            {
                return NotFound(new { Message = "Hiç ders bulunamadı." });
            }

            return Ok(courses);
        }


        // GET: api/Courses/5
        [HttpGet("{id}")]
        public async Task<IActionResult> DersGetir(int id)
        {
            var course = await _context.Courses
                .Include(c => c.CourseSelection)  // Ders seçimleri ilişkisini dahil ediyoruz
                .Where(c => c.CourseID == id)
                .Select(c => new
                {
                    c.CourseID,
                    c.CourseCode,
                    c.CourseName,
                    c.IsMandatory,
                    c.Credit,
                    c.Department,
                    Students = c.CourseSelection.Select(cs => new
                    {
                        cs.StudentID,
                        cs.SelectionDate
                    }).ToList()  // Öğrencilerin sadece ID ve seçim tarihlerini alıyoruz
                })
                .FirstOrDefaultAsync();

            if (course == null)
            {
                return NotFound(new { Message = $"ID {id} ile eşleşen ders bulunamadı." });
            }

            return Ok(course);
        }

        // PUT: api/Courses/5
        [HttpPut("{id}")]
        public async Task<IActionResult> DersGuncelle(int id, [FromBody] Course course)
        {
            if (id != course.CourseID)
            {
                return BadRequest(new { Message = "Ders ID'si eşleşmiyor." });
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.Entry(course).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await DersVarMi(id))
                {
                    return NotFound(new { Message = $"ID {id} ile ders bulunamadı." });
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Courses
        [HttpPost]
        public async Task<IActionResult> DersEkle([FromBody] Course course)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.Courses.Add(course);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(DersGetir), new { id = course.CourseID }, course);
        }

        // DELETE: api/Courses/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DersSil(int id)
        {
            var course = await _context.Courses.FindAsync(id);
            if (course == null)
            {
                return NotFound(new { Message = $"ID {id} ile ders bulunamadı." });
            }

            _context.Courses.Remove(course);
            await _context.SaveChangesAsync();

            return Ok(new { Message = $"Ders ID {id} başarıyla silindi." });
        }

        private async Task<bool> DersVarMi(int id)
        {
            return await _context.Courses.AnyAsync(e => e.CourseID == id);
        }
    }
}
