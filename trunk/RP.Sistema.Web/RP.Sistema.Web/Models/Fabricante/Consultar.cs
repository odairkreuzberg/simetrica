using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RP.Sistema.Web.Models.Fabricante
{
    public class Consultar
    {
        public int? idFabricante { get; set; }
        public string nome { get; set; }

        internal static Consultar GetModel(Model.Entities.Fabricante model)
        {
            var _result = new Consultar();

            if(model !=null)
            {
                _result.idFabricante = model.idFabricante;
                _result.nome = model.nome;
            }
            return _result;
        }
    }
}