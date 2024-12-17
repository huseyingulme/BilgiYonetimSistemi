using BilgiYonetimSistemi.Data;
using BilgiYonetimSistemi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BilgiYonetimSistemi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CourseCapacityController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public CourseCapacityController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<CourseWithCapacityModel>>> GetAllCourseCapacities()
        {
            var courseCapacities = await _context.CourseCapacity
                .Include(cc => cc.Course)
                .Select(cc => new CourseWithCapacityModel
                {
                    CourseID = cc.CourseID,
                    CourseName = cc.Course.CourseName,
                    CourseCode = cc.Course.CourseCode,
                    Credit = cc.Course.Credit,
                    Capacity = cc.Capacity.ToString(),      
                    RemainingCapacity = cc.RemainingCapacity.ToString()  
                })
                .ToListAsync();

            if (courseCapacities.Count == 0)
            {
                return NoContent();
            }

            return Ok(courseCapacities);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetCourseCapacityById(int id)
        {
            var courseCapacity = await _context.CourseCapacity
                .Where(cc => cc.CourseID == id)
                .Select(cc => new CourseWithCapacityModel
                {
                    CourseID = cc.CourseID,
                    CourseName = cc.Course.CourseName,
                    CourseCode = cc.Course.CourseCode,
                    Credit = cc.Course.Credit,
                    Capacity = cc.Capacity.ToString(),
                    RemainingCapacity = cc.RemainingCapacity.ToString()
                })
                .FirstOrDefaultAsync();

            if (courseCapacity == null)
            {
                return NotFound(new { Message = "Ders kapasitesi bulunamadı." });
            }

            return Ok(courseCapacity);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCourseCapacity(int id, CourseCapacity updatedCourseCapacity)
        {
            if (id != updatedCourseCapacity.CourseID)
            {
                return BadRequest(new { Message = "Ders ID uyumsuzluğu." });
            }

            _context.Entry(updatedCourseCapacity).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CourseCapacityExists(id))
                {
                    return NotFound(new { Message = "Ders kapasitesi bulunamadı." });
                }
                throw;
            }

            return NoContent();
        }

        [HttpPost]
        public async Task<ActionResult<CourseCapacity>> CreateCourseCapacity(CourseCapacity courseCapacity)
        {
            _context.CourseCapacity.Add(courseCapacity);

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (CourseCapacityExists(courseCapacity.CourseID))
                {
                    return Conflict(new { Message = "Ders kapasitesi zaten mevcut." });
                }
                throw;
            }

            return CreatedAtAction("GetCourseCapacityById", new { id = courseCapacity.CourseID }, courseCapacity);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCourseCapacity(int id)
        {
            var courseCapacity = await _context.CourseCapacity.FindAsync(id);

            if (courseCapacity == null)
            {
                return NotFound(new { Message = "Ders kapasitesi bulunamadı." });
            }

            _context.CourseCapacity.Remove(courseCapacity);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpPatch("{courseId}")]
        public async Task<IActionResult> UpdateRemainingCapacity(int courseId)
        {
            var courseCapacity = await _context.CourseCapacity.FindAsync(courseId);

            if (courseCapacity == null)
            {
                return NotFound(new { Message = "Ders bulunamadı." });
            }

            if (courseCapacity.RemainingCapacity > 0)
            {
                courseCapacity.RemainingCapacity--;
                await _context.SaveChangesAsync();
                return Ok(courseCapacity);
            }

            return BadRequest(new { Message = "Kontenjan dolmuş." });
        }

        private bool CourseCapacityExists(int id)
        {
            return _context.CourseCapacity.Any(e => e.CourseID == id);
        }
    }
}
