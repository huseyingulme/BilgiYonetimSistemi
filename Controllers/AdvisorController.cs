using BilgiYonetimSistemi.Data;
using BilgiYonetimSistemi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
 
namespace BilgiYonetimSistemi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdvisorsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public AdvisorsController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetAdvisors()
        {
            var advisors = await GetAdvisorList();
            return Ok(advisors);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetAdvisorById(int id)
        {
            var advisor = await GetAdvisorByIdAsync(id);
            if (advisor == null)
            {
                return NotFound();
            }
            return Ok(advisor);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateAdvisor(int id, Advisors advisor)
        {
            if (id != advisor.AdvisorID)
            {
                return BadRequest();
            }

            _context.Entry(advisor).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!AdvisorExists(id))
                {
                    return NotFound();
                }
                throw;
            }

            return NoContent();
        }

        [HttpPost]
        public async Task<ActionResult<Advisors>> CreateAdvisor(Advisors advisor)
        {
            _context.Advisors.Add(advisor);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetAdvisorById), new { id = advisor.AdvisorID }, advisor);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAdvisor(int id)
        {
            var advisor = await _context.Advisors.FindAsync(id);
            if (advisor == null)
            {
                return NotFound();
            }

            _context.Advisors.Remove(advisor);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private async Task<object> GetAdvisorList()
        {
            return await _context.Advisors
                .Select(a => new
                {
                    a.AdvisorID,
                    a.FullName,
                    a.Title,
                    a.Department,
                    a.Email,
                    Students = a.Students.Select(s => new
                    {
                        s.StudentID,
                        s.FirstName,
                        s.LastName
                    }).ToList()
                })
                .ToListAsync();
        }

        private async Task<object> GetAdvisorByIdAsync(int id)
        {
            return await _context.Advisors
                .Where(a => a.AdvisorID == id)
                .Select(a => new
                {
                    a.AdvisorID,
                    a.FullName,
                    a.Title,
                    a.Department,
                    a.Email,
                    Students = a.Students.Select(s => new
                    {
                        s.StudentID,
                        s.FirstName,
                        s.LastName
                    }).ToList()
                })
                .FirstOrDefaultAsync();
        }

        private bool AdvisorExists(int id)
        {
            return _context.Advisors.Any(e => e.AdvisorID == id);
        }
    }
}
