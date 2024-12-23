using BilgiYonetimSistemi.Data;
using BilgiYonetimSistemi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BilgiYonetimSistemi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StudentCourseSelectionsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public StudentCourseSelectionsController(ApplicationDbContext context)
        {
            _context = context;
        }
 
        [HttpGet]
        public async Task<ActionResult<IEnumerable<StudentCourseSelections>>> GetStudentCourseSelections()
        {
            var studentsWithCourses = await _context.Students
                .Include(s => s.StudentCourseSelections)
                    .ThenInclude(sc => sc.Course)  
                .Select(s => new
                {
                    s.StudentID,
                    s.FirstName,
                    s.LastName,
                    Courses = s.StudentCourseSelections.Select(sc => new
                    {
                        sc.CourseID,
                        sc.SelectionDate,
                        CourseName = sc.Course.CourseName
                    }).ToList()
                })
                .ToListAsync();
  
            return Ok(studentsWithCourses);
        }
         
        [HttpGet("{studentId}")]
        public IActionResult GetStudentCourseSelections(int studentId)
        {
 
            var selections = _context.StudentCourseSelections
               .Where(s => s.StudentID == studentId)
               .Select(s => new
               {
                   s.Student.StudentID,
                   Course = new
                   {
                       s.Course.CourseCode,
                       s.Course.CourseName,
                       s.Course.Department,
                       s.Course.Credit
                   }
               })
               .ToList();

            return Ok(selections);
        }

          [HttpPut("{id}")]
        public async Task<IActionResult> PutStudentCourseSelections(int id, StudentCourseSelections studentCourseSelections)
        {
            if (id != studentCourseSelections.SelectionID)
            {
                return BadRequest();
            }

            _context.Entry(studentCourseSelections).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!StudentCourseSelectionsExists(id))
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
        public async Task<ActionResult<StudentCourseSelections>> PostStudentCourseSelections(StudentCourseSelections studentCourseSelections)
        {
            _context.StudentCourseSelections.Add(studentCourseSelections);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetStudentCourseSelections", new { id = studentCourseSelections.SelectionID }, studentCourseSelections);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteStudentCourseSelections(int id)
        {
            var studentCourseSelections = await _context.StudentCourseSelections.FindAsync(id);
            if (studentCourseSelections == null)
            {
                return NotFound();
            }

            _context.StudentCourseSelections.Remove(studentCourseSelections);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool StudentCourseSelectionsExists(int id)
        {
            return _context.StudentCourseSelections.Any(e => e.SelectionID == id);
        }
    }
}
