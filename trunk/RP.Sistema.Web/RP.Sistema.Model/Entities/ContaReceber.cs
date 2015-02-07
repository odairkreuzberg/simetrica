using System;
using System.Collections.Generic;

namespace RP.Sistema.Model.Entities
{
    public partial class ContaReceber
    {
        public ContaReceber()
        {
            this.Caixas = new List<Caixa>();
        }

        public int idContaReceber { get; set; }
        public int idCliente { get; set; }
        public int parcela { get; set; }
        public string descricao { get; set; }
        public string flFormaPagamento { get; set; }
        public System.DateTime vencimento { get; set; }
        public Nullable<System.DateTime> pagamento { get; set; }
        public decimal valorConta { get; set; }
        public decimal? valorPago { get; set; }
        public string situacao { get; set; }
        public Nullable<int> idCompra { get; set; }
        public Nullable<int> idOrigem { get; set; }
        public int idUsuario { get; set; }
        public int? idProjeto { get; set; }
        public ICollection<Caixa> Caixas { get; set; }
        public Cliente Cliente { get; set; }
        public Projeto Projeto { get; set; }

        public static string SITUACAO_AGUARDANDO_PAGAMENTO = "Aguardando pagamento";
        public static string SITUACAO_PAGO = "Pago";
        public static string SITUACAO_CANCELADO = "Cancelado";
    }
}
