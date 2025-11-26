
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
using System.Text;
using VillagioApi.Data;
using VillagioApi.Model;

namespace VillagioApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsuarioController : ControllerBase
    {
        private readonly DBContext _context;

        public UsuarioController(DBContext context)
        {
            _context = context;
        }

        // ✅ Funções utilitárias para normalização
        private static string NormalizeText(string? input)
        {
            if (string.IsNullOrWhiteSpace(input)) return string.Empty;

            var normalized = input.Trim().Normalize(NormalizationForm.FormD);
            var sb = new StringBuilder(normalized.Length);

            foreach (var ch in normalized)
            {
                var uc = CharUnicodeInfo.GetUnicodeCategory(ch);
                if (uc != UnicodeCategory.NonSpacingMark &&
                    uc != UnicodeCategory.SpacingCombiningMark &&
                    uc != UnicodeCategory.EnclosingMark)
                {
                    sb.Append(char.ToLowerInvariant(ch));
                }
            }

            return sb.ToString().Normalize(NormalizationForm.FormC);
        }

        private static string DigitsOnly(string? s) =>
            new string((s ?? string.Empty).Where(char.IsDigit).ToArray());

        // ✅ Cadastro
        [HttpPost("cadastrar")]
        public async Task<IActionResult> Cadastrar([FromBody] Usuario usuario)
        {
            bool existeUsuario = await _context.Usuarios.AnyAsync(u =>
                (usuario.TipoUsuarioId == 1 && u.Telefone == usuario.Telefone) ||
                (usuario.TipoUsuarioId == 2 && (u.CNPJ == usuario.CNPJ))
            );

            if (existeUsuario)
                return BadRequest("Já existe um usuário cadastrado com esses dados.");

            if (usuario.TipoUsuarioId == 1)
            {
                if (string.IsNullOrEmpty(usuario.Nome) || string.IsNullOrEmpty(usuario.Telefone) || string.IsNullOrEmpty(usuario.Senha))
                    return BadRequest("Família deve informar Nome, Telefone e Senha.");
                usuario.CNPJ = null;
                usuario.Email = null;
            }
            else if (usuario.TipoUsuarioId == 2)
            {
                if (string.IsNullOrEmpty(usuario.Nome) || string.IsNullOrEmpty(usuario.Telefone) ||
                    string.IsNullOrEmpty(usuario.Senha) || string.IsNullOrEmpty(usuario.CNPJ))
                    return BadRequest("Agência deve informar Nome, Telefone, CNPJ e Senha.");
                usuario.Email = null; // Não usamos mais
            }

            _context.Usuarios.Add(usuario);
            await _context.SaveChangesAsync();
            return Ok(usuario);
        }



        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest dto)
        {
            try
            {
                // Validação do corpo
                if (dto == null)
                    return BadRequest(new ProblemDetails { Title = "Requisição inválida", Detail = "Corpo da requisição está vazio." });

                if (dto.TipoUsuarioId != 1 && dto.TipoUsuarioId != 2)
                    return BadRequest(new ProblemDetails { Title = "Tipo inválido", Detail = "TipoUsuarioId deve ser 1 (Família) ou 2 (Agência)." });

                if (string.IsNullOrWhiteSpace(dto.Nome))
                    return BadRequest(new ProblemDetails { Title = "Nome obrigatório", Detail = "Informe o nome." });

                if (string.IsNullOrWhiteSpace(dto.Telefone) || dto.Telefone.Length < 10)
                    return BadRequest(new ProblemDetails { Title = "Telefone inválido", Detail = "Informe DDD + número (10 ou 11 dígitos)." });

                if (string.IsNullOrWhiteSpace(dto.Senha) || dto.Senha.Length < 6)
                    return BadRequest(new ProblemDetails { Title = "Senha inválida", Detail = "A senha deve ter pelo menos 6 caracteres." });

                if (dto.TipoUsuarioId == 2) // Agência
                {
                    if (string.IsNullOrWhiteSpace(dto.CNPJ) || dto.CNPJ.Length != 14)
                        return BadRequest(new ProblemDetails { Title = "CNPJ inválido", Detail = "Informe apenas números (14 dígitos)." });
                }

                // Normaliza valores para evitar NullReference
                var nome = dto.Nome.Trim();
                var telefone = dto.Telefone.Trim();
                var senha = dto.Senha.Trim();
                var cnpj = dto.CNPJ?.Trim() ?? "";
                var email = dto.Email?.Trim() ?? "";

                // Consulta no banco (exemplo com Dapper ou EF Core)
                Usuario usuario = null;

                if (dto.TipoUsuarioId == 1) // Família
                {
                    usuario = await _context.Usuarios
                        .FirstOrDefaultAsync(u => u.Nome == nome && u.Telefone == telefone && u.Senha == senha && u.TipoUsuarioId == 1);
                }
                else // Agência
                {
                    usuario = await _context.Usuarios
                        .FirstOrDefaultAsync(u => u.Nome == nome && u.CNPJ == cnpj && u.Senha == senha && u.TipoUsuarioId == 2);
                }

                if (usuario == null)
                    return Unauthorized(new ProblemDetails { Title = "Credenciais inválidas", Detail = "Usuário não encontrado ou senha incorreta." });

                // Retorna sucesso
                return Ok(new
                {
                    message = "Login realizado com sucesso",
                    usuarioId = usuario.Id,
                    nome = usuario.Nome,
                    tipo = usuario.TipoUsuarioId
                });
            }
            catch (Exception ex)
            {


                // Retorna erro interno com detalhes amigáveis
                return StatusCode(500, new ProblemDetails
                {
                    Title = "Erro interno",
                    Detail = "Ocorreu um erro inesperado. Tente novamente mais tarde.",
                    Status = 500
                });
            }
        }



        // ✅ Listar todos
        [HttpGet("listar")]
        public async Task<IActionResult> Listar()
        {
            var usuarios = await _context.Usuarios.ToListAsync();
            return Ok(usuarios);
        }

        // ✅ Buscar por ID
        [HttpGet("{id}")]
        public async Task<IActionResult> BuscarPorId(int id)
        {
            var usuario = await _context.Usuarios.FindAsync(id);
            if (usuario == null)
                return NotFound("Usuário não encontrado.");
            return Ok(usuario);
        }

        // ✅ Atualizar
        [HttpPut("{id}")]
        public async Task<IActionResult> Atualizar(int id, [FromBody] Usuario usuarioAtualizado)
        {
            var usuario = await _context.Usuarios.FindAsync(id);
            if (usuario == null)
                return NotFound("Usuário não encontrado.");

            usuario.Nome = usuarioAtualizado.Nome;
            usuario.Telefone = usuarioAtualizado.Telefone;
            usuario.Senha = usuarioAtualizado.Senha;
            usuario.Email = usuarioAtualizado.Email;
            usuario.CNPJ = usuarioAtualizado.CNPJ;
            usuario.TipoUsuarioId = usuarioAtualizado.TipoUsuarioId;

            await _context.SaveChangesAsync();
            return Ok(usuario);
        }

        // ✅ Excluir
        [HttpDelete("{id}")]
        public async Task<IActionResult> Excluir(int id)
        {
            var usuario = await _context.Usuarios.FindAsync(id);
            if (usuario == null)
                return NotFound("Usuário não encontrado.");

            _context.Usuarios.Remove(usuario);
            await _context.SaveChangesAsync();
            return Ok("Usuário excluído com sucesso.");
        }
    }
}
