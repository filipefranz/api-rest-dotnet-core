using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api_rest_dotnet_core.HATEOAS;

namespace api_rest_dotnet_core.Models
{
    public class ProdutoContainer
    {
        public Produto produto { get; set; }
        public Link[] links { get; set; }
    }
}