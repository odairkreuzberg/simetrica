using RP.Sistema.BLL;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;

namespace RP.Sistema.Report.Class
{
    public class CaixaDetalhado
    {
        public System.Web.Mvc.ActionResult GetReport(Model.Context db, int _idUsuario)
        {
            String titulo = "<center>GRÁFICO DETALHADO REFERENTE AS CONTAS A PAGAR E RECEBER<center>";
            return Report.genericReport(
                new Report.genericReportData
                {
                    exportTO = Report.stringTOExportFormatType("PDF"),
                    fileRPT = "CaixaDetalhado.rpt",
                    listData = this.GetReportData(db, _idUsuario),
                    parameters = new Dictionary<string, object> { { "titulo", titulo } }
                }
            );
        }

        public Dictionary<string, DataSet> GetReportData(Model.Context db, int _idUsuario)
        {
            var listData = new Dictionary<string, DataSet>
            { 
                {"table", GetDataSet(db, _idUsuario)},
                {"subentidade.rpt", RP.Sistema.BLL.EntidadeBLL.getDtSetEntidade(db)}, 
            };

            return listData;
        }

        private DataSet GetDataSet(Model.Context db, int _idUsuario)
        {
            var dataTable = Relatorio.GetDataTable(db, _idUsuario);

            var dataset = new DataSet();
            dataset.Tables.Add(dataTable);

            return dataset;
        }

        private class Relatorio
        {
            private int idcaixa { get; set; }
            private string situacao { get; set; }
            private decimal saldoatual { get; set; }
            private decimal saldoanterior { get; set; }
            private decimal valor { get; set; }
            private string descricao { get; set; }
            private DateTime dtlancamento { get; set; }
            private string nmusuario { get; set; }

            internal static DataTable GetDataTable(Model.Context db, int _idUsuario)
            {
                var _result = new DataTable("table");

                _result.Columns.Add("idcaixa", Type.GetType("System.Int32"));
                _result.Columns.Add("situacao", Type.GetType("System.String"));
                _result.Columns.Add("saldoatual", Type.GetType("System.Decimal"));
                _result.Columns.Add("saldoanterior", Type.GetType("System.Decimal"));
                _result.Columns.Add("valor", Type.GetType("System.Decimal"));
                _result.Columns.Add("dtlancamento", Type.GetType("System.DateTime"));
                _result.Columns.Add("nmusuario", Type.GetType("System.String"));
                _result.Columns.Add("descricao", Type.GetType("System.String"));

                var contaPagarBLL = new ContaPagarBLL(db, _idUsuario);
                var contasPagar = contaPagarBLL.Find(u => u.situacao != "Cancelado")
                    .Select(u => new
                    {
                        u.situacao,
                        u.valorConta,
                        u.valorPago,
                        u.vencimento,
                    }).ToList();

                var contaReceberBLL = new ContaReceberBLL(db, _idUsuario);
                var contasReceber = contaReceberBLL.Find(u => u.situacao != "Cancelado")
                    .Select(u => new
                    {
                        u.situacao,
                        u.valorConta,
                        u.valorPago,
                        u.vencimento,
                    }).ToList();

                var inicio = new DateTime((DateTime.Now.Year-1), 1, 1);
                var fim = inicio.AddMonths(1);
                CultureInfo culture = new CultureInfo("pt-BR");
                DateTimeFormatInfo dtfi = culture.DateTimeFormat;

                for (int i = 1; i <= 48; i++)
                {
                    var pagamento = contasPagar.Any(u => u.situacao == "Aguardando pagamento" && u.vencimento >= inicio && u.vencimento < fim);
                    var recebimento = contasReceber.Any(u => u.situacao == "Aguardando pagamento" && u.vencimento >= inicio && u.vencimento < fim);
                    if (pagamento || recebimento)
                    {
                        var valorPagar = contasPagar.Where(u => u.situacao == "Aguardando pagamento" && u.vencimento >= inicio && u.vencimento < fim).Sum(u => u.valorConta);
                        var valorReceber= contasReceber.Where(u => u.situacao == "Aguardando pagamento" && u.vencimento >= inicio && u.vencimento < fim).Sum(u => u.valorConta);
                        var row = _result.NewRow();
                        row["idcaixa"] = i;
                        row["dtlancamento"] = inicio;
                        row["situacao"] = culture.TextInfo.ToTitleCase(dtfi.GetMonthName(inicio.Month));
                        row["descricao"] = "CONTA A PAGAR";
                        row["valor"] = valorPagar;
                        _result.Rows.Add(row);

                        row = _result.NewRow();
                        row["dtlancamento"] = inicio;
                        row["situacao"] = culture.TextInfo.ToTitleCase(dtfi.GetMonthName(inicio.Month));
                        row["descricao"] = "CONTA A RECEBER";
                        row["valor"] = valorReceber;
                        _result.Rows.Add(row);

                        row = _result.NewRow();
                        row["dtlancamento"] = inicio;
                        row["situacao"] = culture.TextInfo.ToTitleCase(dtfi.GetMonthName(inicio.Month));
                        row["descricao"] = "PREVISÃO";
                        row["valor"] = valorReceber - valorPagar;
                        _result.Rows.Add(row);
                    }

                    fim = fim.AddMonths(1);
                    inicio = inicio.AddMonths(1);

                }
                return _result;
            }
        }
    }
}
