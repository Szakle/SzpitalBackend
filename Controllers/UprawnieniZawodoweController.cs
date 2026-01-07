using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SzpitalnaKadra.Data;
using SzpitalnaKadra.Models;

namespace SzpitalnaKadra.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UprawnieniZawodoweController : ControllerBase
    {
        private readonly AppDbContext _context;

        public UprawnieniZawodoweController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet("osoba/{osobaId}")]
        public ActionResult<IEnumerable<UprawnieniZawodowe>> GetByOsobaId(int osobaId)
        {
            var uprawnienia = _context.UprawnieniZawodowe
                .Where(u => u.OsobaId == osobaId)
                .ToList();

            return Ok(uprawnienia);
        }

        [HttpGet("options/rodzaj")]
        public ActionResult<IEnumerable<string>> GetRodzajOptions()
        {
            var rodzaje = _context.UprawnieniZawodowe
                .Where(u => !string.IsNullOrEmpty(u.Rodzaj))
                .Select(u => u.Rodzaj)
                .Distinct()
                .OrderBy(r => r)
                .ToList();
            return Ok(rodzaje);
        }

        [HttpGet("options/npwz")]
        public ActionResult<IEnumerable<string>> GetNpwzOptions()
        {
            // Zwraca wszystkie PWZ z bazy, usuwając prefiks PWZ jeśli istnieje
            var npwz = _context.UprawnieniZawodowe
                .Where(u => !string.IsNullOrEmpty(u.NpwzIdRizh))
                .AsEnumerable()
                .Select(u => 
                {
                    // Usuwamy prefiks PWZ jeśli istnieje
                    return u.NpwzIdRizh.StartsWith("PWZ", System.StringComparison.OrdinalIgnoreCase) 
                        ? u.NpwzIdRizh.Substring(3) 
                        : u.NpwzIdRizh;
                })
                .Distinct()
                .OrderBy(n => n)
                .ToList();
            return Ok(npwz);
        }

        [HttpGet("options/organ")]
        public ActionResult<IEnumerable<string>> GetOrganOptions()
        {
            var organy = _context.UprawnieniZawodowe
                .Where(u => !string.IsNullOrEmpty(u.OrganRejestrujacy))
                .Select(u => u.OrganRejestrujacy)
                .Distinct()
                .OrderBy(o => o)
                .ToList();
            return Ok(organy);
        }

        [HttpGet("{id}")]
        public ActionResult<UprawnieniZawodowe> GetById(int id)
        {
            var uprawnienie = _context.UprawnieniZawodowe.Find(id);
            if (uprawnienie == null)
                return NotFound();
            
            return Ok(uprawnienie);
        }

        [HttpPost]
        public ActionResult<UprawnieniZawodowe> Create(UprawnieniZawodowe uprawnienie)
        {
            try
            {
                Console.WriteLine($"Otrzymane dane POST:");
                Console.WriteLine($"  OsobaId: {uprawnienie.OsobaId}");
                Console.WriteLine($"  Rodzaj: {uprawnienie.Rodzaj}");
                Console.WriteLine($"  NpwzIdRizh: {uprawnienie.NpwzIdRizh}");
                Console.WriteLine($"  OrganRejestrujacy: {uprawnienie.OrganRejestrujacy}");
                Console.WriteLine($"  DataUzyciaUprawnienia: {uprawnienie.DataUzyciaUprawnienia}");
                
                // Konwersja DateTime na UTC dla PostgreSQL
                if (uprawnienie.DataUzyciaUprawnienia.HasValue)
                {
                    uprawnienie.DataUzyciaUprawnienia = DateTime.SpecifyKind(uprawnienie.DataUzyciaUprawnienia.Value, DateTimeKind.Utc);
                }
                
                _context.UprawnieniZawodowe.Add(uprawnienie);
                _context.SaveChanges();
                return CreatedAtAction(nameof(GetById), new { id = uprawnienie.Id }, uprawnienie);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"BŁĄD podczas dodawania uprawnienia zawodowego: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
                if (ex.InnerException != null)
                {
                    Console.WriteLine($"Inner exception: {ex.InnerException.Message}");
                }
                return BadRequest(new { error = ex.Message, innerError = ex.InnerException?.Message });
            }
        }

        [HttpPut("{id}")]
        public ActionResult<UprawnieniZawodowe> Update(int id, UprawnieniZawodowe uprawnienie)
        {
            try
            {
                Console.WriteLine($"Otrzymane dane PUT dla ID {id}:");
                Console.WriteLine($"  OsobaId: {uprawnienie.OsobaId}");
                Console.WriteLine($"  Rodzaj: {uprawnienie.Rodzaj}");
                Console.WriteLine($"  NpwzIdRizh: {uprawnienie.NpwzIdRizh}");
                Console.WriteLine($"  OrganRejestrujacy: {uprawnienie.OrganRejestrujacy}");
                Console.WriteLine($"  DataUzyciaUprawnienia: {uprawnienie.DataUzyciaUprawnienia}");
                
                var existing = _context.UprawnieniZawodowe.Find(id);
                if (existing == null)
                    return NotFound();

                existing.Rodzaj = uprawnienie.Rodzaj;
                existing.NpwzIdRizh = uprawnienie.NpwzIdRizh;
                existing.OrganRejestrujacy = uprawnienie.OrganRejestrujacy;
                
                // Konwersja DateTime na UTC dla PostgreSQL
                if (uprawnienie.DataUzyciaUprawnienia.HasValue)
                {
                    existing.DataUzyciaUprawnienia = DateTime.SpecifyKind(uprawnienie.DataUzyciaUprawnienia.Value, DateTimeKind.Utc);
                }
                else
                {
                    existing.DataUzyciaUprawnienia = uprawnienie.DataUzyciaUprawnienia;
                }

                _context.SaveChanges();
                return Ok(existing);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"BŁĄD podczas aktualizacji uprawnienia zawodowego: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
                if (ex.InnerException != null)
                {
                    Console.WriteLine($"Inner exception: {ex.InnerException.Message}");
                }
                return BadRequest(new { error = ex.Message, innerError = ex.InnerException?.Message });
            }
        }

        [HttpDelete("{id}")]
        public ActionResult Delete(int id)
        {
            var uprawnienie = _context.UprawnieniZawodowe.Find(id);
            if (uprawnienie == null)
                return NotFound();

            _context.UprawnieniZawodowe.Remove(uprawnienie);
            _context.SaveChanges();
            return NoContent();
        }
    }
}
