using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace RP.Sistema.Web.Models.Acao
{
    public class AcaoVM
    {
        public Sistema.Model.Entities.Acao VM2E()
        {
            var _result = new Sistema.Model.Entities.Acao
                {
                    dsAcao = this.dsAcao,
                    flMenu = this.flMenu,
                    idAcao = this.idAcao,
                    idControle = this.Controle.idControle ?? 0,
                    idMenu = this.Menu.idMenu,
                    nmAcao = this.nmAcao,
                    nmMenu = this.nmMenu,
                    nrOrdem = this.nrOrdem,
                    dsIcone = this.dsIcone,
                };
            return _result;
        }

        public static AcaoVM E2VM(Sistema.Model.Entities.Acao model)
        {
            var _result = new AcaoVM
                {
                    Controle = new Controle.Consultar { idControle = model.idControle, dsControle = model.Controle.dsControle, nmControle = model.Controle.nmControle },
                    dsAcao = model.dsAcao,
                    flMenu = model.flMenu,
                    idAcao = model.idAcao,
                    nmAcao = model.nmAcao,
                    nmMenu = model.nmMenu,
                    nrOrdem = model.nrOrdem,
                    dsIcone = model.dsIcone
                };
            if (model.idMenu != null)
            {
                _result.Menu = new Menu.Consultar { idMenu = model.idMenu, nmMenu = model.Menu.nmMenu, dsCor = model.Menu.dsCor };
            }

            return _result;
        }

        [Display(Name = "Id Ação")]
        public int idAcao { get; set; }

        [Display(Name = "Nome")]
        public string nmAcao { get; set; }

        [Display(Name = "Descrição")]
        public string dsAcao { get; set; }

        [Display(Name = "Exibir no Menu?")]
        public string flMenu { get; set; }

        [Display(Name = "Nome do Menu")]
        public string nmMenu { get; set; }

        [Display(Name = "Controle")]
        public RP.Sistema.Web.Models.Controle.Consultar Controle { get; set; }

        [Display(Name = "Item de menu")]
        public  RP.Sistema.Web.Models.Menu.Consultar Menu { get; set; }

        [Display(Name = "Ordem")]
        public int? nrOrdem { get; set; }

        [Display(Name = "Ícone")]
        public string dsIcone { get; set; }

        public List<string> listaIcones { get; set; }
    }
}