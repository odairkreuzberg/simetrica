using System.ComponentModel.DataAnnotations;

namespace RP.Sistema.Web.Models.Material
{
    public class MaterialVM
    {
        public Sistema.Model.Entities.Material GetMaterial()
        {
            var _result = new Sistema.Model.Entities.Material
                {
                    nome = this.nome,
                    idMaterial = this.idMaterial,
                    preco = this.preco,
                    margemGanho = this.margemGanho,
                    nrQuantidade = this.nrQuantidade ?? 0,
                    //idFabricante = this.Fabricante.idFabricante ?? 0,
                    idUnidadeMedida = this.UnidadeMedida.idUnidadeMedida ?? 0

                };
            return _result;
        }

        public static MaterialVM GetMaterial(Sistema.Model.Entities.Material model)
        {
            var _result = new MaterialVM
            {
                nome = model.nome,
                idMaterial = model.idMaterial,
                preco = model.preco,
                nrQuantidade = model.nrQuantidade,
                margemGanho = model.margemGanho,
                UnidadeMedida = Models.UnidadeMedida.Consultar.GetModel(model.UnidadeMedida),
                //Fabricante = Models.Fabricante.Consultar.GetModel(model.Fabricante)
            };

            return _result;
        }
        public int idMaterial { get; set; }

        [Display(Name = "Nome")]
        [Required(ErrorMessage = "Informe o nome")]
        public string nome { get; set; }

        [Display(Name = "Preço (R$)")]
        [Required(ErrorMessage = "Informe o preço")]
        public decimal preco { get; set; }

        [Display(Name = "Margem de ganho (%)")]
        [Required(ErrorMessage = "Informe a margem de ganho")]
        public decimal margemGanho { get; set; }

        [Display(Name = "Saldo")]
        public decimal? nrQuantidade { get; set; }

        public UnidadeMedida.Consultar UnidadeMedida { get; set; }
        //public Fabricante.Consultar Fabricante { get; set; }
    }
}