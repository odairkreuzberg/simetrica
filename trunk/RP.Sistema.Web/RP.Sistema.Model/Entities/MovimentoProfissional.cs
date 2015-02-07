using System;
using System.Collections.Generic;

namespace RP.Sistema.Model.Entities
{
    public partial class MovimentoProfissional
    {
        public MovimentoProfissional()
        {
            this.Caixas = new List<Caixa>();
        }
        public int idMovimento { get; set; }
        public string tipo { get; set; }
        public string situacao { get; set; }
        public int idFuncionario { get; set; }
        public Nullable<int> idFolhaPagamento { get; set; }
        public string descricao { get; set; }
        public decimal valor { get; set; }
        public System.DateTime dtLancamento { get; set; }
        public System.DateTime dtVencimento { get; set; }
        public Nullable<int> idProjeto { get; set; }
        public int idUsuario { get; set; }
        public FolhaPagamento FolhaPagamento { get; set; }
        public Funcionario Funcionario { get; set; }
        public Projeto Projeto { get; set; }
        public Usuario Usuario { get; set; }
        public List<Caixa> Caixas { get; set; }

        public static string TIPO_COMISSAO = "Comissão";
        public static string TIPO_VALE = "Vale";

        public static string SITUACAO_PENDENTE = "Pendente";
        public static string SITUACAO_PAGO = "Pago";
        public static string SITUACAO_CANCELADO = "Cancelado";
        public static string SITUACAO_AGUARDANDO_PAGAMENTO = "Aguardando pagamento";
    }
}
