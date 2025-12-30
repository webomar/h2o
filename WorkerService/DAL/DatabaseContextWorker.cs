using ApiService.Models;
using Microsoft.EntityFrameworkCore;

namespace WorkerService.DAL
{
    public class DatabaseContextWorker : DbContext
    {
        public DatabaseContextWorker(DbContextOptions<DatabaseContextWorker> options) : base(options) { }

        public DbSet<Functie> Functies { get; set; }
        public DbSet<Medewerker> Medewerkers { get; set; }
        public DbSet<Dienstverband> Dienstverbanden { get; set; }
        public DbSet<OrganisatorischeEenheid> OrganisatorischeEenheden { get; set; }
        public DbSet<Kostenplaats> Kostenplaatsen { get; set; }
        public DbSet<Begroting> Begrotingen { get; set; }
        public DbSet<Begrotingsregel> Begrotingsregels { get; set; }
        public DbSet<Periode> Periodes { get; set; }
        public DbSet<Inhuurkosten> Inhuurkosten { get; set; }
        public DbSet<Contract> Contracten { get; set; }
        public DbSet<Transactie> Transacties { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure Functie
            modelBuilder.Entity<Functie>()
                .HasKey(f => f.Functiecode);

            // Configure Medewerker
            modelBuilder.Entity<Medewerker>()
                .HasKey(m => m.Nummer);

            modelBuilder.Entity<Medewerker>()
                .HasOne(m => m.Dienstverband)
                .WithOne(d => d.Medewerker)
                .HasForeignKey<Dienstverband>(d => d.MedewerkerNummer)
                .OnDelete(DeleteBehavior.Restrict);

            // Configure Dienstverband
            modelBuilder.Entity<Dienstverband>()
                .HasKey(d => d.Nummer);

            modelBuilder.Entity<Dienstverband>()
                .HasOne(d => d.Functie)
                .WithMany(f => f.Dienstverbanden)
                .HasForeignKey(d => d.Functiecode)
                .OnDelete(DeleteBehavior.Restrict);

            // Configure OrganisatorischeEenheid (self-referencing)
            modelBuilder.Entity<OrganisatorischeEenheid>()
                .HasKey(o => o.Code);

            modelBuilder.Entity<OrganisatorischeEenheid>()
                .HasOne(o => o.Parent)
                .WithMany(o => o.Children)
                .HasForeignKey(o => o.ParentCode)
                .OnDelete(DeleteBehavior.Restrict);

            // Configure Kostenplaats
            modelBuilder.Entity<Kostenplaats>()
                .HasKey(k => k.Code);

            modelBuilder.Entity<Kostenplaats>()
                .HasOne(k => k.OrganisatorischeEenheid)
                .WithMany(o => o.Kostenplaatsen)
                .HasForeignKey(k => k.OrganisatorischeEenheidCode)
                .OnDelete(DeleteBehavior.Restrict);

            // Configure Begroting
            modelBuilder.Entity<Begroting>()
                .HasKey(b => b.Jaar);

            // Configure Begrotingsregel
            modelBuilder.Entity<Begrotingsregel>()
                .HasKey(b => b.Id);

            modelBuilder.Entity<Begrotingsregel>()
                .HasOne(b => b.Begroting)
                .WithMany(b => b.Begrotingsregels)
                .HasForeignKey(b => b.BegrotingJaar)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Begrotingsregel>()
                .HasOne(b => b.Medewerker)
                .WithMany(m => m.Begrotingsregels)
                .HasForeignKey(b => b.MedewerkerNummer)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<Begrotingsregel>()
                .HasOne(b => b.Kostenplaats)
                .WithMany(k => k.Begrotingsregels)
                .HasForeignKey(b => b.KostenplaatsCode)
                .OnDelete(DeleteBehavior.SetNull);

            // Configure Periode
            modelBuilder.Entity<Periode>()
                .HasKey(p => p.Id);

            // Configure Inhuurkosten
            modelBuilder.Entity<Inhuurkosten>()
                .HasKey(i => i.Id);

            modelBuilder.Entity<Inhuurkosten>()
                .HasOne(i => i.Periode)
                .WithMany(p => p.Inhuurkosten)
                .HasForeignKey(i => i.PeriodeId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Inhuurkosten>()
                .HasOne(i => i.Kostenplaats)
                .WithMany(k => k.Inhuurkosten)
                .HasForeignKey(i => i.KostenplaatsCode)
                .OnDelete(DeleteBehavior.Restrict);

            // Configure Contract
            modelBuilder.Entity<Contract>()
                .HasKey(c => c.Id);

            modelBuilder.Entity<Contract>()
                .HasOne(c => c.Medewerker)
                .WithMany(m => m.Contracten)
                .HasForeignKey(c => c.MedewerkerNummer)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<Contract>()
                .HasOne(c => c.OrganisatorischeEenheid)
                .WithMany(o => o.Contracten)
                .HasForeignKey(c => c.OrganisatorischeEenheidCode)
                .OnDelete(DeleteBehavior.SetNull);

            // Configure Transactie
            modelBuilder.Entity<Transactie>()
                .HasKey(t => t.Id);

            modelBuilder.Entity<Transactie>()
                .HasOne(t => t.Contract)
                .WithMany(c => c.Transacties)
                .HasForeignKey(t => t.ContractId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
