using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using api_rest_dotnet_core.Data;
using api_rest_dotnet_core.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;

namespace api_rest_dotnet_core.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class UsuariosController : ControllerBase
    {
        private readonly ILogger<UsuariosController> _logger;
        private readonly ApplicationDbContext _database;

        public UsuariosController(ILogger<UsuariosController> logger, ApplicationDbContext database)
        {
            _logger = logger;
            _database = database;
        }

        [HttpPost("registro")]
        public IActionResult Registro([FromBody] Usuario usuario)
        {
            // Em um sistema real deve-se:
            // Verificar se as credencias são válidas
            // Verificar se o e-mail já está cadastrado
            // Encriptar a senha
            _database.Add(usuario);
            _database.SaveChanges();
            return Ok(new {msg = "usuário cadastrado com successo"});
        }

        [HttpPost("login")]
        public IActionResult Login([FromBody] Usuario credenciais)
        {
            try
            {
                Usuario usuario =_database.Usuarios.First(user => user.Email.Equals(credenciais.Email));
                if (usuario != null)
                {
                    if (usuario.Senha.Equals(credenciais.Senha))
                    {
                        string chaveDeseguranca = "chave_teste_65437658@";
                        var chaveSimetrica = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(chaveDeseguranca));
                        var signIn = new SigningCredentials(chaveSimetrica, SecurityAlgorithms.HmacSha256Signature);

                        var claims = new List<Claim>();
                        claims.Add(new Claim("id", usuario.Id.ToString()));
                        claims.Add(new Claim("email", usuario.Email));
                        claims.Add(new Claim(ClaimTypes.Role, "Admin"));

                        var jwt = new JwtSecurityToken(
                            issuer: "meuSistema.com", //Quem está fornecendo o jwt para o usuario
                            expires: DateTime.Now.AddHours(1),
                            audience: "usuario_comum", //token para usuario comum, admin, etc
                            signingCredentials: signIn,
                            claims:  claims
                        );
                        return Ok(new JwtSecurityTokenHandler().WriteToken(jwt));
                    }
                }
            }
            catch (Exception e)
            {
                Response.StatusCode = 401;
                return new ObjectResult("");
            }

            Response.StatusCode = 401;
            return new ObjectResult("");
        }
    }
}