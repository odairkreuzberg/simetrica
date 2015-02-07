using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RP.Sistema.Web.Models.Cidade
{
    public class Consultar
    {
        public int? idCidade { get; set; }
        public string nome { get; set; }

        internal static Consultar GetModel(Model.Entities.Cidade model)
        {
            var _result = new Consultar();

            if (model != null)
            {
                _result.idCidade = model.idCidade;
                _result.nome = model.nome;
            }
            return _result;
        }
    }
}