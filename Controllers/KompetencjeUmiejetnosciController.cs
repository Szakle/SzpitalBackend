using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SzpitalnaKadra.Data;
using SzpitalnaKadra.Models;
using System;

namespace SzpitalnaKadra.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class KompetencjeUmiejetnosciController : ControllerBase
    {
        private readonly AppDbContext _context;

        public KompetencjeUmiejetnosciController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet("osoba/{osobaId}")]
        public ActionResult<IEnumerable<KompetencjeUmiejetnosci>> GetByOsobaId(int osobaId)
        {
            var kompetencje = _context.KompetencjeUmiejetnosci
                .Where(k => k.OsobaId == osobaId)
                .ToList();

            return Ok(kompetencje);
        }

        [HttpGet("options/kod")]
        public ActionResult<IEnumerable<string>> GetKodyOptions()
        {
            var kody = _context.KompetencjeUmiejetnosci
                .Where(k => !string.IsNullOrEmpty(k.Kod))
                .Select(k => k.Kod)
                .Distinct()
                .OrderBy(k => k)
                .ToList();
            return Ok(kody);
        }

        [HttpGet("options/nazwa")]
        public ActionResult<IEnumerable<string>> GetNazwyOptions()
        {
            var nazwy = _context.KompetencjeUmiejetnosci
                .Where(k => !string.IsNullOrEmpty(k.Nazwa))
                .Select(k => k.Nazwa)
                .Distinct()
                .OrderBy(n => n)
                .ToList();
            return Ok(nazwy);
        }

        [HttpGet("options/poziom")]
        public ActionResult<IEnumerable<string>> GetPoziomyOptions()
        {
            var poziomy = _context.KompetencjeUmiejetnosci
                .Where(k => !string.IsNullOrEmpty(k.Poziom))
                .Select(k => k.Poziom)
                .Distinct()
                .OrderBy(p => p)
                .ToList();
            return Ok(poziomy);
        }

        [HttpGet("options/zaswiadczenie")]
        public ActionResult<IEnumerable<string>> GetZaswiadczeniaOptions()
        {
            var zaswiadczenia = _context.KompetencjeUmiejetnosci
                .Where(k => !string.IsNullOrEmpty(k.Zaswiadczenie))
                .Select(k => k.Zaswiadczenie)
                .Distinct()
                .OrderBy(z => z)
                .ToList();
            return Ok(zaswiadczenia);
        }

        [HttpGet("{id}")]
        public ActionResult<KompetencjeUmiejetnosci> GetById(int id)
        {
            var kompetencja = _context.KompetencjeUmiejetnosci.Find(id);
            if (kompetencja == null)
                return NotFound();
            
            return Ok(kompetencja);
        }

        [HttpPost]
        public ActionResult<KompetencjeUmiejetnosci> Create(KompetencjeUmiejetnosci kompetencja)
        {
            try
            {
                Console.WriteLine($"Otrzymane dane POST:");
                Console.WriteLine($"  OsobaId: {kompetencja.OsobaId}");
                Console.WriteLine($"  Kod: {kompetencja.Kod}");
                Console.WriteLine($"  Nazwa: {kompetencja.Nazwa}");
                Console.WriteLine($"  Poziom: {kompetencja.Poziom}");
                Console.WriteLine($"  Zaswiadczenie: {kompetencja.Zaswiadczenie}");
                
                _context.KompetencjeUmiejetnosci.Add(kompetencja);
                _context.SaveChanges();
                return CreatedAtAction(nameof(GetById), new { id = kompetencja.Id }, kompetencja);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"BŁĄD podczas dodawania kompetencji: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
                if (ex.InnerException != null)
                {
                    Console.WriteLine($"Inner exception: {ex.InnerException.Message}");
                }
                return BadRequest(new { error = ex.Message, innerError = ex.InnerException?.Message });
            }
        }

        [HttpPut("{id}")]
        public ActionResult<KompetencjeUmiejetnosci> Update(int id, KompetencjeUmiejetnosci kompetencja)
        {
            try
            {
                Console.WriteLine($"Otrzymane dane PUT dla ID {id}:");
                Console.WriteLine($"  OsobaId: {kompetencja.OsobaId}");
                Console.WriteLine($"  Kod: {kompetencja.Kod}");
                Console.WriteLine($"  Nazwa: {kompetencja.Nazwa}");
                Console.WriteLine($"  Poziom: {kompetencja.Poziom}");
                Console.WriteLine($"  Zaswiadczenie: {kompetencja.Zaswiadczenie}");
                
                var existing = _context.KompetencjeUmiejetnosci.Find(id);
                if (existing == null)
                    return NotFound();

                existing.Kod = kompetencja.Kod;
                existing.Nazwa = kompetencja.Nazwa;
                existing.Poziom = kompetencja.Poziom;
                existing.Zaswiadczenie = kompetencja.Zaswiadczenie;

                _context.SaveChanges();
                return Ok(existing);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"BŁĄD podczas aktualizacji kompetencji: {ex.Message}");
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
            var kompetencja = _context.KompetencjeUmiejetnosci.Find(id);
            if (kompetencja == null)
                return NotFound();

            _context.KompetencjeUmiejetnosci.Remove(kompetencja);
            _context.SaveChanges();
            return NoContent();
        }
    }
}
