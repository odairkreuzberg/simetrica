using System;
using System.Collections.Generic;

namespace RP.Sistema.Model.Entities
{
    public partial class Projeto
    {
        public Projeto()
        {
            this.Compras = new List<Compra>();
            this.MovimentoProfissionais = new List<MovimentoProfissional>();
            this.Produtos = new List<Produto>();
            this.ProjetoCustos = new List<ProjetoCusto>();
            this.ContasReceber = new List<ContaReceber>();
            this.ContasPagar = new List<ContaPagar>();
            this.Requisicoes = new List<Requisicao>();
        }

        public int idProjeto { get; set; }
        public int idCliente { get; set; }
        public int idVendedor { get; set; }
        public int idUsuario { get; set; }
        public string descricao { get; set; }
        public string flConcluido { get; set; }
        public Nullable<decimal> vlVenda { get; set; }
        public Nullable<decimal> vlDesconto { get; set; }
        public Nullable<decimal> vlProjeto { get; set; }
        public Nullable<decimal> porcentagemVendedor { get; set; }
        public string status { get; set; }
        public Nullable<System.DateTime> dtInicio { get; set; }
        public Nullable<System.DateTime> dtFim { get; set; }
        public string dsObservacao { get; set; }
        public string dsGarantia { get; set; }
        public string dsPrevisao { get; set; }
        public string dsIncluso { get; set; }
        public string dsValidade { get; set; }
        public string dsCondicao { get; set; }

        public Cliente Cliente { get; set; }
        public ICollection<Compra> Compras { get; set; }
        public Funcionario Vendedor { get; set; }
        public ICollection<MovimentoProfissional> MovimentoProfissionais { get; set; }
        public ICollection<Produto> Produtos { get; set; }
        public Usuario Usuario { get; set; }
        public ICollection<ProjetoCusto> ProjetoCustos { get; set; }
        public ICollection<ContaReceber> ContasReceber { get; set; }
        public ICollection<ContaPagar> ContasPagar { get; set; }
        public ICollection<Requisicao> Requisicoes { get; set; }

        public static string ORCAMENTO = "Orçamento";
        public static string CONCLUIDO = "Concluído";
        public static string PRODUCAO = "Produção";
        public static string VENDIDO = "Vendido";
        public static string NAO_VENDIDO = "Não vendido";
        public static string CANCELADO = "Cancelado";
    }
}
