using System.ComponentModel.DataAnnotations;

namespace RP.Sistema.Web.Models.Estado
{
    public class EstadoVM
    {
        public Sistema.Model.Entities.Estado GetEstado()
        {
            var _result = new Sistema.Model.Entities.Estado
                {
                    nome = this.nome,
                    idEstado = this.idEstado,
                    sigla = this.sigla,
                    idPais = this.Pais.idPais ?? 0,
                };
            return _result;
        }

        public static EstadoVM GetEstado(Sistema.Model.Entities.Estado model)
        {
            var _result = new EstadoVM
            {
                nome = model.nome,
                idEstado = model.idEstado,
                sigla = model.sigla,
                Pais = Models.Pais.Consultar.GetModel(model.Pais)
            };

            return _result;
        }
        public int idEstado { get; set; }

        [Display(Name = "Nome")]
        [Required(ErrorMessage = "Informe o nome")]
        public string nome { get; set; }

        [Display(Name = "Sigla")]
        [Required(ErrorMessage = "Informe a sigla")]
        public string sigla { get; set; }

        public Pais.Consultar Pais { get; set; }
    }
}