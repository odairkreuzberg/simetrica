using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RP.Sistema.Web.Models.UnidadeMedida
{
    public class Consultar
    {
        public int? idUnidadeMedida { get; set; }
        public string nome { get; set; }
        public string abreviatura { get; set; }

        internal static Consultar GetModel(Model.Entities.UnidadeMedida model)
        {
            var _result = new Consultar();

            if(model !=null)
            {
                _result.idUnidadeMedida = model.idUnidadeMedida;
                _result.nome = model.nome;
                _result.abreviatura = model.abreviatura;
            }
            return _result;
        }
    }
}