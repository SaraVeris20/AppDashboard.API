using AppDashboard.API.DTOs;

namespace AppDashboard.API.Services
{
    public interface IUsuarioService
    {
        Task<List<UsuarioDto>> ObterTodosAsync();
        Task<UsuarioDto?> ObterPorIdAsync(int id);
        Task<UsuarioDto?> ObterPorEmailAsync(string email);
        Task<UsuarioDto> CriarAsync(CriarUsuarioDto dto);
        Task<UsuarioDto?> AtualizarAsync(int id, AtualizarUsuarioDto dto);
        Task<bool> DeletarAsync(int id);
        Task<LoginResponseDto?> LoginAsync(LoginDto dto);
    }
}