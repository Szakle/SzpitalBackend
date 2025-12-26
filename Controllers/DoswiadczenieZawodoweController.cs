using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SzpitalnaKadra.Data;
using SzpitalnaKadra.Models;
using System;

namespace SzpitalnaKadra.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DoswiadczenieZawodoweController : ControllerBase
    {
        private readonly AppDbContext _context;

        public DoswiadczenieZawodoweController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet("osoba/{osobaId}")]
        public ActionResult<IEnumerable<DoswiadczenieZawodowe>> GetByOsobaId(int osobaId)
        {
            var doswiadczenie = _context.DoswiadczenieZawodowe
                .Where(d => d.OsobaId == osobaId)
                .ToList();

            return Ok(doswiadczenie);
        }

        [HttpGet("options/kod")]
        public ActionResult<IEnumerable<string>> GetKodyOptions()
        {
            var kody = _context.DoswiadczenieZawodowe
                .Where(d => !string.IsNullOrEmpty(d.Kod))
                .Select(d => d.Kod)
                .Distinct()
                .OrderBy(k => k)
                .ToList();
            return Ok(kody);
        }

        [HttpGet("options/nazwa")]
        public ActionResult<IEnumerable<string>> GetNazwyOptions()
        {
            var nazwy = _context.DoswiadczenieZawodowe
                .Where(d => !string.IsNullOrEmpty(d.Nazwa))
                .Select(d => d.Nazwa)
                .Distinct()
                .OrderBy(n => n)
                .ToList();
            return Ok(nazwy);
        }

        [HttpGet("options/zaswiadczenie")]
        public ActionResult<IEnumerable<string>> GetZaswiadczeniaOptions()
        {
            var zaswiadczenia = _context.DoswiadczenieZawodowe
                .Where(d => !string.IsNullOrEmpty(d.Zaswiadczenie))
                .Select(d => d.Zaswiadczenie)
                .Distinct()
                .OrderBy(z => z)
                .ToList();
            return Ok(zaswiadczenia);
        }

        [HttpGet("{id}")]
        public ActionResult<DoswiadczenieZawodowe> GetById(int id)
        {
            var doswiadczenie = _context.DoswiadczenieZawodowe.Find(id);
            if (doswiadczenie == null)
                return NotFound();
            
            return Ok(doswiadczenie);
        }

        [HttpPost]
        public ActionResult<DoswiadczenieZawodowe> Create(DoswiadczenieZawodowe doswiadczenie)
        {
            try
            {
                Console.WriteLine($"Otrzymane dane POST:");
                Console.WriteLine($"  OsobaId: {doswiadczenie.OsobaId}");
                Console.WriteLine($"  Kod: {doswiadczenie.Kod}");
                Console.WriteLine($"  Nazwa: {doswiadczenie.Nazwa}");
                Console.WriteLine($"  Zaswiadczenie: {doswiadczenie.Zaswiadczenie}");
                
                _context.DoswiadczenieZawodowe.Add(doswiadczenie);
                _context.SaveChanges();
                return CreatedAtAction(nameof(GetById), new { id = doswiadczenie.Id }, doswiadczenie);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"BŁĄD podczas dodawania doświadczenia zawodowego: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
                if (ex.InnerException != null)
                {
                    Console.WriteLine($"Inner exception: {ex.InnerException.Message}");
                }
                return BadRequest(new { error = ex.Message, innerError = ex.InnerException?.Message });
            }
        }

        [HttpPut("{id}")]
        public ActionResult<DoswiadczenieZawodowe> Update(int id, DoswiadczenieZawodowe doswiadczenie)
        {
            try
            {
                Console.WriteLine($"Otrzymane dane PUT dla ID {id}:");
                Console.WriteLine($"  OsobaId: {doswiadczenie.OsobaId}");
                Console.WriteLine($"  Kod: {doswiadczenie.Kod}");
                Console.WriteLine($"  Nazwa: {doswiadczenie.Nazwa}");
                Console.WriteLine($"  Zaswiadczenie: {doswiadczenie.Zaswiadczenie}");
                
                var existing = _context.DoswiadczenieZawodowe.Find(id);
                if (existing == null)
                    return NotFound();

                existing.Kod = doswiadczenie.Kod;
                existing.Nazwa = doswiadczenie.Nazwa;
                existing.Zaswiadczenie = doswiadczenie.Zaswiadczenie;

                _context.SaveChanges();
                return Ok(existing);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"BŁĄD podczas aktualizacji doświadczenia zawodowego: {ex.Message}");
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
            var doswiadczenie = _context.DoswiadczenieZawodowe.Find(id);
            if (doswiadczenie == null)
                return NotFound();

            _context.DoswiadczenieZawodowe.Remove(doswiadczenie);
            _context.SaveChanges();
            return NoContent();
        }
    }
}
