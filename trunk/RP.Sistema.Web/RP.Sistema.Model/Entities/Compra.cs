using System;
using System.Collections.Generic;

namespace RP.Sistema.Model.Entities
{
    public partial class Compra
    {
        public Compra()
        {
            this.ItensCompra = new List<ItemCompra>();
            this.ContasPagar = new List<ContaPagar>();
        }
        public int idCompra { get; set; }
        public Nullable<int> idProjeto { get; set; }
        public decimal total { get; set; }
        public System.DateTime dtLancamento { get; set; }
        public int? idFornecedor { get; set; }
        public string descricao { get; set; }
        public string flCancelado { get; set; }
        public int idUsuario { get; set; }
        public Fornecedor Fornecedor { get; set; }
        public Projeto Projeto { get; set; }
        public Usuario Usuario { get; set; }
        public ICollection<ItemCompra> ItensCompra { get; set; }
        public ICollection<ContaPagar> ContasPagar { get; set; }
    }
}
