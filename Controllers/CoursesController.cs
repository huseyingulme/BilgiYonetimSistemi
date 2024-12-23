using BilgiYonetimSistemi.Data;
using BilgiYonetimSistemi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

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
        public async Task<ActionResult<IEnumerable<object>>> GetCourse()
        {
            var courses = await _context.Courses
                .Include(c => c.StudentCourseSelections)    
                .Select(c => new
                {
                    c.CourseID,
                    c.CourseCode,
                    c.CourseName,
                    c.IsMandatory,
                    c.Credit,
                    c.Department,
                    StudentCourseSelections = c.StudentCourseSelections.Select(sc => new
                    {
                        sc.StudentID,
                        sc.SelectionDate
                    }).ToList()   
                })
                .ToListAsync();

            return Ok(courses);
        }
 
        [HttpGet("{id}")]
        public async Task<IActionResult> GetCourse(int id)
        {
            var course = await _context.Courses
                .Include(c => c.StudentCourseSelections)   
                .Where(c => c.CourseID == id)
                .Select(c => new
                {
                    c.CourseID,
                    c.CourseCode,
                    c.CourseName,
                    c.IsMandatory,
                    c.Credit,
                    c.Department,
                    StudentCourseSelections = c.StudentCourseSelections.Select(sc => new
                    {
                        sc.StudentID,
                        sc.SelectionDate
                    }).ToList()   
                })
                .FirstOrDefaultAsync();
 
            return Ok(course);
        }
 
        [HttpPut("{id}")]
        public async Task<IActionResult> PutCourse(int id, Course course)
        {
            if (id != course.CourseID)
            {
                return BadRequest();
            }

            _context.Entry(course).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CourseExists(id))
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
        public async Task<ActionResult<Course>> PostCourse(Course course)
        {
            _context.Courses.Add(course);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetCourse", new { id = course.CourseID }, course);
        }
 
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCourse(int id)
        {
            var course = await _context.Courses.FindAsync(id);
            if (course == null)
            {
                return NotFound();
            }

            _context.Courses.Remove(course);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool CourseExists(int id)
        {
            return _context.Courses.Any(e => e.CourseID == id);
        }
    }
}