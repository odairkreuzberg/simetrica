using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace RP.Sistema.Web.Models.Modulo
{
    public class ModuloVM
    {
        public Sistema.Model.Entities.Modulo VM2E()
        {
            var _result = new Sistema.Model.Entities.Modulo
            {
                idModulo = this.idModulo,
                nmModulo = this.nmModulo,
                dsModulo = this.dsModulo,
                nrOrdem = this.nrOrdem,
                nmURL = this.nmURL
            };
            return _result;
        }

        public static ModuloVM E2VM(Sistema.Model.Entities.Modulo model)
        {
            var _result = new ModuloVM
            {
                idModulo = model.idModulo,
                nmModulo = model.nmModulo,
                dsModulo = model.dsModulo,
                nrOrdem = model.nrOrdem,
                nmURL = model.nmURL
            };
            return _result;
        }

        [Display(Name = "Id. Modulo")]
        public int idModulo { get; set; }
        [Display(Name = "Nome")]
        public string nmModulo { get; set; }
        [Display(Name = "Descrição")]
        public string dsModulo { get; set; }
        [Display(Name = "Ordem")]
        public Nullable<int> nrOrdem { get; set; }
        [Display(Name = "URL")]
        public string nmURL { get; set; }
    }
}