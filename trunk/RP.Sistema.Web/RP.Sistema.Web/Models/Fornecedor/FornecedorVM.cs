using RP.Sistema.Web.Models.Telefone;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using System.Linq;

namespace RP.Sistema.Web.Models.Fornecedor
{
    public class FornecedorVM
    {
        public static readonly SelectListItem[] Tipo = new[]
        {
            new SelectListItem { Text = "Físico", Value = "Físico" }, 
            new SelectListItem { Text = "Jurídico", Value = "Jurídico" }
        };

        public Sistema.Model.Entities.Fornecedor GetFornecedor()
        {
            var _result = new Sistema.Model.Entities.Fornecedor
                {
                    nome = this.nome,
                    idFornecedor = this.idFornecedor,
                    tipo = this.tipo,
                    documento = this.tipo == Sistema.Model.Entities.Fornecedor.FISICO ? this.cpf : this.cnpj,
                    celularContato = this.celularContato,
                    bairro = this.bairro,
                    contato = this.contato,
                    cep = this.cep,
                    email = this.email,
                    foneContato = this.foneContato,
                    idCidade = this.Cidade.idCidade ?? 0,
                    logradouro = this.logradouro,
                    numero = this.numero,
                    observacao = this.observacao,
                    site = this.site,
                    Telefones = TelefoneVM.GetTelefonesFornecedor(this.Telefones)
                };
            return _result;
        }

        public static FornecedorVM GetFornecedor(Sistema.Model.Entities.Fornecedor model)
        {
            var _result = new FornecedorVM
            {
                nome = model.nome,
                idFornecedor = model.idFornecedor,
                tipo = model.tipo,
                cpf = model.documento,
                cnpj = model.documento,
                celularContato = model.celularContato,
                bairro = model.bairro,
                contato = model.contato,
                cep = model.cep,
                email = model.email,
                foneContato = model.foneContato,
                Cidade = Models.Cidade.Consultar.GetModel(model.Cidade),
                logradouro = model.logradouro,
                numero = model.numero,
                observacao = model.observacao,
                site = model.site,
                Telefones = TelefoneVM.GetTelefonesFornecedor(model.Telefones.ToList())
            };

            return _result;
        }

        public int idFornecedor { get; set; }

        [Display(Name = "Nome")]
        [Required(ErrorMessage = "Informe o nome")]
        public string nome { get; set; }

        [Display(Name = "Tipo")]
        [Required(ErrorMessage = "Informe o tipo")]
        public string tipo { get; set; }

        [Display(Name = "CPF")]
        public string cpf { get; set; }

        [Display(Name = "CNPJ")]
        public string cnpj { get; set; }

        [Display(Name = "E-mail")]
        public string email { get; set; }

        [Display(Name = "Site")]
        public string site { get; set; }

        [Display(Name = "Observação")]
        public string observacao { get; set; }

        [Display(Name = "Nº. residencial")]
        [Required(ErrorMessage = "Informe o tipo")]
        public string numero { get; set; }

        [Display(Name = "CEP")]
        public string cep { get; set; }

        [Display(Name = "Logradouro")]
        [Required(ErrorMessage = "Informe o tipo")]
        public string logradouro { get; set; }

        [Display(Name = "Bairro")]
        [Required(ErrorMessage = "Informe o tipo")]
        public string bairro { get; set; }

        [Display(Name = "Contato")]
        public string contato { get; set; }

        [Display(Name = "Telefone")]
        public string foneContato { get; set; }

        [Display(Name = "Celular")]
        public string celularContato { get; set; }

        public Models.Cidade.Consultar Cidade { get; set; }
        public List<TelefoneVM> Telefones { get; set; }
    }
}