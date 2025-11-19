using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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

        // ✅ Cadastro
        [HttpPost("cadastrar")]
        public async Task<IActionResult> Cadastrar([FromBody] Usuario usuario)
        {
            bool existeUsuario = await _context.Usuarios.AnyAsync(u =>
                (usuario.TipoUsuarioId == 1 && u.Telefone == usuario.Telefone) ||
                (usuario.TipoUsuarioId == 2 && (u.Email == usuario.Email || u.CNPJ == usuario.CNPJ))
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
                    string.IsNullOrEmpty(usuario.Senha) || string.IsNullOrEmpty(usuario.Email) || string.IsNullOrEmpty(usuario.CNPJ))
                    return BadRequest("Agência deve informar todos os campos.");
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

                Usuario? usuario = null;

                if (request.TipoUsuarioId == 1)
                {
                    string telefoneLimpo = new string((request.Telefone ?? "").Where(char.IsDigit).ToArray());
                    string nomeLimpo = (request.Nome ?? "").Trim().ToLower();
                    string senhaLimpa = (request.Senha ?? "").Trim();

                    var usuarios = await _context.Usuarios
                        .Where(u => u.TipoUsuarioId == 1 && u.Senha.Trim() == senhaLimpa)
                        .ToListAsync();

                    usuario = usuarios.FirstOrDefault(u =>
                        new string((u.Telefone ?? "").Where(char.IsDigit).ToArray()) == telefoneLimpo &&
                        (u.Nome ?? "").Trim().ToLower() == nomeLimpo);
                }
                else if (request.TipoUsuarioId == 2)
                {
                    string emailLimpo = (request.Email ?? "").Trim().ToLower();
                    string cnpjLimpo = new string((request.CNPJ ?? "").Where(char.IsDigit).ToArray());
                    string senhaLimpa = (request.Senha ?? "").Trim();

                    var usuariosAgencia = await _context.Usuarios
                        .Where(u => u.TipoUsuarioId == 2)
                        .ToListAsync();

                    usuario = usuariosAgencia.FirstOrDefault(u =>
                        (u.Email ?? "").Trim().ToLower() == emailLimpo &&
                        new string((u.CNPJ ?? "").Where(char.IsDigit).ToArray()) == cnpjLimpo &&
                        u.Senha.Trim() == senhaLimpa);
                }
                else
                {
                    return BadRequest("Tipo de usuário inválido.");
                }

                if (usuario == null)
                    return Unauthorized("Credenciais inválidas.");

                var userResponse = new
                {
                    usuario.Id,
                    usuario.Nome,
                    usuario.Email,
                    usuario.TipoUsuarioId
                };

                return Ok(userResponse);
            }
            catch (Exception ex)
            {
                // Captura qualquer erro interno e retorna detalhes
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
