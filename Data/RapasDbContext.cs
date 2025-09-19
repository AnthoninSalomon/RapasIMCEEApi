using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using RapasIMCEEApi.Models;

namespace RapasIMCEEApi.Data
{
    public class RapasDbContext : DbContext
    {
        public RapasDbContext(DbContextOptions<RapasDbContext> options) : base(options) { }

        public DbSet<Objet> Objets { get; set; }
        public DbSet<ObjetData> ObjetDatas { get; set; }
        public DbSet<Utilisateur> Utilisateurs { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Objet>().HasIndex(o => o.Libelle).IsUnique();

            // DateOnly <-> Date mapping
            var dateOnlyConverter = new ValueConverter<DateOnly, DateTime>(
                d => d.ToDateTime(TimeOnly.MinValue),
                dt => DateOnly.FromDateTime(dt));
            var nullableDateOnlyConverter = new ValueConverter<DateOnly?, DateTime?>(
                d => d.HasValue ? d.Value.ToDateTime(TimeOnly.MinValue) : null,
                dt => dt.HasValue ? DateOnly.FromDateTime(dt.Value) : null);

            // TimeOnly <-> TimeSpan (MySQL TIME)
            var timeOnlyConverter = new ValueConverter<TimeOnly, TimeSpan>(
                t => t.ToTimeSpan(),
                ts => TimeOnly.FromTimeSpan(ts));
            var nullableTimeOnlyConverter = new ValueConverter<TimeOnly?, TimeSpan?>(
                t => t.HasValue ? t.Value.ToTimeSpan() : null,
                ts => ts.HasValue ? TimeOnly.FromTimeSpan(ts.Value) : null);

            modelBuilder.Entity<ObjetData>().Property(od => od.DateObservation)
                .HasConversion(nullableDateOnlyConverter)
                .HasColumnType("date");

            modelBuilder.Entity<ObjetData>().Property(od => od.UTCHHMNSS)
                .HasConversion(nullableTimeOnlyConverter)
                .HasColumnType("time");

            // Si une contrainte unique existe déjà côté DB pour éviter doublons,
            // tu n'as pas besoin de la redéfinir ici — mais tu peux la refléter si tu veux.
        }
    }
}
