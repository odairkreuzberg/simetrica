using System.ComponentModel.DataAnnotations;

namespace RP.Sistema.Web.Models.Pais
{
    public class PaisVM
    {
        public Sistema.Model.Entities.Pais GetPais()
        {
            var _result = new Sistema.Model.Entities.Pais
                {
                    nome = this.nome,
                    idPais = this.idPais,
                    sigla = this.sigla
                };
            return _result;
        }

        public static PaisVM GetPais(Sistema.Model.Entities.Pais model)
        {
            var _result = new PaisVM
            {
                nome = model.nome,
                idPais = model.idPais,
                sigla = model.sigla
            };

            return _result;
        }
        public int idPais { get; set; }

        [Display(Name = "Nome")]
        [Required(ErrorMessage = "Informe o nome")]
        public string nome { get; set; }

        [Display(Name = "Sigla")]
        [Required(ErrorMessage = "Informe a sigla")]
        public string sigla { get; set; }
    }
}