using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace api_rest_dotnet_core.Models
{
    public class Produto
    {
        public int Id { get; set; }
        public string? Nome { get; set; }
        public float Preco { get; set; }
    }
}