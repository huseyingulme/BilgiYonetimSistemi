using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BilgiYonetimSistemi.Data;
using BilgiYonetimSistemi.Models;

namespace BilgiYonetimSistemi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public UsersController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/Users
        [HttpGet]
        public async Task<IActionResult> TumKullanicilariGetir()
        {
            var users = await _context.Users.ToListAsync();
            if (users == null || !users.Any())
            {
                return NotFound(new { Message = "Herhangi bir kullanıcı bulunamadı." });
            }

            return Ok(users);
        }

        // GET: api/Users/5
        [HttpGet("{id}")]
        public async Task<IActionResult> KullaniciGetir(int id)
        {
            var user = await _context.Users.FindAsync(id);

            if (user == null)
            {
                return NotFound(new { Message = $"ID {id} ile eşleşen kullanıcı bulunamadı." });
            }

            return Ok(user);
        }

        // PUT: api/Users/5
        [HttpPut("{id}")]
        public async Task<IActionResult> KullaniciGuncelle(int id, [FromBody] Users user)
        {
            if (id != user.UserID)
            {
                return BadRequest(new { Message = "Kullanıcı ID'si eşleşmiyor." });
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.Entry(user).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await KullaniciVarMi(id))
                {
                    return NotFound(new { Message = $"ID {id} ile kullanıcı bulunamadı." });
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Users
        [HttpPost]
        public async Task<IActionResult> KullaniciEkle([FromBody] Users user)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(KullaniciGetir), new { id = user.UserID }, user);
        }

        // DELETE: api/Users/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> KullaniciSil(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound(new { Message = $"ID {id} ile kullanıcı bulunamadı." });
            }

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();

            return Ok(new { Message = $"Kullanıcı {id} başarıyla silindi." });
        }

        private async Task<bool> KullaniciVarMi(int id)
        {
            return await _context.Users.AnyAsync(e => e.UserID == id);
        }
    }
}
