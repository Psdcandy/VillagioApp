
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
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            try
            {
                if (request == null)
                    return BadRequest("Dados inválidos.");

                // Log inicial
                Console.WriteLine($"[LOGIN] TipoUsuarioId={request.TipoUsuarioId}, Nome='{request.Nome}', Telefone='{request.Telefone}', CNPJ='{request.CNPJ}', Senha='{request.Senha}'");

                Usuario? usuario = null;

                // Funções auxiliares
                string Normalize(string? text)
                {
                    if (string.IsNullOrWhiteSpace(text)) return string.Empty;
                    var normalized = text.Normalize(NormalizationForm.FormD);
                    var sb = new StringBuilder();
                    foreach (var ch in normalized)
                    {
                        var uc = CharUnicodeInfo.GetUnicodeCategory(ch);
                        if (uc != UnicodeCategory.NonSpacingMark)
                            sb.Append(char.ToLowerInvariant(ch));
                    }
                    return sb.ToString();
                }

                string DigitsOnly(string? s) => new string((s ?? "").Where(char.IsDigit).ToArray());

                if (request.TipoUsuarioId == 1) // Família
                {
                    string nomeReq = Normalize(request.Nome);
                    string telReq = DigitsOnly(request.Telefone);
                    string senhaReq = (request.Senha ?? "").Trim();

                    var usuarios = await _context.Usuarios
                        .Where(u => u.TipoUsuarioId == 1 && (u.Senha ?? "").Trim() == senhaReq)
                        .ToListAsync();

                    usuario = usuarios.FirstOrDefault(u =>
                        DigitsOnly(u.Telefone) == telReq &&
                        Normalize(u.Nome) == nomeReq);
                }
                else if (request.TipoUsuarioId == 2) // Agência
                {
                    string nomeReq = Normalize(request.Nome);
                    string telReq = DigitsOnly(request.Telefone);
                    string cnpjReq = DigitsOnly(request.CNPJ);
                    string senhaReq = (request.Senha ?? "").Trim();

                    var usuariosAgencia = await _context.Usuarios
                        .Where(u => u.TipoUsuarioId == 2)
                        .ToListAsync();

                    usuario = usuariosAgencia.FirstOrDefault(u =>
                        Normalize(u.Nome) == nomeReq &&
                        DigitsOnly(u.Telefone) == telReq &&
                        DigitsOnly(u.CNPJ) == cnpjReq &&
                        (u.Senha ?? "").Trim() == senhaReq);
                }
                else
                {
                    return BadRequest("Tipo de usuário inválido.");
                }

                if (usuario == null)
                {
                    Console.WriteLine($"[LOGIN] Nenhum usuário encontrado. Nome(normalizado)='{Normalize(request.Nome)}', Telefone='{DigitsOnly(request.Telefone)}', CNPJ='{DigitsOnly(request.CNPJ)}'");
                    return Unauthorized("Credenciais inválidas.");
                }

                var userResponse = new
                {
                    usuario.Id,
                    usuario.Nome,
                    usuario.TipoUsuarioId
                };

                return Ok(userResponse);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro interno no servidor: {ex.Message}");
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
