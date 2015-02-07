using System.ComponentModel.DataAnnotations;

namespace RP.Sistema.Web.Models.Cidade
{
    public class CidadeVM
    {
        public Sistema.Model.Entities.Cidade GetCidade()
        {
            var _result = new Sistema.Model.Entities.Cidade
                {
                    nome = this.nome,
                    idCidade = this.idCidade,
                    idEstado = this.Estado.idEstado ?? 0,
                };
            return _result;
        }

        public static CidadeVM GetCidade(Sistema.Model.Entities.Cidade model)
        {
            var _result = new CidadeVM
            {
                nome = model.nome,
                idCidade = model.idCidade,
                Estado = Models.Estado.Consultar.GetModel(model.Estado)
            };

            return _result;
        }
        public int idCidade { get; set; }

        [Display(Name = "Nome")]
        [Required(ErrorMessage = "Informe o nome")]
        public string nome { get; set; }

        public Estado.Consultar Estado { get; set; }
    }
}