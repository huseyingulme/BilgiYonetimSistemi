using BilgiYonetimSistemi.Data;
using BilgiYonetimSistemi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;

namespace BilgiYonetimSistemi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NonConfirmedSelectionsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public NonConfirmedSelectionsController(ApplicationDbContext context)
        {
            _context = context;
        }
 
        [HttpGet]
        public async Task<ActionResult<IEnumerable<NonConfirmedSelections>>> GetNonConfirmedSelections()
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
 
        [HttpGet("{id}")]
        public async Task<ActionResult<NonConfirmedSelections>> GetNonConfirmedSelections(int id)
        {
            var nonConfirmedSelections = await _context.NonConfirmedSelections.FindAsync(id);

            if (nonConfirmedSelections == null)
            {
                return NotFound();
            }

            return nonConfirmedSelections;
        }
 
        [HttpPut("{id}")]
        public async Task<IActionResult> PutNonConfirmedSelections(int id, NonConfirmedSelections nonConfirmedSelections)
        {
            if (id != nonConfirmedSelections.Id)
            {
                return BadRequest();
            }

            _context.Entry(nonConfirmedSelections).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!NonConfirmedSelectionsExists(id))
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
        public async Task<ActionResult<NonConfirmedSelections>> PostNonConfirmedSelections(NonConfirmedSelections nonConfirmedSelections)
        {
            _context.NonConfirmedSelections.Add(nonConfirmedSelections);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetNonConfirmedSelections", new { id = nonConfirmedSelections.Id }, nonConfirmedSelections);
        }
 
        [HttpDelete("{id}")]
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

        private bool NonConfirmedSelectionsExists(int id)
        {
            return _context.NonConfirmedSelections.Any(e => e.Id == id);
        }
 
        [HttpGet("Advisor/{advisorId}")]
        public async Task<ActionResult<IEnumerable<NonConfirmedSelections>>> GetNonConfirmedSelectionsForAdvisor(int advisorId)
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
  
    }

}
