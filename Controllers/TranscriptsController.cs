using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BilgiYonetimSistemi.Data;
using BilgiYonetimSistemi.Models;

namespace BilgiYonetimSistemi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TranscriptsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public TranscriptsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/Transcripts
        [HttpGet]
        public async Task<IActionResult> TumTranskriptleriGetir()
        {
            var transkriptler = await _context.Transcripts
                .ToListAsync();

            if (!transkriptler.Any())
            {
                return NotFound(new { Mesaj = "Herhangi bir transkript kaydı bulunamadı." });
            }

            return Ok(transkriptler);
        }

        // GET: api/Transcripts/5
        [HttpGet("{id}")]
        public async Task<IActionResult> TranskriptGetir(int id)
        {
            var transkript = await _context.Transcripts
                .FirstOrDefaultAsync(t => t.TranscriptID == id);

            if (transkript == null)
            {
                return NotFound(new { Mesaj = $"ID {id} ile eşleşen transkript bulunamadı." });
            }

            return Ok(transkript);
        }

        // POST: api/Transcripts
        [HttpPost]
        public async Task<IActionResult> TranskriptEkle([FromBody] Transcripts yeniTranskript)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.Transcripts.Add(yeniTranskript);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(TranskriptGetir), new { id = yeniTranskript.TranscriptID }, yeniTranskript);
        }

        // PUT: api/Transcripts/5
        [HttpPut("{id}")]
        public async Task<IActionResult> TranskriptGuncelle(int id, [FromBody] Transcripts guncelTranskript)
        {
            if (id != guncelTranskript.TranscriptID)
            {
                return BadRequest(new { Mesaj = "Transkript ID eşleşmiyor." });
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.Entry(guncelTranskript).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await TranskriptVarMi(id))
                {
                    return NotFound(new { Mesaj = $"ID {id} ile transkript kaydı bulunamadı." });
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // DELETE: api/Transcripts/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> TranskriptSil(int id)
        {
            var transkript = await _context.Transcripts.FindAsync(id);
            if (transkript == null)
            {
                return NotFound(new { Mesaj = $"ID {id} ile transkript bulunamadı." });
            }

            _context.Transcripts.Remove(transkript);
            await _context.SaveChangesAsync();

            return Ok(new { Mesaj = $"ID {id} numaralı transkript başarıyla silindi." });
        }

        private async Task<bool> TranskriptVarMi(int id)
        {
            return await _context.Transcripts.AnyAsync(t => t.TranscriptID == id);
        }
    }
}
