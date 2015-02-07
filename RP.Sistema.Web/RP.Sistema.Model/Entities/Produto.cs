using System;
using System.Collections.Generic;

namespace RP.Sistema.Model.Entities
{
    public partial class Produto
    {
        public Produto()
        {
            this.ProdutoMateriais = new List<ProdutoMaterial>();
        }

        public int idProduto { get; set; }
        public int idProjeto { get; set; }
        public string nome { get; set; }
        public string descricao { get; set; }
        public int? idProjetista { get; set; }
        public int? idMarceneiro { get; set; }
        public Nullable<decimal> vlVenda { get; set; }
        public Nullable<decimal> vlDesconto { get; set; }
        public Nullable<decimal> vlProduto { get; set; }
        public Nullable<decimal> porcentagemMarceneiro { get; set; }
        public Nullable<decimal> porcentagemProjetista { get; set; }
        public Nullable<decimal> margemGanho { get; set; }
        public Funcionario Projetista { get; set; }
        public Funcionario Marceneiro { get; set; }
        public Projeto projeto { get; set; }
        public ICollection<ProdutoMaterial> ProdutoMateriais { get; set; }
    }
}
