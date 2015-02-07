using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RP.Sistema.Web.Models.Fornecedor
{
    public class Consultar
    {
        public int? idFornecedor { get; set; }
        public string nome { get; set; }

        internal static Consultar GetModel(Model.Entities.Fornecedor Fornecedor)
        {
            var _result = new Consultar();

            if(Fornecedor !=null)
            {
                _result.idFornecedor = Fornecedor.idFornecedor;
                _result.nome = Fornecedor.nome;
            }
            return _result;
        }
    }
}