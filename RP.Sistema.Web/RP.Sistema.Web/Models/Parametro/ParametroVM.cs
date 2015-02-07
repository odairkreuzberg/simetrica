using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace RP.Sistema.Web.Models.Parametro
{
    public class ParametroVM
    {
        public Model.Entities.Parametro VM2E()
        {
            var _result = new Model.Entities.Parametro
            {
                nmParametro = nmParametro,
                dsParametro = dsParametro,
                dsValor = dsValor
            };
            return _result;
        }
        public static ParametroVM E2VM(Model.Entities.Parametro model)
        {
            var _result = new ParametroVM
            {
                nmParametro = model.nmParametro,
                dsParametro = model.dsParametro,
                dsValor = model.dsValor
            };
            return _result;
        }


        [Display(Name = "Nome")]
        public string nmParametro { get; set; }

        [Display(Name = "Descrição")]
        public string dsParametro { get; set; }

        [Display(Name = "Valor")]
        public string dsValor { get; set; }

    }
}