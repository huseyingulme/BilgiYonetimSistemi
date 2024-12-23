using BilgiYonetimSistemi.Data;
using BilgiYonetimSistemi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

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
 
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Student>>> GetStudent()
        {
            var studentsWithAdvisorAndCourses = await _context.Students
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
                    Courses = s.StudentCourseSelections.Select(sc => new
                    {
                        sc.CourseID,
                        sc.Course.CourseName, 
                        sc.SelectionDate
                    }).ToList()
                })
                .ToListAsync();

            return Ok(studentsWithAdvisorAndCourses);
        }
 
        [HttpGet("{id}")]
        public async Task<ActionResult<Student>> GetStudent(int id)
        {
            var studentWithAdvisorAndCourses = await _context.Students
                .Where(s => s.StudentID == id)
                .Select(s => new
                {
                    s.StudentID,
                    s.FirstName,
                    s.LastName,
                    s.Email,
                    Advisor = new
                    {
                        s.AdvisorID, 
                        s.Advisors.FullName,  
                        s.Advisors.Title,  
                        s.Advisors.Department  
                    },
                    Courses = s.StudentCourseSelections.Select(sc => new
                    {
                        sc.CourseID,
                        sc.Course.CourseName, 
                        sc.SelectionDate
                    }).ToList()
                })
                .FirstOrDefaultAsync();
 
            return Ok(studentWithAdvisorAndCourses);  
        }
 
        [HttpPut("{id}")]
        public async Task<IActionResult> PutStudent(int id, Student student)
        {
            if (id != student.StudentID)
            {
                return BadRequest();
            }

            _context.Entry(student).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!StudentExists(id))
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
        public async Task<ActionResult<Student>> PostStudent(Student student)
        {
            _context.Students.Add(student);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetStudent", new { id = student.StudentID }, student);
        }
  
        private bool StudentExists(int id)
        {
            return _context.Students.Any(e => e.StudentID == id);
        }
    }
}