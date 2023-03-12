using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using api_rest_dotnet_core.Data;
using api_rest_dotnet_core.Models;
using api_rest_dotnet_core.Models.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using api_rest_dotnet_core.HATEOAS;
using Microsoft.AspNetCore.Authorization;

namespace api_rest_dotnet_core.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class ProdutosController : ControllerBase
    {
        private readonly ILogger<ProdutosController> _logger;
        private readonly ApplicationDbContext _database;
        private HATEOAS.HATEOAS HATEOAS;

        public ProdutosController(ILogger<ProdutosController> logger, ApplicationDbContext database)
        {
            _logger = logger;
            _database = database;
            HATEOAS = new HATEOAS.HATEOAS("localhost:5238/api/v1/Produtos");
            HATEOAS.AddAction("GET_INFO", "GET");
            HATEOAS.AddAction("DELETE_PRODUCT", "DELETE");
            HATEOAS.AddAction("EDIT_PRODUCT", "PATCH");
        }

        [HttpGet]
        public IActionResult Get()
        {
            var lista = _database.Produtos?.ToList();
            List<ProdutoContainer> produtosHateoas = new List<ProdutoContainer>();
            foreach (var item in lista)
            {
                ProdutoContainer produtoContainer = new ProdutoContainer();
                produtoContainer.produto = item;
                produtoContainer.links = HATEOAS.GetActions(item.Id.ToString());
                produtosHateoas.Add(produtoContainer);
            }
            return Ok(produtosHateoas);
        }

        [HttpGet("teste-claim")]
        public IActionResult TesteClaims()
        {
            var claim = HttpContext.User.Claims.First(claim => claim.Type.ToString().Equals("id", StringComparison.CurrentCultureIgnoreCase)).Value;
            return Ok(claim);
        }

        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
            try
            {
                var produto = _database.Produtos?.First(p => p.Id == id);
                ProdutoContainer produtoHATEOAS = new ProdutoContainer();
                produtoHATEOAS.produto = produto;
                produtoHATEOAS.links = HATEOAS.GetActions(produto.Id.ToString());
                // return Ok(produto);
                return Ok(produtoHATEOAS);
            }
            catch (Exception)
            {
                return BadRequest(new {msg = "Id inválido"});
            }
        }

        [HttpPatch]
        public IActionResult Patch([FromBody] Produto produtoEnviado)
        {
            if (produtoEnviado.Id > 0)
            {
                try
                {
                    var produto = _database.Produtos?.FirstOrDefault(p => p.Id == produtoEnviado.Id);

                    if (produto != null)
                    {
                        produto.Nome = produtoEnviado.Nome != null ? produtoEnviado.Nome : produto.Nome;
                        produto.Preco = produtoEnviado.Preco >= 0 ? produtoEnviado.Preco : produto.Preco;
                        _database.SaveChanges();
                        return Ok(produto); 
                    } else {
                        Response.StatusCode = 400;
                        return new ObjectResult(new {msg = "Produto não encontrado"});
                    }
                }
                catch (Exception)
                {
                    Response.StatusCode = 400;
                    return new ObjectResult(new {msg = "Produto não encontrado"});
                }
            } else {
                Response.StatusCode = 400;
                return new ObjectResult(new {msg = "O Id é inválido"});
            }
        }

        [HttpPost]
        public IActionResult Post([FromBody] ProdutoDTO p)
        {
            if (p.Preco <= 0)
            {
                Response.StatusCode = 400;
                return new ObjectResult(new {msg = "O preço não pode ser menor ou igual a zero" });
            }

            if (p.Nome.Length <= 1)
            {
                Response.StatusCode = 400;
                return new ObjectResult(new {msg = "Nome deve conter pelo menos uma caracter" });
            }

            Produto produto = new Produto();
            produto.Nome = p.Nome;
            produto.Preco = p.Preco;

            _database.Produtos?.Add(produto);
            _database.SaveChanges();

            Response.StatusCode = 201;
            return new ObjectResult(new { info = "Criado novo produto", produto = p });
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            try
            {
                var produto = _database.Produtos?.FirstOrDefault(p => p.Id == id);
                _database.Produtos?.Remove(produto);
                _database.SaveChanges();
                return Ok(produto);
            }
            catch (Exception)
            {
                return BadRequest(new {msg = "Id inválido"});
            }
        }
    }

}