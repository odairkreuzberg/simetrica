using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace RP.Sistema.Web.Models.Entidade
{
    public class EntidadeVM
    {
        public Sistema.Model.Entities.Entidade VM2E()
        {
            var _result = new Sistema.Model.Entities.Entidade
            {
                idEntidade = this.idEntidade ?? 0,
                nmRazaoSocial = this.nmRazaoSocial,
                nmFantasia = this.nmFantasia,
                nrCNPJ = this.nrCNPJ,
                dsCidade = this.dsCidade,
                dsBairro = this.dsBairro,
                dsLogradouro = this.dsLogradouro,
                nrEndereco = this.nrEndereco,
                nrCEP = this.nrCEP,
                nrTelefone = this.nrTelefone,
                dsWebSite = this.dsWebSite,
                dsEmail = this.dsEmail

            };
            return _result;
        }

        public static EntidadeVM E2VM(Sistema.Model.Entities.Entidade model)
        {
            var _result = new EntidadeVM
            {
                idEntidade = model.idEntidade,
                nmRazaoSocial = model.nmRazaoSocial,
                nmFantasia = model.nmFantasia,
                nrCNPJ = model.nrCNPJ,
                dsCidade = model.dsCidade,
                dsBairro = model.dsBairro,
                dsLogradouro = model.dsLogradouro,
                nrEndereco = model.nrEndereco,
                nrCEP = model.nrCEP,
                nrTelefone = model.nrTelefone,
                dsWebSite = model.dsWebSite,
                dsEmail = model.dsEmail,

            };
            return _result;
        }

        [Display(Name = "Id. Entidade")]
        public int? idEntidade { get; set; }

        [Display(Name = "Razão Social")]
        [Required(ErrorMessage = "Informe a razão social")]
        public string nmRazaoSocial { get; set; }

        [Display(Name = "Nome Fantasia")]
        [Required(ErrorMessage = "Informe o nome fantasia")]
        public string nmFantasia { get; set; }

        [Display(Name = "CNPJ")]
        [Required(ErrorMessage = "Informe o CNPJ")]
        public string nrCNPJ { get; set; }

        [Display(Name = "Cidade")]
        [Required(ErrorMessage = "Informe a cidade")]
        public string dsCidade { get; set; }

        [Display(Name = "Bairro")]
        [Required(ErrorMessage = "Informe o bairro")]
        public string dsBairro { get; set; }

        [Display(Name = "Logradouro")]
        [Required(ErrorMessage = "Informe o logradouro")]
        public string dsLogradouro { get; set; }

        [Display(Name = "Número")]
        [Required(ErrorMessage = "Informe o número")]
        public string nrEndereco { get; set; }

        [Display(Name = "CEP")]
        [Required(ErrorMessage = "Informe o CEP")]
        public string nrCEP { get; set; }

        [Display(Name = "Telefone")]
        [Required(ErrorMessage = "Informe o telefone")]
        public string nrTelefone { get; set; }

        [Display(Name = "Site")]
        public string dsWebSite { get; set; }

        [Display(Name = "e-mail")]
        public string dsEmail { get; set; }

        public byte imLogo { get; set; }

        public string pathLogo { get; set; }
    }
}