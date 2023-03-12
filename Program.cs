using System.Text;
using api_rest_dotnet_core.Data;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

internal class Program
{
    private static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
        string chaveDeseguranca = "chave_teste_65437658@";
        var chaveSimetrica = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(chaveDeseguranca));

        // Add services to the container.

        builder.Services.AddControllers();
        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen(config => {
            config.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo { Title="API DE PRODUTOS", Version ="v1"});
        });
        builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));

        // Informando ao sistema que JWT como forma de autenticação
        builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options => {
            // Como o sistema deve ler o Token
            options.TokenValidationParameters = new TokenValidationParameters{
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateIssuerSigningKey = true,
                // Dados de validação de um JWT
                ValidIssuer = "meuSistema.com",
                ValidAudience = "usuario_comum",
                IssuerSigningKey = chaveSimetrica
            };
        });

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseHttpsRedirection();
        app.UseAuthentication(); // Aplica o sistema de autenticação na aplicação
        app.UseAuthorization();

        app.MapControllers();

        app.Run();
    }
}