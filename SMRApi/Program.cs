using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using SMRApi.Repositories;
using SMRInfraestrutura;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

var key = Encoding.ASCII.GetBytes("SmrAppUelerBernardoLuizFelipeTCC");

// --- 1. CONFIGURAÇÃO DOS SERVIÇOS (Injeção de Dependência) ---

builder.Services.AddAuthentication(x =>
{
    x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(x =>
{
    x.RequireHttpsMetadata = false;
    x.SaveToken = true;
    x.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(key),
        ValidateIssuer = false,
        ValidateAudience = false
    };
});

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "SMR-API", Version = "v1" });

    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header usando o esquema Bearer. Exemplo: \"Authorization: Bearer {token}\"",
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
            new string[] {}
        }
    });
});

var connectionString = builder.Configuration.GetConnectionString("MariaDb");

builder.Services.AddDbContext<SMRDBContext>(options =>
{
    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString));
});

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddScoped<RelatoriosRepository>(provider =>
    new RelatoriosRepository(connectionString));

// Configuração do CORS (Movida para ANTES do builder.Build())
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSMRWeb", policy =>
    {
        policy.WithOrigins(
            "https://smrapp.com.br",
            "https://www.smrapp.com.br",
            "https://smr-app-virid.vercel.app"
        )
        .AllowAnyHeader()
        .AllowAnyMethod();
    });
});

// --- 2. CONSTRUÇÃO DA APLICAÇÃO ---
var app = builder.Build();

// --- 3. PIPELINE DE REQUISIÇÕES (Middlewares) ---
app.UseStaticFiles();
app.UseRouting();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// Middleware do CORS (Sempre após UseRouting e antes de UseAuthentication)
app.UseCors("AllowSMRWeb");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();