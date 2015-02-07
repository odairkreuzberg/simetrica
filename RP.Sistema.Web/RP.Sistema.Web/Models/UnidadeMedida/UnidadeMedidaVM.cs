using System.ComponentModel.DataAnnotations;

namespace RP.Sistema.Web.Models.UnidadeMedida
{
    public class UnidadeMedidaVM
    {
        public Sistema.Model.Entities.UnidadeMedida GetUnidadeMedida()
        {
            var _result = new Sistema.Model.Entities.UnidadeMedida
                {
                    nome = this.nome,
                    idUnidadeMedida = this.idUnidadeMedida,
                    abreviatura = this.abreviatura
                };
            return _result;
        }

        public static UnidadeMedidaVM GetUnidadeMedida(Sistema.Model.Entities.UnidadeMedida model)
        {
            var _result = new UnidadeMedidaVM
            {
                nome = model.nome,
                idUnidadeMedida = model.idUnidadeMedida,
                abreviatura = model.abreviatura
            };

            return _result;
        }
        public int idUnidadeMedida { get; set; }

        [Display(Name = "Nome")]
        [Required(ErrorMessage = "Informe o nome")]
        public string nome { get; set; }

        [Display(Name = "Abreviatura")]
        [Required(ErrorMessage = "Informe a abreviatura")]
        public string abreviatura { get; set; }
    }
}