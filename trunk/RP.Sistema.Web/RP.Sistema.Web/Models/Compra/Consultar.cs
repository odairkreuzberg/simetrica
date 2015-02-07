using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RP.Sistema.Web.Models.Compra
{
    public class Consultar
    {
        public int? idCompra { get; set; }
        public string descricao { get; set; }

        internal static Consultar GetModel(Model.Entities.Compra model)
        {
            var _result = new Consultar();

            if (model != null)
            {
                _result.idCompra = model.idCompra;
                _result.descricao = model.descricao;
            }
            return _result;
        }
    }
}