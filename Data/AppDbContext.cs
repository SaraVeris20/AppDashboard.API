using AppDashboard.API.Models;
using Microsoft.EntityFrameworkCore;

namespace AppDashboard.API.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<Usuario> Usuarios { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Usuario>()
                .HasIndex(u => u.Email)
                .IsUnique();

            
            modelBuilder.Entity<Usuario>().HasData(
                new Usuario
                {
                    Id = 1,
                    Nome = "Administrador",
                    Email = "admin@appdashboard.com",
                    // Senha: Admin@123
                    SenhaHash = BCrypt.Net.BCrypt.HashPassword("Admin@123"),
                    Cargo = "Administrador",
                    Ativo = true,
                    DataCriacao = DateTime.UtcNow
                }
            );
        }
    }
}