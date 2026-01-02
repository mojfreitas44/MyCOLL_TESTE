using API.Data;
using API.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UtilizadoresController : ControllerBase
    {
        private readonly IConfiguration _config;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;

        public UtilizadoresController(
            IConfiguration config,
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager)
        {
            _config = config;
            _userManager = userManager;
            _signInManager = signInManager;
        }

        // ==========================================================
        // LOGIN (Geração de Token JWT Manual)
        // ==========================================================
        [HttpPost("login")]
        public async Task<IActionResult> LoginUser([FromBody] UtilizadorLoginModel utilizador)
        {
            if (utilizador is null || string.IsNullOrWhiteSpace(utilizador.Email) || string.IsNullOrWhiteSpace(utilizador.Password))
            {
                return BadRequest("Por favor, forneça um email e uma palavra-passe.");
            }

            var user = await _userManager.FindByEmailAsync(utilizador.Email);
            if (user is null)
                return BadRequest("Erro: Utilizador não encontrado.");

            var pwdOk = await _signInManager.CheckPasswordSignInAsync(user, utilizador.Password, lockoutOnFailure: false);
            if (!pwdOk.Succeeded)
                return BadRequest("Erro: Login inválido.");

            var roles = await _userManager.GetRolesAsync(user);
            var role = roles.FirstOrDefault() ?? "SemRole";
            var estado = user.Estado ?? "Pendente";

            // Validação de Permissões e Estado
            var roleOk = role.Equals("Cliente", StringComparison.OrdinalIgnoreCase)
                      || role.Equals("Fornecedor", StringComparison.OrdinalIgnoreCase);
                     // || role.Equals("Administrador", StringComparison.OrdinalIgnoreCase)
                     // || role.Equals("Admin", StringComparison.OrdinalIgnoreCase);

            if (!roleOk)
                return StatusCode(StatusCodes.Status403Forbidden, "A sua conta não tem permissão para entrar na aplicação Frontend.");

            if (!role.Contains("Admin") && !estado.Equals("Ativo", StringComparison.OrdinalIgnoreCase))
                return StatusCode(StatusCodes.Status403Forbidden, "Login proíbido. Aguarde que a administração autorize a sua conta.");

            // Configuração da Chave JWT
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["JWT:Key"]!));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(ClaimTypes.Email, user.Email ?? utilizador.Email),
                new Claim(ClaimTypes.Name, user.Nome ?? user.Email ?? "Utilizador"),
                new Claim(ClaimTypes.Role, role),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var jwtToken = new JwtSecurityToken(
                issuer: _config["JWT:Issuer"],
                audience: _config["JWT:Audience"],
                expires: DateTime.UtcNow.AddHours(3),
                signingCredentials: credentials,
                claims: claims
            );

            var jwt = new JwtSecurityTokenHandler().WriteToken(jwtToken);

            return Ok(new
            {
                accesstoken = jwt,
                tokentype = "bearer",
                utilizadorid = user.Id,
                utilizadornome = user.Nome,
                role = role,
                estado = estado
            });
        }

        // ==========================================================
        // REGISTO (Criação de conta com estado Pendente)
        // ==========================================================
        [HttpPost("register")]
        public async Task<IActionResult> RegistarUser([FromBody] RegisterModel utilizador)
        {
            if (utilizador == null || string.IsNullOrEmpty(utilizador.Email) || string.IsNullOrEmpty(utilizador.Password))
            {
                return BadRequest("Dados inválidos.");
            }

            var utilizadorExiste = await _userManager.FindByEmailAsync(utilizador.Email);
            if (utilizadorExiste != null)
                return BadRequest("Já existe um utilizador com este email.");

            var novoUtilizador = new ApplicationUser
            {
                UserName = utilizador.Email,
                Email = utilizador.Email,
                Nome = utilizador.Nome,
                Apelido = utilizador.Apelido,
                NIF = utilizador.NIF,
                Telemovel = utilizador.Telemovel,
                // Mapeamento dos campos individuais
                Rua = utilizador.Rua,
                Localidade = utilizador.Localidade,
                Cidade = utilizador.Cidade,
                Pais = utilizador.Pais,
                CodigoPostal = utilizador.CodigoPostal,
                Estado = "Pendente",
                DataRegisto = DateTime.UtcNow
            };

            var result = await _userManager.CreateAsync(novoUtilizador, utilizador.Password);
            if (!result.Succeeded) return BadRequest(result.Errors);

            await _userManager.AddToRoleAsync(novoUtilizador, "Cliente");

            return Ok(new { Message = "Registo efetuado com sucesso (pendente)." });
        }

        // ==========================================================
        // GET PERFIL
        // ==========================================================
        [HttpGet("perfil")]
        [Authorize]
        public async Task<IActionResult> GetPerfil()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null) return Unauthorized();

            var user = await _userManager.FindByIdAsync(userId);
            return Ok(user);
        }

        // ==========================================================
        // UPDATE PERFIL (Com Morada Agrupada no DTO)
        // ==========================================================
        [HttpPut("perfil")]
        [Authorize(Roles = "Cliente,Fornecedor,Administrador,Admin")]
        public async Task<IActionResult> UpdatePerfil([FromBody] EditarPerfilModel model)
        {
            if (model == null) return BadRequest("Dados inválidos.");

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId)) return Unauthorized();

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) return NotFound("Utilizador não encontrado.");

            // Atualiza dados pessoais
            user.Nome = model.Nome;
            user.Apelido = model.Apelido;
            user.NIF = model.NIF;
            user.Telemovel = model.Telemovel;

            // Atualiza morada a partir do DTO agrupado
            if (model.Morada != null)
            {
                user.Rua = model.Morada.Rua;
                user.Localidade = model.Morada.Localidade;
                user.Cidade = model.Morada.Cidade;
                user.Pais = model.Morada.Pais;
                user.CodigoPostal = model.Morada.CodigoPostal;
            }

            var result = await _userManager.UpdateAsync(user);

            if (result.Succeeded)
            {
                return Ok(new { Message = "Perfil atualizado com sucesso!", User = user.Nome });
            }

            return BadRequest(result.Errors);
        }
        [HttpPost("alterar-password")]
        [Authorize]
        public async Task<IActionResult> AlterarPassword([FromBody] AlterarPasswordModel model)
        {
            if (model.PasswordAtual == model.NovaPassword)
                return BadRequest("A nova password deve ser diferente da atual.");
            
            if (model.NovaPassword != model.ConfirmarNovaPassword)
                return BadRequest("A nova password e a confirmação não coincidem.");

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = await _userManager.FindByIdAsync(userId);

            if (user == null) return NotFound("Utilizador não encontrado.");

            // Esta função do Identity trata de verificar a antiga e hashear a nova
            var result = await _userManager.ChangePasswordAsync(user, model.PasswordAtual, model.NovaPassword);

            if (!result.Succeeded)
            {
                // Retorna o primeiro erro (ex: "Password incorreta", "Password muito curta")
                return BadRequest(result.Errors.FirstOrDefault()?.Description);
            }

            return Ok(new { Message = "Password alterada com sucesso!" });
        }
    }

    // ==========================================================
    // MODELOS DE DADOS (DTOs)
    // ==========================================================

    public class UtilizadorLoginModel
    {
        public string Email { get; set; } = "";
        public string Password { get; set; } = "";
    }

    public class RegisterModel
    {
        public string Nome { get; set; } = "";
        public string Apelido { get; set; } = "";
        public long NIF { get; set; }
        public string Telemovel { get; set; } = "";
        public string Rua { get; set; } = "";
        public string Localidade { get; set; } = "";
        public string CodigoPostal { get; set; } = "";
        public string Cidade { get; set; } = "";
        public string Pais { get; set; } = "";
        public string Email { get; set; } = "";
        public string Password { get; set; } = "";
    }

    public class EditarPerfilModel
    {
        public string Nome { get; set; } = "";
        public string Apelido { get; set; } = "";
        public long NIF { get; set; }
        public string Telemovel { get; set; } = "";
        public MoradaDTO Morada { get; set; } = new MoradaDTO();
    }

    public class MoradaDTO
    {
        public string Rua { get; set; } = "";
        public string Localidade { get; set; } = "";
        public string CodigoPostal { get; set; } = "";
        public string Cidade { get; set; } = "";
        public string Pais { get; set; } = "";
    }
    public class AlterarPasswordModel
    {
        public string PasswordAtual { get; set; } = "";
        public string NovaPassword { get; set; } = "";
        public string ConfirmarNovaPassword { get; set; } = "";
    }
}