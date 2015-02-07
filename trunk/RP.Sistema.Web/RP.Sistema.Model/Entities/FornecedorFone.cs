using System;
using System.Collections.Generic;

namespace RP.Sistema.Model.Entities
{
    public partial class FornecedorFone
    {
        public int idFornecedorFone { get; set; }
        public int idFornecedor { get; set; }
        public string tipo { get; set; }
        public string numero { get; set; }
        public Fornecedor Fornecedor { get; set; }
    }
}
