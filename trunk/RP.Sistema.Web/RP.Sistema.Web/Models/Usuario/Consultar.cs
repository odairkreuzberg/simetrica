using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RP.Sistema.Web.Models.Usuario
{
    public class Consultar
    {
        public int? idUsuario { get; set; }
        public string nmUsuario { get; set; }

        internal static Consultar GetModel(Model.Entities.Usuario model)
        {
            var _result = new Consultar();

            if (model != null)
            {
                _result.idUsuario = model.idUsuario;
                _result.nmUsuario = model.nmUsuario;
            }
            return _result;
        }
    }
}