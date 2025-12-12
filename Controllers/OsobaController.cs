using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SzpitalnaKadra.Data;
using SzpitalnaKadra.Models;
using Npgsql;

namespace SzpitalnaKadra.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OsobaController : ControllerBase
    {
        private readonly AppDbContext _context;

        public OsobaController(AppDbContext context)
        {
            _context = context;
        }

        private List<Osoba> GetOsobyWithFallback()
        {
            try
            {
                return _context.Osoby.ToList();
            }
            catch (Exception ex) when (ex.ToString().Contains("42703") || ex.ToString().Contains("data_zgonu"))
            {
                return _context.Osoby.FromSqlRaw(@"
                    SELECT id, pesel, plec_id, data_urodzenia, nazwisko, imie, imie2, 
                           typ_personelu_id, nr_pwz, numer_telefonu, NULL::date as data_zgonu
                    FROM osoba
                ").ToList();
            }
        }

        [HttpGet("filters")]
        public IActionResult GetFilters()
        {
            try
            {
                var plecIds = _context.Osoby.Select(o => o.PlecId).Distinct().OrderBy(p => p).ToList();
                var typPersoneluIds = _context.Osoby.Select(o => o.TypPersoneluId).Distinct().OrderBy(t => t).ToList();

                return Ok(new { plecIds, typPersoneluIds });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }

        [HttpGet]
        public IActionResult GetAll(
            [FromQuery] string? pesel, 
            [FromQuery] string? imie, 
            [FromQuery] string? nazwisko,
            [FromQuery] int? plecId,
            [FromQuery] int? typPersoneluId,
            [FromQuery] bool? aktywnieZatrudnieni,
            [FromQuery] bool? majaOgraniczenia,
            [FromQuery] string? rodzajWyksztalcenia)
        {
            var osoby = GetOsobyWithFallback();

            // Filtrowanie wyszukiwania
            if (!string.IsNullOrWhiteSpace(pesel))
                osoby = osoby.Where(o => o.Pesel != null && o.Pesel.Contains(pesel, StringComparison.OrdinalIgnoreCase)).ToList();

            if (!string.IsNullOrWhiteSpace(imie))
                osoby = osoby.Where(o => o.Imie.Contains(imie, StringComparison.OrdinalIgnoreCase)).ToList();

            if (!string.IsNullOrWhiteSpace(nazwisko))
                osoby = osoby.Where(o => o.Nazwisko.Contains(nazwisko, StringComparison.OrdinalIgnoreCase)).ToList();

            // Filtrowanie dropdown
            if (plecId.HasValue)
                osoby = osoby.Where(o => o.PlecId == plecId.Value).ToList();

            if (typPersoneluId.HasValue)
                osoby = osoby.Where(o => o.TypPersoneluId == typPersoneluId.Value).ToList();

            // Filtrowanie na podstawie zatrudnienia
            if (aktywnieZatrudnieni.HasValue && aktywnieZatrudnieni.Value)
            {
                var dzis = DateTime.Now.Date;
                var zatrudnieniOsobyIds = _context.Zatrudnienia
                    .Where(z => z.ZatrudnionyDo == null || z.ZatrudnionyDo >= dzis)
                    .Select(z => z.OsobaId)
                    .Distinct()
                    .ToList();
                osoby = osoby.Where(o => zatrudnieniOsobyIds.Contains(o.Id)).ToList();
            }

            // Filtrowanie na podstawie ograniczeń
            if (majaOgraniczenia.HasValue && majaOgraniczenia.Value)
            {
                var osobyZOgraniczeniami = _context.OgraniczeniaUprawnien
                    .Select(og => og.OsobaId)
                    .Distinct()
                    .ToList();
                osoby = osoby.Where(o => osobyZOgraniczeniami.Contains(o.Id)).ToList();
            }

            // Filtrowanie na podstawie wykształcenia
            if (!string.IsNullOrWhiteSpace(rodzajWyksztalcenia))
            {
                var osobyZWyksztalceniem = _context.Wyksztalcenia
                    .Where(w => w.RodzajWyksztalcenia != null && 
                                w.RodzajWyksztalcenia.Contains(rodzajWyksztalcenia, StringComparison.OrdinalIgnoreCase))
                    .Select(w => w.OsobaId)
                    .Distinct()
                    .ToList();
                osoby = osoby.Where(o => osobyZWyksztalceniem.Contains(o.Id)).ToList();
            }

            return Ok(osoby);
        }

        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            var osoba = GetOsobyWithFallback().FirstOrDefault(o => o.Id == id);
            if (osoba == null)
                return NotFound();
            return Ok(osoba);
        }

        [HttpPost]
        public IActionResult Add(Osoba osoba)
        {
            _context.Osoby.Add(osoba);
            _context.SaveChanges();
            return Ok(osoba);
        }

        [HttpPut("{id}")]
        public IActionResult Update(int id, Osoba osoba)
        {
            var existing = _context.Osoby.FirstOrDefault(o => o.Id == id);
            if (existing == null)
                return NotFound();

            existing.Imie = osoba.Imie;
            existing.Imie2 = osoba.Imie2;
            existing.Nazwisko = osoba.Nazwisko;
            existing.Pesel = osoba.Pesel;
            existing.DataUrodzenia = osoba.DataUrodzenia;
            existing.NrPwz = osoba.NrPwz;
            existing.NumerTelefonu = osoba.NumerTelefonu;
            existing.PlecId = osoba.PlecId;
            existing.TypPersoneluId = osoba.TypPersoneluId;

            try { existing.DataZgonu = osoba.DataZgonu; } catch { }

            _context.SaveChanges();
            return Ok(existing);
        }
    }
}
