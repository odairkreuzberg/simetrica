using RP.Sistema.Model.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web.Mvc;

namespace RP.Sistema.Web.Models.Caixa
{
    public class CaixaVM
    {

        public static CaixaVM GetCaixa(Sistema.Model.Entities.Caixa model)
        {
            var _result = new CaixaVM
            {
                idCaixa = model.idCaixa,
                valor = model.valor,
                situacao = model.situacao,
                saldoAnterior = model.saldoAnterior,
                saldoAtual = model.saldoAtual,
                descricao = model.descricao,
                dtLancamento = model.dtLancamento
            };
            if (model.ContaReceber != null)
            {
                _result.Cliente = Models.Cliente.Consultar.GetModel(model.ContaReceber.Cliente);
                _result.Projeto = Models.Projeto.Consultar.GetModel(model.ContaReceber.Projeto);
            }
            else if (model.ContaPagar != null && model.ContaPagar.Fornecedor != null)
            {
                _result.Fornecedor = Models.Fornecedor.Consultar.GetModel(model.ContaPagar.Fornecedor);
                _result.Projeto = Models.Projeto.Consultar.GetModel(model.ContaPagar.Projeto);
            }
            else if (model.MovimentoProfissional != null)
            {
                _result.Funcionario = Models.Funcionario.Consultar.GetModel(model.MovimentoProfissional.Funcionario);
            }
            return _result;
        }

        public int idCaixa { get; set; }

        [Display(Name = "Situação")]
        public string situacao { get; set; }

        [Display(Name = "Valor lançamento R$")]
        public decimal valor { get; set; }

        [Display(Name = "Saldo anterior R$")]
        public decimal saldoAnterior { get; set; }

        [Display(Name = "Saldo atual R$")]
        public decimal saldoAtual { get; set; }

        [Display(Name = "Descrição")]
        public string descricao { get; set; }

        [Display(Name = "Dt. lançamento")]
        public DateTime dtLancamento { get; set; }

        public Cliente.Consultar Cliente { get; set; }
        public Fornecedor.Consultar Fornecedor { get; set; }
        public Funcionario.Consultar Funcionario { get; set; }
        public Projeto.Consultar Projeto { get; set; }
    }
}