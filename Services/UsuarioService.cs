using AppDashboard.API.Data;
using AppDashboard.API.DTOs;
using AppDashboard.API.Helpers;
using AppDashboard.API.Models;
using Microsoft.EntityFrameworkCore;
using System.IdentityModel.Tokens.Jwt;

namespace AppDashboard.API.Services
{
    public class UsuarioService : IUsuarioService
    {
        private readonly AppDbContext _context;
        private readonly JwtHelper _jwtHelper;

        public UsuarioService(AppDbContext context, JwtHelper jwtHelper)
        {
            _context = context;
            _jwtHelper = jwtHelper;
        }

        public async Task<List<UsuarioDto>> ObterTodosAsync()
        {
            var usuarios = await _context.Usuarios
                .Where(u => u.Ativo)
                .OrderBy(u => u.Nome)
                .ToListAsync();

            return usuarios.Select(MapearParaDto).ToList();
        }

        public async Task<UsuarioDto?> ObterPorIdAsync(int id)
        {
            var usuario = await _context.Usuarios.FindAsync(id);
            return usuario != null ? MapearParaDto(usuario) : null;
        }

        public async Task<UsuarioDto?> ObterPorEmailAsync(string email)
        {
            var usuario = await _context.Usuarios
                .FirstOrDefaultAsync(u => u.Email == email);

            return usuario != null ? MapearParaDto(usuario) : null;
        }

        public async Task<UsuarioDto> CriarAsync(CriarUsuarioDto dto)
        {
           
            var emailExiste = await _context.Usuarios
                .AnyAsync(u => u.Email == dto.Email);

            if (emailExiste)
            {
                throw new InvalidOperationException("Email já cadastrado");
            }

            
            var fotoUrl = ObterGravatarUrl(dto.Email);

            var usuario = new Usuario
            {
                Nome = dto.Nome,
                Email = dto.Email,
                SenhaHash = BCrypt.Net.BCrypt.HashPassword(dto.Senha),
                Cargo = dto.Cargo,
                UnidadeGrupo = dto.UnidadeGrupo,
                FotoUrl = fotoUrl,
                Ativo = true,
                DataCriacao = DateTime.UtcNow
            };

            _context.Usuarios.Add(usuario);
            await _context.SaveChangesAsync();

            return MapearParaDto(usuario);
        }

        public async Task<UsuarioDto?> AtualizarAsync(int id, AtualizarUsuarioDto dto)
        {
            var usuario = await _context.Usuarios.FindAsync(id);
            if (usuario == null) return null;

            if (!string.IsNullOrEmpty(dto.Nome))
                usuario.Nome = dto.Nome;

            if (!string.IsNullOrEmpty(dto.Email))
            {
                
                var emailExiste = await _context.Usuarios
                    .AnyAsync(u => u.Email == dto.Email && u.Id != id);

                if (emailExiste)
                {
                    throw new InvalidOperationException("Email já cadastrado");
                }

                usuario.Email = dto.Email;
                usuario.FotoUrl = ObterGravatarUrl(dto.Email);
            }

            if (!string.IsNullOrEmpty(dto.Cargo))
                usuario.Cargo = dto.Cargo;

            if (!string.IsNullOrEmpty(dto.UnidadeGrupo))
                usuario.UnidadeGrupo = dto.UnidadeGrupo;

            if (dto.Ativo.HasValue)
                usuario.Ativo = dto.Ativo.Value;

            usuario.DataAtualizacao = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return MapearParaDto(usuario);
        }

        public async Task<bool> DeletarAsync(int id)
        {
            var usuario = await _context.Usuarios.FindAsync(id);
            if (usuario == null) return false;

            
            usuario.Ativo = false;
            usuario.DataAtualizacao = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<LoginResponseDto?> LoginAsync(LoginDto dto)
        {
            var usuario = await _context.Usuarios
                .FirstOrDefaultAsync(u => u.Email == dto.Email && u.Ativo);

            if (usuario == null) return null;

            
            if (!BCrypt.Net.BCrypt.Verify(dto.Senha, usuario.SenhaHash))
            {
                return null;
            }

            
            var token = _jwtHelper.GerarToken(usuario);

            return new LoginResponseDto
            {
                Token = token,
                Usuario = MapearParaDto(usuario)
            };
        }

        
        private static UsuarioDto MapearParaDto(Usuario usuario)
        {
            return new UsuarioDto
            {
                Id = usuario.Id,
                Nome = usuario.Nome,
                Email = usuario.Email,
                Cargo = usuario.Cargo,
                FotoUrl = usuario.FotoUrl,
                UnidadeGrupo = usuario.UnidadeGrupo,
                Ativo = usuario.Ativo,
                DataCriacao = usuario.DataCriacao
            };
        }

        private static string ObterGravatarUrl(string email)
        {
            
            using var md5 = System.Security.Cryptography.MD5.Create();
            var inputBytes = System.Text.Encoding.ASCII.GetBytes(email.Trim().ToLower());
            var hashBytes = md5.ComputeHash(inputBytes);
            var hash = Convert.ToHexString(hashBytes).ToLower();

            return $"https://www.gravatar.com/avatar/{hash}?d=identicon&s=200";
        }
    }
}