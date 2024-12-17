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
        public async Task<ActionResult<IEnumerable<object>>> GetAllCourseSelections()
        {
            var studentsWithCourses = await _context.Students
                .Include(s => s.CourseSelection)
                    .ThenInclude(cs => cs.Course)
                .Select(s => new
                {
                    s.StudentID,
                    s.FirstName,
                    s.LastName,
                    Courses = s.CourseSelection.Select(cs => new
                    {
                        cs.CourseID,
                        cs.SelectionDate,
                        cs.Course.CourseName
                    }).ToList()
                })
                .ToListAsync();

            if (studentsWithCourses == null || !studentsWithCourses.Any())
            {
                return NotFound(new { Message = "No students or course selections found." });
            }

            return Ok(studentsWithCourses);
        }

        // GET: api/CourseSelections/Student/5
        [HttpGet("Student/{studentId}")]
        public async Task<ActionResult<IEnumerable<object>>> GetStudentCourseSelections(int studentId)
        {
            var selections = await _context.CourseSelection
                .Where(cs => cs.StudentID == studentId)
                .Select(cs => new
                {
                    cs.Student.StudentID,
                    Course = new
                    {
                        cs.Course.CourseCode,
                        cs.Course.CourseName,
                        cs.Course.Department,
                        cs.Course.Credit
                    }
                })
                .ToListAsync();

            if (selections == null || !selections.Any())
            {
                return NotFound("No course selections found for this student.");
            }

            return Ok(selections);
        }

        // POST: api/CourseSelections
        [HttpPost]
        public async Task<ActionResult<CourseSelection>> PostCourseSelection(CourseSelection courseSelection)
        {
            _context.CourseSelection.Add(courseSelection);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetStudentCourseSelections", new { studentId = courseSelection.StudentID }, courseSelection);
        }

        // DELETE: api/CourseSelections/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCourseSelection(int id)
        {
            var courseSelection = await _context.CourseSelection.FindAsync(id);
            if (courseSelection == null)
            {
                return NotFound();
            }

            _context.CourseSelection.Remove(courseSelection);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // GET: api/CourseSelections/History
        [HttpGet("History")]
        public async Task<ActionResult<IEnumerable<object>>> GetCourseSelectionHistory()
        {
            var courseSelectionHistory = await _context.CourseSelectionHistory
                .Include(csh => csh.Student)
                .Select(csh => new
                {
                    csh.StudentID,
                    csh.SelectionDate,
                    StudentName = csh.Student.FirstName,
                    StudentLastName = csh.Student.LastName
                })
                .ToListAsync();

            return Ok(courseSelectionHistory);
        }

        // GET: api/CourseSelections/History/5
        [HttpGet("History/{id}")]
        public async Task<ActionResult<object>> GetCourseSelectionHistoryByStudent(int id)
        {
            var courseSelectionHistory = await _context.CourseSelectionHistory
                .Include(csh => csh.Student)
                .Where(csh => csh.StudentID == id)
                .Select(csh => new
                {
                    csh.StudentID,
                    csh.SelectionDate,
                    StudentName = csh.Student.FirstName,
                    StudentLastName = csh.Student.LastName
                })
                .FirstOrDefaultAsync();

            if (courseSelectionHistory == null)
            {
                return NotFound();
            }

            return Ok(courseSelectionHistory);
        }

        // GET: api/CourseSelections/NonConfirmed
        [HttpGet("NonConfirmed")]
        public async Task<ActionResult<IEnumerable<object>>> GetNonConfirmedSelections()
        {
            var nonConfirmedSelections = await _context.NonConfirmedSelections
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

            return Ok(nonConfirmedSelections);
        }

        // GET: api/CourseSelections/NonConfirmed/Advisor/{advisorId}
        [HttpGet("NonConfirmed/Advisor/{advisorId}")]
        public async Task<ActionResult<IEnumerable<object>>> GetNonConfirmedSelectionsByAdvisor(int advisorId)
        {
            var nonConfirmedSelections = await _context.NonConfirmedSelections
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

            return Ok(nonConfirmedSelections);
        }

        // GET: api/CourseSelections/NonConfirmed/Student/{studentId}
        [HttpGet("NonConfirmed/Student/{studentId}")]
        public async Task<IActionResult> GetNonConfirmedSelectionsByStudent(int studentId)
        {
            var nonConfirmedSelections = await _context.NonConfirmedSelections
                .Include(ns => ns.Course)
                .Where(ns => ns.StudentId == studentId)
                .Select(ns => new
                {
                    ns.Id,
                    ns.StudentId,
                    FirstName = ns.Student.FirstName,
                    LastName = ns.Student.LastName,
                    CourseName = ns.Course.CourseName,
                    CourseID = ns.Course.CourseID
                })
                .ToListAsync();

            if (nonConfirmedSelections == null || !nonConfirmedSelections.Any())
            {
                return NotFound(new { message = "No non-confirmed course selections found for this student." });
            }

            return Ok(nonConfirmedSelections);
        }

        // POST: api/CourseSelections/NonConfirmed
        [HttpPost("NonConfirmed")]
        public async Task<ActionResult<NonConfirmedSelections>> PostNonConfirmedSelections(NonConfirmedSelections nonConfirmedSelections)
        {
            _context.NonConfirmedSelections.Add(nonConfirmedSelections);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetNonConfirmedSelections", new { id = nonConfirmedSelections.Id }, nonConfirmedSelections);
        }

        // DELETE: api/CourseSelections/NonConfirmed/5
        [HttpDelete("NonConfirmed/{id}")]
        public async Task<IActionResult> DeleteNonConfirmedSelections(int id)
        {
            var nonConfirmedSelections = await _context.NonConfirmedSelections.FindAsync(id);
            if (nonConfirmedSelections == null)
            {
                return NotFound();
            }

            _context.NonConfirmedSelections.Remove(nonConfirmedSelections);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
