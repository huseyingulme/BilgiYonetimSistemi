using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BilgiYonetimSistemi.Data;
using BilgiYonetimSistemi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BilgiYonetimSistemi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CourseSelectionHistoriesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public CourseSelectionHistoriesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/CourseSelectionHistories
        [HttpGet]
        public async Task<ActionResult<IEnumerable<object>>> TumSecimGecmisiniGetir()
        {
            var gecmisler = await _context.CourseSelectionHistory
                .Include(csh => csh.Student)
                .Select(csh => new
                {
                    csh.StudentID,
                    Tarih = csh.SelectionDate,
                    OgrenciAdi = $"{csh.Student.FirstName} {csh.Student.LastName}"
                })
                .ToListAsync();

            return Ok(gecmisler);
        }

        // GET: api/CourseSelectionHistories/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<object>> OgrenciGecmisiGetir(int id)
        {
            var gecmis = await _context.CourseSelectionHistory
                .Include(csh => csh.Student)
                .Where(csh => csh.StudentID == id)
                .Select(csh => new
                {
                    csh.StudentID,
                    Tarih = csh.SelectionDate,
                    OgrenciAdi = $"{csh.Student.FirstName} {csh.Student.LastName}"
                })
                .FirstOrDefaultAsync();

            if (gecmis == null)
            {
                return NotFound(new { Mesaj = "Öğrenciye ait seçim geçmişi bulunamadı." });
            }

            return Ok(gecmis);
        }

        // POST: api/CourseSelectionHistories
        [HttpPost]
        public async Task<ActionResult<CourseSelectionHistory>> SecimGecmisiEkle(CourseSelectionHistory gecmis)
        {
            if (_context.CourseSelectionHistory.Any(csh => csh.StudentID == gecmis.StudentID))
            {
                return Conflict(new { Mesaj = "Bu öğrenci için zaten bir seçim geçmişi mevcut." });
            }

            _context.CourseSelectionHistory.Add(gecmis);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(OgrenciGecmisiGetir), new { id = gecmis.StudentID }, gecmis);
        }

        // PUT: api/CourseSelectionHistories/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> SecimGecmisiGuncelle(int id, CourseSelectionHistory gecmis)
        {
            if (id != gecmis.StudentID)
            {
                return BadRequest(new { Mesaj = "ID ile öğrenci ID'si eşleşmiyor." });
            }

            _context.Entry(gecmis).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!SecimGecmisiVarMi(id))
                {
                    return NotFound(new { Mesaj = "Seçim geçmişi bulunamadı." });
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // DELETE: api/CourseSelectionHistories/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> SecimGecmisiSil(int id)
        {
            var gecmis = await _context.CourseSelectionHistory.FindAsync(id);
            if (gecmis == null)
            {
                return NotFound(new { Mesaj = "Silinecek seçim geçmişi bulunamadı." });
            }

            _context.CourseSelectionHistory.Remove(gecmis);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool SecimGecmisiVarMi(int id)
        {
            return _context.CourseSelectionHistory.Any(csh => csh.StudentID == id);
        }
    }
}
