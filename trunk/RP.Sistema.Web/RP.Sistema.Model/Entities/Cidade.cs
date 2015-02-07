using System;
using System.Collections.Generic;

namespace RP.Sistema.Model.Entities
{
    public partial class Cidade
    {
        public Cidade()
        {
            this.Clientes = new List<Cliente>();
            this.Fornecedores = new List<Fornecedor>();
            this.Funcionarios = new List<Funcionario>();
        }

        public int idCidade { get; set; }
        public string nome { get; set; }
        public Nullable<int> idEstado { get; set; }
        public Estado Estado { get; set; }
        public ICollection<Cliente> Clientes { get; set; }
        public ICollection<Fornecedor> Fornecedores { get; set; }
        public ICollection<Funcionario> Funcionarios { get; set; }
    }
}
