using RP.Sistema.Model.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web.Mvc;

namespace RP.Sistema.Web.Models.ContaReceber
{
    public class ContaReceberVM
    {
        public Sistema.Model.Entities.ContaReceber GetContaReceber()
        {
            var _result = new Sistema.Model.Entities.ContaReceber
            {
                idOrigem = this.idOrigem,
                idCompra = this.idCompra,
                idContaReceber = this.idContaReceber,
                idCliente = this.Cliente.idCliente ?? 0,
                descricao = this.descricao,
                flFormaPagamento = this.flFormaPagamento,
                vencimento = (DateTime)this.vencimento,
                pagamento = this.pagamento,
                valorConta = this.valorConta ?? 0,
                valorPago = this.valorPago,
            };
            return _result;
        }

        public static ContaReceberVM GetContaReceber(Sistema.Model.Entities.ContaReceber model)
        {
            var _result = new ContaReceberVM
            {
                idOrigem = model.idOrigem,
                idContaReceber = model.idContaReceber,
                idCompra = model.idCompra,
                Cliente = RP.Sistema.Web.Models.Cliente.Consultar.GetModel(model.Cliente),
                Projeto = RP.Sistema.Web.Models.Projeto.Consultar.GetModel(model.Projeto),
                descricao = model.descricao,
                flFormaPagamento = model.flFormaPagamento,
                vencimento = model.vencimento,
                pagamento = model.pagamento,
                valorConta = model.valorConta,
                valorPago = model.valorPago,
            };

            return _result;
        }

        public int idContaReceber { get; set; }
        public int? idCompra { get; set; }
        public int? idOrigem { get; set; }

        [Display(Name = "Descrição")]
        [Required(ErrorMessage = "Informe o descricao")]
        public string descricao { get; set; }

        [Display(Name = "Data de vencimento")]
        [Required(ErrorMessage = "Informe o vencimento")]
        public DateTime? vencimento { get; set; }

        [Display(Name = "Data de pagamento")]
        public DateTime? pagamento { get; set; }

        [Display(Name = "Valor da conta R($)")]
        [Required(ErrorMessage = "Informe o valor")]
        public decimal? valorConta { get; set; }

        [Display(Name = "Valor do pagamento R($)")]
        public decimal? valorPago { get; set; }

        [Display(Name = "Diferença R($)")]
        public decimal? vlDiferenca { get; set; }

        [Display(Name = "Gerar diferença")]
        public string flDiferenca { get; set; }

        [Display(Name = "Forma de pagamento")]
        public string flFormaPagamento { get; set; }

        public Cliente.Consultar Cliente { get; set; }
        public Projeto.Consultar Projeto { get; set; }

        public static readonly SelectListItem[] FormaPagamento = new[]
        {
            new SelectListItem { Text = "Cheque", Value = "Cheque" }, 
            new SelectListItem { Text = "Dinheiro", Value = "Dinheiro" }, 
            new SelectListItem { Text = "Cartão", Value = "Cartão" }, 
            new SelectListItem { Text = "Boleto", Value = "Boleto" }
        };
    }
}