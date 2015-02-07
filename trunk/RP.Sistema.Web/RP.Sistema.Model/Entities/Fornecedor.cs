using System;
using System.Collections.Generic;

namespace RP.Sistema.Model.Entities
{
    public partial class Fornecedor
    {
        public Fornecedor()
        {
            this.ContasPagar = new List<ContaPagar>();
            this.Telefones = new List<FornecedorFone>();
            this.Compras = new List<Compra>();
        }

        public int idFornecedor { get; set; }
        public string nome { get; set; }
        public string tipo { get; set; }
        public string documento{ get; set; }
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
        public ICollection<ContaPagar> ContasPagar { get; set; }
        public ICollection<FornecedorFone> Telefones { get; set; }
        public ICollection<Compra> Compras { get; set; }

        public static string FISICO = "Físico";
        public static string JURIDICO = "Jurídico";
    }
}
