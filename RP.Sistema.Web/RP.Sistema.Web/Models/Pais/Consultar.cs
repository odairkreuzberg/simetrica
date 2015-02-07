using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RP.Sistema.Web.Models.Pais
{
    public class Consultar
    {
        public int? idPais { get; set; }
        public string nome { get; set; }
        public string sigla { get; set; }

        internal static Consultar GetModel(Model.Entities.Pais pais)
        {
            var _result = new Consultar();

            if(pais !=null)
            {
                _result.idPais = pais.idPais;
                _result.nome = pais.nome;
                _result.sigla = pais.sigla;
            }
            return _result;
        }
    }
}