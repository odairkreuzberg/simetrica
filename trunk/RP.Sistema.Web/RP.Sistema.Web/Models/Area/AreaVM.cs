using System.ComponentModel.DataAnnotations;

namespace RP.Sistema.Web.Models.Area
{
    public class AreaVM
    {
        public Sistema.Model.Entities.Area VM2E()
        {
            var _result = new Sistema.Model.Entities.Area 
            {
                dsArea = this.dsArea,
                flUsaURL = this.flUsaURL,
                idArea = this.idArea,
                idModulo = this.Modulo.idModulo ?? 0,
                nmArea = this.nmArea
            };

            return _result;
        }



        public static AreaVM E2VM(Sistema.Model.Entities.Area model)
        {
            var _result = new AreaVM
            {
                dsArea = model.dsArea,
                flUsaURL = model.flUsaURL,
                idArea = model.idArea,
                Modulo = new Modulo.Consultar {idModulo = model.idModulo, nmModulo = model.Modulo.nmModulo },
                nmArea = model.nmArea
            };
            return _result;
        }


        [Display(Name = "Id. Area")]
        public int idArea { get; set; }

        [StringLength(100, ErrorMessage = "Tamanho limite do campo excedido.")]
        [Display(Name = "Nome")]
        public string nmArea { get; set; }
        
        [Display(Name = "Descrição")]
        public string dsArea { get; set; }

        [Display(Name = "Modulo")]
        public RP.Sistema.Web.Models.Modulo.Consultar Modulo { get; set; }

        [Display(Name = "Adicionar na URL?")]
        public string flUsaURL { get; set; }
    }
}