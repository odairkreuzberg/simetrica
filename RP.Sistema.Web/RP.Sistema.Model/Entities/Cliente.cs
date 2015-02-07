using System;
using System.Collections.Generic;

namespace RP.Sistema.Model.Entities
{
    public partial class Cliente
    {
        public Cliente()
        {
            this.Telefones = new List<ClienteFone>();
            this.ContasPagar = new List<ContaReceber>();
            this.Projetos = new List<Projeto>();
        }

        public int idCliente { get; set; }
        public string nome { get; set; }
        public string tipo { get; set; }
        public string documento { get; set; }
        public string email { get; set; }
        public string site { get; set; }
        public string observacao { get; set; }
        public string numero { get; set; }
        public string cep { get; set; }
        public string logradouro { get; set; }
        public string bairro { get; set; }
        public int idCidade { get; set; }
        public string contato { get; set; }
        public string foneContato { get; set; }
        public string celularContato { get; set; }
        public Cidade Cidade { get; set; }
        public ICollection<ClienteFone> Telefones { get; set; }
        public ICollection<ContaReceber> ContasPagar { get; set; }
        public ICollection<Projeto> Projetos { get; set; }
        
        public static string FISICO ="Físico";
        public static string JURIDICO = "Jurídico";
    }
}
