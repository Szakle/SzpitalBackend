using Microsoft.EntityFrameworkCore;
using SzpitalnaKadra.Models;

namespace SzpitalnaKadra.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options) { }

        public DbSet<Osoba> Osoby { get; set; }
        public DbSet<DbUser> DbUsers { get; set; }
        public DbSet<Zatrudnienie> Zatrudnienia { get; set; }
        public DbSet<Wyksztalcenie> Wyksztalcenia { get; set; }
        public DbSet<UprawnieniZawodowe> UprawnieniZawodowe { get; set; }
        public DbSet<OgraniczeniaUprawnien> OgraniczeniaUprawnien { get; set; }
        public DbSet<ZawodySpecjalnosci> ZawodySpecjalnosci { get; set; }
        public DbSet<KompetencjeUmiejetnosci> KompetencjeUmiejetnosci { get; set; }
        public DbSet<DoswiadczenieZawodowe> DoswiadczenieZawodowe { get; set; }
        public DbSet<MiejscePracy> MiejscaPracy { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }
    }
}
