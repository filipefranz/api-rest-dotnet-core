using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace api_rest_dotnet_core.Models.DTOs
{
    public class ProdutoDTO
    {
        public string? Nome { get; set; }
        public float Preco { get; set; }
    }
}