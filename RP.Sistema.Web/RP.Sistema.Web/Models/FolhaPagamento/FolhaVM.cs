using RP.Sistema.Model.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;
using System.Web;

namespace RP.Sistema.Web.Models.FolhaPagamento
{
    public class FolhaVM
    {
        public class Ponto
        {
            public string entradaManha { get; set; }
            public string saidaManha { get; set; }
            public string entraTarde { get; set; }
            public string saidaTarde { get; set; }
            public string entradaExtra { get; set; }
            public string saidaExtra { get; set; }
            public int idFuncionario { get; set; }
            public int nrDia { get; set; }
            public string dsDia { get; set; }
            public int nrMes { get; set; }
            public string flSituacao { get; set; }
            public string dsObservacao { get; set; }

            internal static List<Ponto> GetPontos(int ano, int mes, int idFuncionario, List<Feriado> list)
            {
                var competenia = new DateTime(ano, mes, 1);
                int dias = competenia.AddMonths(1).AddDays(-1).Day;
                var _result = new List<Ponto>();
                for (int i = 1; i <= dias; i++)
                {
                    string diaExtenco = new CultureInfo("pt-BR").DateTimeFormat.GetDayName(competenia.DayOfWeek);
                    _result.Add(new Ponto
                    {
                        entradaManha = (diaExtenco.Contains("domingo") || diaExtenco.Contains("sábado") || list.Any(u => u.nrDia == i)) ? "" : "08:00",
                        saidaManha = (diaExtenco.Contains("domingo") || diaExtenco.Contains("sábado") || list.Any(u => u.nrDia == i)) ? "" : "12:00",
                        entraTarde = (diaExtenco.Contains("domingo") || diaExtenco.Contains("sábado") || list.Any(u => u.nrDia == i)) ? "" : "14:00",
                        saidaTarde = (diaExtenco.Contains("domingo") || diaExtenco.Contains("sábado") || list.Any(u => u.nrDia == i)) ? "" : "18:00",
                        nrDia = competenia.Day,
                        dsDia = diaExtenco,
                        flSituacao = (diaExtenco.Contains("domingo") || diaExtenco.Contains("sábado")) ? diaExtenco : list.Any(u => u.nrDia == i) ? "Feriado" : "Dia útil", //(diaExtenco.Contains("domingo") || diaExtenco.Contains("sábado") || list.Any(u => u.nrDia == i)) ? "" : CartaoPonto.TRABALHOU,
                        nrMes = mes,
                        idFuncionario = idFuncionario,
                        dsObservacao = (diaExtenco.Contains("domingo") || diaExtenco.Contains("sábado")) ? diaExtenco : (list.Any(u => u.nrDia == i) ? list.First(u => u.nrDia == i).nmFeriado : string.Empty),
                    });
                    competenia = competenia.AddDays(1);
                }
                return _result;
            }
        }
        public class Comissao
        {
            public int idMovimento { get; set; }
            public DateTime dtVencimento { get; set; }
            public string situacao { get; set; }
            public string descricao { get; set; }
            public decimal valor { get; set; }

            internal static List<Comissao> GetComicoes(List<MovimentoProfissional> list)
            {
                var _result = new List<Comissao>();
                foreach (var item in list)
                {
                    _result.Add(new Comissao{ idMovimento = item.idMovimento, descricao = item.descricao, dtVencimento = item.dtVencimento, situacao = item.situacao, valor = item.valor});
                }
                return _result;
            }
        }

        public Funcionario.Consultar Funcionario { get; set; }
        public List<Ponto> Pontos { get; set; }
        public List<Comissao> Comissoes { get; set; }
        public List<Comissao> Proximos { get; set; }
        [Display(Name = "Competencia")]
        public int nrAno { get; set; }
        public int nrMes { get; set; }
        [Display(Name = "Mes")]
        public string dsMes { get; set; }
    }
}