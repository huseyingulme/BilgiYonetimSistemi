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

        // GET: api/Advisors
        [HttpGet]
        public async Task<IActionResult> GetAdvisor()
        {
            var advisors = await _context.Advisors
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
                    }).ToList() // Öğrencileri seçiyoruz
                })
                .ToListAsync();

            return Ok(advisors);
        }


        // GET: api/Advisors/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetAdvisors(int id)
        {
            var advisor = await _context.Advisors
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

            if (advisor == null)
            {
                return NotFound(); // Eğer danışman bulunamazsa 404 döner
            }

            return Ok(advisor);
        }


        // PUT: api/Advisors/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutAdvisors(int id, Advisors advisors)
        {
            if (id != advisors.AdvisorID)
            {
                return BadRequest();
            }

            _context.Entry(advisors).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!AdvisorsExists(id))
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

        // POST: api/Advisors
        [HttpPost]
        public async Task<ActionResult<Advisors>> PostAdvisors(Advisors advisors)
        {
            _context.Advisors.Add(advisors);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetAdvisors", new { id = advisors.AdvisorID }, advisors);
        }

        // DELETE: api/Advisors/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAdvisors(int id)
        {
            var advisors = await _context.Advisors.FindAsync(id);
            if (advisors == null)
            {
                return NotFound();
            }

            _context.Advisors.Remove(advisors);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool AdvisorsExists(int id)
        {
            return _context.Advisors.Any(e => e.AdvisorID == id);
        }
    }
}