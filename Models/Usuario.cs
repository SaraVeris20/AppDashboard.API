using System.ComponentModel.DataAnnotations;

namespace AppDashboard.API.Models
{
    public class Usuario
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(200)]
        public string Nome { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        [StringLength(200)]
        public string Email { get; set; } = string.Empty;

        [Required]
        public string SenhaHash { get; set; } = string.Empty;

        [StringLength(100)]
        public string Cargo { get; set; } = string.Empty;

        public string? FotoUrl { get; set; }

        [StringLength(100)]
        public string? UnidadeGrupo { get; set; }

        public bool Ativo { get; set; } = true;

        public DateTime DataCriacao { get; set; } = DateTime.UtcNow;

        public DateTime? DataAtualizacao { get; set; }
    }
}