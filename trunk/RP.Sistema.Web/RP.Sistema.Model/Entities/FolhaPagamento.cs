using System;
using System.Collections.Generic;

namespace RP.Sistema.Model.Entities
{
    public partial class FolhaPagamento
    {
        public FolhaPagamento()
        {
            this.MovimentoProfissionais = new List<MovimentoProfissional>();
        }

        public int idFolhaPagamento { get; set; }
        public int idFuncionario { get; set; }
        public decimal total { get; set; }
        public decimal comissao { get; set; }
        public decimal salario { get; set; }
        public decimal bonificacao { get; set; }
        public decimal outrosDescontos { get; set; }
        public decimal? horaExtra { get; set; }
        public decimal inss { get; set; }
        public decimal vale { get; set; }
        public System.DateTime dtPagamento { get; set; }
        public int nrAno { get; set; }
        public int nrMes { get; set; }
        public string situacao { get; set; }
        public int idUsuario { get; set; }
        public Funcionario Funcionario { get; set; }
        public Usuario Usuario { get; set; }
        public ICollection<MovimentoProfissional> MovimentoProfissionais { get; set; }

        public static string AGUARDANDO_PAGAMENTO = "Aguardando pagamento";
        public static string PAGO = "Pago";
    }
}
