using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RP.Sistema.Web.Models.Cliente
{
    public class Consultar
    {
        public int? idCliente { get; set; }
        public string nome { get; set; }

        internal static Consultar GetModel(Model.Entities.Cliente Cliente)
        {
            var _result = new Consultar();

            if(Cliente !=null)
            {
                _result.idCliente = Cliente.idCliente;
                _result.nome = Cliente.nome;
            }
            return _result;
        }
    }
}