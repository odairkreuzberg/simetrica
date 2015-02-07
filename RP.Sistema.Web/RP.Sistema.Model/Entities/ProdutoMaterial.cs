using System;
using System.Collections.Generic;

namespace RP.Sistema.Model.Entities
{
    public partial class ProdutoMaterial
    {
        public int idProdutoMaterial { get; set; }
        public int idProduto { get; set; }
        public int idMaterial { get; set; }
        public decimal quantidade { get; set; }
        public decimal margemGanho { get; set; }
        public decimal valor { get; set; }
        public Nullable<int> idCompra { get; set; }
        public Material Material { get; set; }
        public Produto Produto { get; set; }
    }
}
