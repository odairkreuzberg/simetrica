using System;
using System.Collections.Generic;

namespace RP.Sistema.Model.Entities
{
    public partial class Material
    {
        public Material()
        {
            this.ProdutoMateriais = new List<ProdutoMaterial>();
            this.ItensCompra = new List<ItemCompra>();
            this.RequisicaoItens = new List<RequisicaoItem>();
        }

        public int idMaterial { get; set; }
        public string nome { get; set; }
        public decimal preco { get; set; }
        public decimal margemGanho { get; set; }
        public int idUnidadeMedida { get; set; }
        public int? idFabricante { get; set; }
        public decimal nrQuantidade { get; set; }
        public Fabricante Fabricante { get; set; }
        public UnidadeMedida UnidadeMedida { get; set; }
        public ICollection<ProdutoMaterial> ProdutoMateriais { get; set; }
        public ICollection<ItemCompra> ItensCompra { get; set; }
        public virtual ICollection<RequisicaoItem> RequisicaoItens { get; set; }
    }
}
