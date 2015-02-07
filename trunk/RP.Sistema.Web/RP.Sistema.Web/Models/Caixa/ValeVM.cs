using RP.Sistema.Model.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web.Mvc;

namespace RP.Sistema.Web.Models.Caixa
{
    public class ValeVM
    {

        public int idMovimento { get; set; }

        [Display(Name = "Descrição")]
        [Required(ErrorMessage = "Informe a descrição")]
        public string descricao { get; set; }

        [Display(Name = "Valor lançamento R$")]
        [Required(ErrorMessage = "Informe o valor")]
        public decimal valor { get; set; }

        public Funcionario.Consultar Funcionario { get; set; }

        internal MovimentoProfissional GetMovimento()
        {
            var _result = new MovimentoProfissional
            {
                valor = this.valor,
                descricao = this.descricao,
                idFuncionario = this.Funcionario.idFuncionario ?? 0,
                tipo = MovimentoProfissional.TIPO_VALE,
                situacao = MovimentoProfissional.SITUACAO_PENDENTE,
                dtVencimento = DateTime.Now,
                dtLancamento = DateTime.Now,
            };
            return _result;
        }
    }
}