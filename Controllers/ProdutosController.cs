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

namespace api_rest_dotnet_core.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProdutosController : ControllerBase
    {
        private readonly ILogger<ProdutosController> _logger;
        private readonly ApplicationDbContext _database;

        public ProdutosController(ILogger<ProdutosController> logger, ApplicationDbContext database)
        {
            _logger = logger;
            _database = database;
        }

        [HttpGet]
        public IActionResult Get()
        {
            var lista = _database.Produtos?.ToList();
            return Ok(lista);
        }

        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
            try
            {
                var produto = _database.Produtos?.First(p => p.Id == id);
                return Ok(produto);
            }
            catch (Exception)
            {
                return BadRequest(new {msg = "Id inválido"});
            }
        }

        [HttpPost]
        public IActionResult Post([FromBody] ProdutoDTO p)
        {
            Produto produto = new Produto();
            produto.Nome = p.Nome;
            produto.Preco = p.Preco;

            _database.Produtos?.Add(produto);
            _database.SaveChanges();

            Response.StatusCode = 201;
            return new ObjectResult(new { info = "Criado novo produto", produto = p });
            // return Ok(new { info = "Criado novo produto", produto = p });
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