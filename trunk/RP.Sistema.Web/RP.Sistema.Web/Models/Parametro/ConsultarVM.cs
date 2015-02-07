using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RP.Sistema.Web.Models.Parametro
{
    public class ConsultarVM
    {
        public string nmParametro { get; set; }
        public string dsParametro { get; set; }
        public string dsValor { get; set; }

        public static ConsultarVM E2VM(Model.Entities.Parametro model)
        {
            if (model != null)
            {
                var _result = new ConsultarVM
                {
                    nmParametro = model.nmParametro,
                    dsParametro = model.dsParametro,
                    dsValor = model.dsValor
                };
                return _result;
            }
            return new ConsultarVM();
        }
    }
}