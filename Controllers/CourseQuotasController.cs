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
 
        [HttpGet]
        public async Task<ActionResult<IEnumerable<object>>> GetCourseQuotas()
        {
            var courseQuotas = await _context.CourseQuotas
                .Include(cq => cq.Course)  
                .Select(cq => new
                {
                    cq.CourseId,
                    cq.Quota,
                    cq.RemainingQuota,
                    CourseName = cq.Course.CourseName  
                })
                .ToListAsync();

            return Ok(courseQuotas);
        }
 
        [HttpGet("{id}")]
        public async Task<IActionResult> GetCourseQuota(int id)
        {
            var courseQuota = await _context.CourseQuotas
                .Where(cq => cq.CourseId == id)
                .Select(cq => new
                {
                    cq.CourseId,
                    cq.Quota,
                    cq.RemainingQuota,
                    CourseName = cq.Course.CourseName  
                })
                .FirstOrDefaultAsync();
  
            return Ok(courseQuota);
        }
 
        [HttpPut("{id}")]
        public async Task<IActionResult> PutCourseQuota(int id, CourseQuotas courseQuota)
        {
            if (id != courseQuota.CourseId)
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
                if (CourseQuotaExists(courseQuota.CourseId))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtAction("GetCourseQuota", new { id = courseQuota.CourseId }, courseQuota);
        }
 
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
 
            if (courseQuota.RemainingQuota > 0)
            {
                courseQuota.RemainingQuota--;
                await _context.SaveChangesAsync(); 
                return Ok(courseQuota);  
            }

            return BadRequest("Kontenjan dolmuş.");
        }



        private bool CourseQuotaExists(int id)
        {
            return _context.CourseQuotas.Any(e => e.CourseId == id);
        }
    }
}