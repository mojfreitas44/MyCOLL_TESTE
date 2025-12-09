using API.Data;
using API.Repositories;
using API.Entities;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// ==============================================================================
// 1. CONFIGURAÇÃO DE CONTROLADORES E JSON
// ==============================================================================
builder.Services.AddControllers().AddJsonOptions(options =>
{
    // Ignora ciclos (ex: Categoria -> SubCategoria) para não bloquear o JSON
    options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
});

// ==============================================================================
// 2. CONFIGURAÇÃO DA BASE DE DADOS
// ==============================================================================
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));

// ==============================================================================
// 3. CONFIGURAÇÃO DO IDENTITY (LOGIN/USERS)
// ==============================================================================
builder.Services.AddIdentityCore<ApplicationUser>(options =>
{
    // Configurações de password mais relaxadas para testes (opcional)
    options.SignIn.RequireConfirmedAccount = false;
    options.User.RequireUniqueEmail = true;
    options.Password.RequireDigit = false;
    options.Password.RequireLowercase = false;
    options.Password.RequireUppercase = false;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequiredLength = 3;
})
.AddRoles<IdentityRole>()
.AddEntityFrameworkStores<ApplicationDbContext>()
.AddDefaultTokenProviders();

// ==============================================================================
// 4. AUTENTICAÇÃO JWT (TOKEN)
// ==============================================================================
var jwtKey = builder.Configuration["JWT:Key"] ?? "ChaveSuperSecretaDeDesenvolvimento123456789";
var jwtIssuer = builder.Configuration["JWT:Issuer"] ?? "GestaoLojaApi";
var jwtAudience = builder.Configuration["JWT:Audience"] ?? "GestaoLojaClient";

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtIssuer,
            ValidAudience = jwtAudience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
        };
    });

builder.Services.AddAuthorization();

// ==============================================================================
// 5. REGISTO DE REPOSITÓRIOS (DEPENDENCY INJECTION)
// ==============================================================================
// Aqui registamos todos os serviços que a API vai usar
builder.Services.AddScoped<ICategoriaRepository, CategoriaRepository>();
builder.Services.AddScoped<ICarrinhoRepository, CarrinhoRepository>();
builder.Services.AddScoped<IEncomendaRepository, EncomendaRepository>();
builder.Services.AddScoped<IProdutoRepository, ProdutoRepository>();

// ==============================================================================
// 6. SWAGGER COM SUPORTE A JWT (O CADEADO)
// ==============================================================================
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "GestaoLoja API", Version = "v1" });

    // Configuração para aparecer o botão "Authorize" no Swagger
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "Insira o token JWT desta forma: Bearer {seu_token}",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

// ==============================================================================
// 7. CORS (PERMITIR ACESSO DO FRONTEND)
// ==============================================================================
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

var app = builder.Build();

// ==============================================================================
// PIPELINE DE EXECUÇÃO
// ==============================================================================

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

// Ativar CORS
app.UseCors("AllowAll");

// Ativar Autenticação e Autorização (A ordem importa!)
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();