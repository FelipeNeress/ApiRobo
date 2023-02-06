using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApiRobo.Domain.Models
{
    public class Produto
    {
        public string? Titulo { get; set; }
        public decimal Preco { get; set; }
        public decimal? PrecoAntigo { get; set; }
        public string? Link { get; set; }
        public DateTime DataBusca { get; set; } = DateTime.Now;
    }
}
