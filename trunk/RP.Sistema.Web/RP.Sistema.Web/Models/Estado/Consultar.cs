using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RP.Sistema.Web.Models.Estado
{
    public class Consultar
    {
        public int? idEstado { get; set; }
        public string nome { get; set; }
        public string sigla { get; set; }

        internal static Consultar GetModel(Model.Entities.Estado model)
        {
            var _result = new Consultar();

            if (model != null)
            {
                _result.idEstado = model.idEstado;
                _result.nome = model.nome;
                _result.sigla = model.sigla;
            }
            return _result;
        }
    }
}