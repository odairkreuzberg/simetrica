using System;
using System.Collections.Generic;

namespace RP.Sistema.Model.Entities
{
    public partial class ItemCompra
    {

        public int idItemCompra { get; set; }
        public int idMaterial { get; set; }
        public int idCompra { get; set; }
        public decimal quantidade { get; set; }
        public decimal valor { get; set; }

        public Material Material { get; set; }
        public Compra Compra { get; set; }
    }
}
