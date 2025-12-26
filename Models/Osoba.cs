using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SzpitalnaKadra.Models
{
    [Table("osoba")]
    public class Osoba
    {
        [Column("id")]
        public int Id { get; set; }

        [Column("pesel")]
        public string? Pesel { get; set; }

        [Column("plec_id")]
        public int PlecId { get; set; }

        [Column("data_urodzenia", TypeName = "date")]
        public DateTime? DataUrodzenia { get; set; }

        [Required]
        [Column("nazwisko")]
        public string Nazwisko { get; set; } = string.Empty;

        [Required]
        [Column("imie")]
        public string Imie { get; set; } = string.Empty;

        [Column("imie2")]
        public string? Imie2 { get; set; }

        [Column("typ_personelu_id")]
        public int TypPersoneluId { get; set; }

        [Required]
        [Column("nr_pwz")]
        public string NrPwz { get; set; } = string.Empty;

        [Column("numer_telefonu")]
        public string? NumerTelefonu { get; set; }

        [NotMapped]
        public DateTime? DataZgonu { get; set; }
    }
}
