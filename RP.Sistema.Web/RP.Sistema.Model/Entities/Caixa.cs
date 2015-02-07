using System;
using System.Collections.Generic;

namespace RP.Sistema.Model.Entities
{
    public partial class Caixa
    {
        public int idCaixa { get; set; }
        public Nullable<int> idContaEeceber { get; set; }
        public Nullable<int> idContaPagar { get; set; }
        public Nullable<int> idMovimento { get; set; }
        public Nullable<int> idCaixaExtorno { get; set; }
        public string situacao { get; set; }
        public decimal valor { get; set; }
        public decimal saldoAnterior { get; set; }
        public decimal saldoAtual { get; set; }
        public int idUsuario { get; set; }
        public string descricao { get; set; }
        public System.DateTime dtLancamento { get; set; }
        public ContaPagar ContaPagar { get; set; }
        public ContaReceber ContaReceber { get; set; }
        public MovimentoProfissional MovimentoProfissional { get; set; }

        public static string CORENTE = "Corrente";
        public static string CANCELADO = "Cancelado";
        public static string EXTORNO = "Extorno";
    }
}
