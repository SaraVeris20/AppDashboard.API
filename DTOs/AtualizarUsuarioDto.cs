using System.ComponentModel.DataAnnotations;

namespace AppDashboard.API.DTOs
{
    public class AtualizarUsuarioDto
    {
        [StringLength(200)]
        public string? Nome { get; set; }

        [EmailAddress]
        public string? Email { get; set; }

        public string? Cargo { get; set; }

        public string? UnidadeGrupo { get; set; }

        public bool? Ativo { get; set; }
    }
}