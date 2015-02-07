using System;
using System.Collections.Generic;

namespace RP.Sistema.Model.Entities
{
    public partial class Funcionario
    {
        public Funcionario()
        {
            this.FolhaPagamentos = new List<FolhaPagamento>();
            this.MovimentoProfissionais = new List<MovimentoProfissional>();
            this.ProjetistaProdutos = new List<Produto>();
            this.MarceneiroProdutos = new List<Produto>();
            this.VendedorProjetos = new List<Projeto>();
            this.CartaoPontos = new List<CartaoPonto>();
            this.Requisicoes = new List<Requisicao>();
        }

        public int idFuncionario { get; set; }
        public string nome { get; set; }
        public string tipo { get; set; }
        public string rg { get; set; }
        public string cpf { get; set; }
        public string email { get; set; }
        public string observacao { get; set; }
        public string numero { get; set; }
        public string cep { get; set; }
        public string logradouro { get; set; }
        public string bairro { get; set; }
        public int idCidade { get; set; }
        public int? nrCargaHoraria { get; set; }
        public string fone { get; set; }
        public string celular { get; set; }
        public string flMensalista { get; set; }
        public string ctps { get; set; }
        public Nullable<System.DateTime> dtNascimento { get; set; }
        public Nullable<System.DateTime> dtEntrada { get; set; }
        public Nullable<decimal> salario { get; set; }
        public Nullable<decimal> comissao { get; set; }
        public Nullable<int> idUsuario { get; set; }
        public Nullable<System.DateTime> dtSaida { get; set; }
        public string motivoSaida { get; set; }
        public string status { get; set; }
        public Cidade Cidade { get; set; }
        public ICollection<FolhaPagamento> FolhaPagamentos { get; set; }
        public Usuario Usuario { get; set; }
        public ICollection<MovimentoProfissional> MovimentoProfissionais { get; set; }
        public ICollection<Produto> ProjetistaProdutos { get; set; }
        public ICollection<Produto> MarceneiroProdutos { get; set; }
        public ICollection<Projeto> VendedorProjetos { get; set; }
        public ICollection<Requisicao> Requisicoes { get; set; }

        public static string ATIVO = "Ativo";
        public static string INATIVO = "Inatico";
        public static string FERIAS = "Férias";
        public ICollection<CartaoPonto> CartaoPontos { get; set; }
    }
}
