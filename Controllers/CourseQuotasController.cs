using BilgiYonetimSistemi.Data;
using BilgiYonetimSistemi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BilgiYonetimSistemi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CourseQuotasController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public CourseQuotasController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/CourseQuotas
        [HttpGet]
        public async Task<ActionResult<IEnumerable<object>>> GetCourseQuotas()
        {
            var courseQuotas = await _context.CourseQuotas
                .Include(cq => cq.Course) // Course ilişkisini dahil ediyoruz
                .Select(cq => new
                {
                    cq.CourseID,
                    cq.Quota,
                    cq.RemainingQuota,
                    CourseName = cq.Course.CourseName // Sadece CourseName alanını dahil ediyoruz
                })
                .ToListAsync();

            return Ok(courseQuotas);
        }


        // GET: api/CourseQuotas/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetCourseQuota(int id)
        {
            var courseQuota = await _context.CourseQuotas
                .Where(cq => cq.CourseID == id)
                .Select(cq => new
                {
                    cq.CourseID,
                    cq.Quota,
                    cq.RemainingQuota,
                    CourseName = cq.Course.CourseName // Sadece CourseName alanı
                })
                .FirstOrDefaultAsync();

            if (courseQuota == null)
            {
                return NotFound(); // Eğer courseQuota bulunamazsa 404 döner
            }

            return Ok(courseQuota);
        }


        // PUT: api/CourseQuotas/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutCourseQuota(int id, CourseQuotas courseQuota)
        {
            if (id != courseQuota.CourseID)
            {
                return BadRequest();
            }

            _context.Entry(courseQuota).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CourseQuotaExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/CourseQuotas
        [HttpPost]
        public async Task<ActionResult<CourseQuotas>> PostCourseQuota(CourseQuotas courseQuota)
        {
            _context.CourseQuotas.Add(courseQuota);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (CourseQuotaExists(courseQuota.CourseID))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtAction("GetCourseQuota", new { id = courseQuota.CourseID }, courseQuota);
        }

        // DELETE: api/CourseQuotas/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCourseQuota(int id)
        {
            var courseQuota = await _context.CourseQuotas.FindAsync(id);
            if (courseQuota == null)
            {
                return NotFound();
            }

            _context.CourseQuotas.Remove(courseQuota);
            await _context.SaveChangesAsync();

            return NoContent();
        }



        [HttpPatch("coursequotas/{courseId}")]
        public async Task<IActionResult> UpdateCourseQuota(int courseId)
        {
            var courseQuota = await _context.CourseQuotas.FindAsync(courseId);

            if (courseQuota == null)
            {
                return NotFound("Ders bulunamadı.");
            }

            // Kontenjanı bir azalt
            if (courseQuota.RemainingQuota > 0)
            {
                courseQuota.RemainingQuota--;
                await _context.SaveChangesAsync(); // Veritabanına kaydediyoruz
                return Ok(courseQuota); // Güncellenmiş bilgiyi geri gönderiyoruz
            }

            return BadRequest("Kontenjan dolmuş.");
        }


        private bool CourseQuotaExists(int id)
        {
            return _context.CourseQuotas.Any(e => e.CourseID == id);
        }
    }
}