using API.Data;
using API.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        // REMOVIDO: private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IConfiguration _config;

        // REMOVIDO o SignInManager daqui dos parenteses
        public AuthController(
            IConfiguration config,
            UserManager<ApplicationUser> userManager)
        {
            _config = config;
            _userManager = userManager;
            // REMOVIDO: _signInManager = signInManager;
        }

        // POST: api/Auth/Login
        [HttpPost("Login")]
        public async Task<IActionResult> Login([FromBody] LoginModel model)
        {
            if (model == null || string.IsNullOrEmpty(model.Email) || string.IsNullOrEmpty(model.Password))
                return BadRequest("Email e Password são obrigatórios.");

            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
                return BadRequest("Utilizador não encontrado.");

            // --- ALTERAÇÃO AQUI ---
            // Em vez de _signInManager, usamos o _userManager para ver se a pass bate certo
            var passwordValida = await _userManager.CheckPasswordAsync(user, model.Password);

            if (!passwordValida)
                return BadRequest("Login inválido.");

            // Validar se está ativo (regra de negócio)
            if (user.Estado != "Ativo")
            {
                return StatusCode(403, "A sua conta ainda não foi aprovada pelo Administrador.");
            }

            // Gerar Token
            var token = GenerateToken(user);

            // Obter a Role para enviar na resposta
            var roles = await _userManager.GetRolesAsync(user);
            var role = roles.FirstOrDefault() ?? "Cliente";

            return Ok(new
            {
                accesstoken = token,
                tokentype = "bearer",
                utilizadorid = user.Id,
                utilizadornome = user.Nome,
                role = role,
                estado = user.Estado
            });
        }

        // POST: api/Auth/Register
        [HttpPost("Register")]
        public async Task<IActionResult> Register([FromBody] RegisterModel model)
        {
            if (model == null) return BadRequest("Dados inválidos.");

            var userExists = await _userManager.FindByEmailAsync(model.Email);
            if (userExists != null)
                return BadRequest("Este email já está registado.");

            var user = new ApplicationUser
            {
                UserName = model.Email,
                Email = model.Email,
                Nome = model.Nome,
                Apelido = model.Apelido,
                NIF = model.NIF,
                Telemovel = model.Telemovel,

                // Endereço
                Rua = model.Rua,
                Localidade = model.Localidade,
                CodigoPostal = model.CodigoPostal,
                Cidade = model.Cidade,
                Pais = model.Pais,

                Estado = "Ativo", // Cria logo como Ativo para não teres de ir à BD
                DataRegisto = DateTime.UtcNow
            };

            var result = await _userManager.CreateAsync(user, model.Password);

            if (!result.Succeeded)
                return BadRequest(result.Errors);

            // Atribuir Role (Cliente).
            //var role = model.Role.ToLower() == "fornecedor" ? "Fornecedor" : "Cliente";
            await _userManager.AddToRoleAsync(user, "Cliente");

            return Created("Register", "Registo efetuado!");
        }

        private string GenerateToken(ApplicationUser user)
        {
            var role = _userManager.GetRolesAsync(user).Result.FirstOrDefault() ?? "Cliente";

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(ClaimTypes.Email, user.Email ?? ""),
                new Claim(ClaimTypes.Role, role)
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["JWT:Key"]!));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _config["JWT:Issuer"],
                audience: _config["JWT:Audience"],
                claims: claims,
                expires: DateTime.Now.AddDays(30),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        // Classes de Modelo (DTOs) internas
        public class LoginModel
        {
            public string Email { get; set; } = string.Empty;
            public string Password { get; set; } = string.Empty;
        }

        public class RegisterModel
        {
            public string Email { get; set; } = string.Empty;
            public string Password { get; set; } = string.Empty;
            public string Nome { get; set; } = string.Empty;
            public string Apelido { get; set; } = string.Empty;
            public long NIF { get; set; }
            public string Telemovel { get; set; } = string.Empty;
            public string Rua { get; set; } = string.Empty;
            public string Localidade { get; set; } = string.Empty;
            public string CodigoPostal { get; set; } = string.Empty;
            public string Cidade { get; set; } = string.Empty;
            public string Pais { get; set; } = string.Empty;
            //public string Role { get; set; } = "Cliente";
        }
    }
}