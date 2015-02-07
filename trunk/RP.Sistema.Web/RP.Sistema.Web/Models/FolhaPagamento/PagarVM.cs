using RP.Sistema.Model.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;
using System.Web;

namespace RP.Sistema.Web.Models.FolhaPagamento
{
    public class PagarVM
    {
        public class Movimento
        {
            public int idMovimento { get; set; }
            public DateTime dtVencimento { get; set; }
            public string tipo { get; set; }
            public string descricao { get; set; }
            public decimal? comissao { get; set; }
            public decimal? vale { get; set; }

            internal static List<Movimento> GetMovimentos(List<MovimentoProfissional> list)
            {
                var _result = new List<Movimento>();
                foreach (var item in list)
                {
                    _result.Add(new Movimento
                    {
                        idMovimento = item.idMovimento,
                        descricao = item.descricao,
                        dtVencimento = item.dtVencimento,
                        tipo = item.tipo,
                        comissao = item.tipo == MovimentoProfissional.TIPO_COMISSAO ? (decimal?)item.valor : null,
                        vale = item.tipo == MovimentoProfissional.TIPO_VALE ? (decimal?)item.valor : null
                    });
                }
                return _result;
            }
        }

        public Funcionario.Consultar Funcionario { get; set; }
        public List<Movimento> Movimentos { get; set; }
        [Display(Name = "Competencia")]
        public int nrAno { get; set; }
        public int nrMes { get; set; }
        public int idFolha { get; set; }
        [Display(Name = "Mes")]
        public string dsMes { get; set; }
        [Display(Name = "Mensalista")]
        public string mensalista { get; set; }
        public decimal? salario { get; set; }
        public decimal? bonificacao { get; set; }
        public decimal? horaExtra { get; set; }
        public decimal? outrosDescontos { get; set; }
        public decimal? inss { get; set; }
        public decimal? totalReceber { get; set; }
        public decimal? totalVencimento { get; set; }
        public decimal? totalDesconto { get; set; }
    }
}