using API.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UtilizadoresController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public UtilizadoresController(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        // POST: api/Utilizadores/RegistarUser
        // Serve para criar conta com NIF, Morada, etc.
        [HttpPost("RegistarUser")]
        public async Task<IActionResult> RegistarUser([FromBody] RegisterUserModel model)
        {
            if (await _userManager.FindByEmailAsync(model.Email) != null)
                return BadRequest("Este email já está registado.");

            // REGRA DO ENUNCIADO:
            // Registo de Cliente -> Estado "Pendente"
            // Só pode ser "Cliente" via app (Fornecedor requer Admin).

            var user = new ApplicationUser
            {
                UserName = model.Email,
                Email = model.Email,

                // Dados Pessoais
                Nome = model.Nome,
                Apelido = model.Apelido,
                NIF = model.NIF,
                Telemovel = model.Telemovel,

                // Morada
                Rua = model.Rua,
                Localidade = model.Localidade,
                CodigoPostal = model.CodigoPostal,
                Cidade = model.Cidade,
                Pais = model.Pais,

                // Regras:
                Estado = "Pendente", // Bloqueado até o Admin ativar
                DataRegisto = DateTime.UtcNow
            };

            var result = await _userManager.CreateAsync(user, model.Password);

            if (!result.Succeeded) return BadRequest(result.Errors);

            // Forçar Role de Cliente
            await _userManager.AddToRoleAsync(user, "Cliente");

            return Ok(new { message = "Registo efetuado! A sua conta aguarda aprovação." });
        }

        // DTO Interno para receber os dados todos
        public class RegisterUserModel
        {
            [Required] public string Nome { get; set; } = "";
            [Required] public string Apelido { get; set; } = "";
            [Required] public long NIF { get; set; }
            public string Telemovel { get; set; } = "";
            public string Rua { get; set; } = "";
            public string Localidade { get; set; } = "";
            public string CodigoPostal { get; set; } = "";
            public string Cidade { get; set; } = "";
            public string Pais { get; set; } = "";
            [Required, EmailAddress] public string Email { get; set; } = "";
            [Required] public string Password { get; set; } = "";
        }
    }
}