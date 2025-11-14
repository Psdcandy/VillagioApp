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

        [HttpPost("cadastrar")]
        public async Task<IActionResult> Cadastrar([FromBody] Usuario usuario)
        {
            // Verifica duplicidade
            bool existeUsuario = await _context.Usuarios.AnyAsync(u =>
                (usuario.TipoUsuarioId == 1 && u.Telefone == usuario.Telefone) || // Família pelo telefone
                (usuario.TipoUsuarioId == 2 && (u.Email == usuario.Email || u.CNPJ == usuario.CNPJ)) // Agência pelo email ou CNPJ
            );

            if (existeUsuario)
                return BadRequest("Já existe um usuário cadastrado com esses dados.");

            // Validações existentes...
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

        [HttpGet("listar")]
        public async Task<IActionResult> Listar()
        {
            var usuarios = await _context.Usuarios.ToListAsync();
            return Ok(usuarios);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> BuscarPorId(int id)
        {
            var usuario = await _context.Usuarios.FindAsync(id);
            if (usuario == null)
                return NotFound("Usuário não encontrado.");
            return Ok(usuario);
        }

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
