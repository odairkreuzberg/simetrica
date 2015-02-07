using System.ComponentModel.DataAnnotations;

namespace RP.Sistema.Web.Models.Controle
{
    public class ControleVM
    {
        public Sistema.Model.Entities.Controle VM2E()
        {
            var _result = new Sistema.Model.Entities.Controle 
            { 
                dsControle = this.dsControle,
                idArea = this.Area.idArea ?? 0,
                idControle = this.idControle,
                nmControle = this.nmControle
            };
            return _result;
        }

        public static ControleVM E2VM(Sistema.Model.Entities.Controle model)
        {
            var _result = new ControleVM 
            { 
                idControle = model.idControle,
                dsControle = model.dsControle,
                nmControle = model.nmControle
            };
            if (model.Area != null)
            {
                _result.Area = new Area.Consultar { idArea = model.Area.idArea, nmArea = model.Area.nmArea };
            }
            return _result;
        }

        [Display(Name = "Id. Controle")]
        public int idControle { get; set; }

        [Required(ErrorMessage="Informe o nome do Controle")]
        [Display(Name = "Nome")]
        public string nmControle { get; set; }

        [Display(Name = "Descrição")]
        public string dsControle { get; set; }

        [Display(Name = "Area")]
        public RP.Sistema.Web.Models.Area.Consultar Area { get; set; }
    }
}