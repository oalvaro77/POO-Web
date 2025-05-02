using Microsoft.EntityFrameworkCore;
using Proyecto_POO.Models;

namespace Proyecto_POO.Data
{
    public class ProjectDbContext : DbContext
    {
        public ProjectDbContext(DbContextOptions<ProjectDbContext> options) : base(options) { }

        public DbSet<Person> Persons { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Ubicacion> Ubicaciones { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Configuración de User
            modelBuilder.Entity<User>()
                .HasKey(u => u.Idpersona); // Se usa solo Idpersona como clave primaria

            modelBuilder.Entity<User>()
                .HasOne(u => u.Person)
                .WithOne(p => p.User)
                .HasForeignKey<User>(u => u.Idpersona)
                .OnDelete(DeleteBehavior.Cascade); // Si se borra Person también su User

            // Configuración opcional para prevenir strings nulos en SQL Server
            modelBuilder.Entity<Person>()
                .Property(p => p.Identificacion)
                .IsRequired()
                .HasMaxLength(50);

            modelBuilder.Entity<Person>()
                .Property(p => p.Pnombre)
                .IsRequired()
                .HasMaxLength(50);

            modelBuilder.Entity<Person>()
                .Property(p => p.Papellido)
                .IsRequired()
                .HasMaxLength(50);

            modelBuilder.Entity<Ubicacion>()
                .HasOne(u => u.Person)
                .WithMany(p => p.Ubicacions)
                .HasForeignKey(u => u.Idpersona)
                .OnDelete(DeleteBehavior.Cascade);
        }


        public override int SaveChanges()
        {
            foreach (var entry in ChangeTracker.Entries<Person>())
            {
                if (entry.State == EntityState.Added || entry.State == EntityState.Modified)
                {
                    entry.Entity.CalcularEdades();
                }
            }
            return base.SaveChanges();
        }

    }
}
