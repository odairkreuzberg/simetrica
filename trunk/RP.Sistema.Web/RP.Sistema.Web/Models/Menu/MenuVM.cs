using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace RP.Sistema.Web.Models.Menu
{
    public class MenuVM
    {
        public Sistema.Model.Entities.Menu VM2E()
        {
            var _result = new Sistema.Model.Entities.Menu
            {
                idMenu = this.idMenu,
                nmMenu = this.nmMenu,
                dsCor = this.dsCor,
                nrOrdem = this.nrOrdem
            };
            return _result;
        }

        public static MenuVM E2VM(Sistema.Model.Entities.Menu model)
        {
            var _result = new MenuVM
            {
                idMenu = model.idMenu,
                nmMenu = model.nmMenu,
                dsCor = model.dsCor,
                nrOrdem = model.nrOrdem ?? 0
            };
            return _result;
        }

        [Display(Name = "Id. Menu")]
        public int idMenu { get; set; }

        [Required(ErrorMessage="Informe o nome do Menu")]
        [Display(Name = "Nome")]
        public string nmMenu { get; set; }

        [Required(ErrorMessage = "Informe a ordem do Menu")]
        [Display(Name = "Ordem")]
        public int nrOrdem { get; set; }

        [Display(Name = "Cor")]
        public string dsCor { get; set; }
    }
}