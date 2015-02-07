using System.ComponentModel.DataAnnotations;

namespace RP.Sistema.Web.Models.Fabricante
{
    public class FabricanteVM
    {
        public Sistema.Model.Entities.Fabricante GetFabricante()
        {
            var _result = new Sistema.Model.Entities.Fabricante
                {
                    nome = this.nome,
                    idFabricante = this.idFabricante,
                };
            return _result;
        }

        public static FabricanteVM GetFabricante(Sistema.Model.Entities.Fabricante model)
        {
            var _result = new FabricanteVM
            {
                nome = model.nome,
                idFabricante = model.idFabricante,
            };

            return _result;
        }
        public int idFabricante { get; set; }

        [Display(Name = "Nome")]
        [Required(ErrorMessage = "Informe o nome")]
        public string nome { get; set; }
    }
}