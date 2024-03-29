﻿using RP.Sistema.BLL;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace RP.Sistema.Report.Class
{
    public class Caixa
    {
        public System.Web.Mvc.ActionResult GetReport(Model.Context db, int _idUsuario)
        {
            String titulo = "<center>GRÁFICO RESUMIDO REFERENTE A TODAS AS CONTAS A PAGAR E RECEBER<center>";
            return Report.genericReport(
                new Report.genericReportData
                {
                    exportTO = Report.stringTOExportFormatType("PDF"),
                    fileRPT = "Caixa.rpt",
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
                var hoje = DateTime.Now.Date;
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
                var contasReceber= contaReceberBLL.Find(u => u.situacao != "Cancelado")
                    .Select(u => new
                    {
                        u.situacao,
                        u.valorConta,
                        u.valorPago,
                        u.vencimento,
                    }).ToList();

                var row = _result.NewRow();
                row["valor"] = contasPagar.Where(u => u.situacao == "Aguardando pagamento" && u.vencimento < hoje).Sum(u => u.valorConta);
                row["descricao"] = "Contas a pagar [VENCIDA]";
                _result.Rows.Add(row);

                row = _result.NewRow();
                row["valor"] = contasPagar.Where(u => u.situacao == "Aguardando pagamento" && u.vencimento >= hoje).Sum(u => u.valorConta);
                row["descricao"] = "Contas a pagar [A VENCER]";
                _result.Rows.Add(row);

                row = _result.NewRow();
                row["valor"] = contasReceber.Where(u => u.situacao == "Aguardando pagamento" && u.vencimento < hoje).Sum(u => u.valorConta);
                row["descricao"] = "Contas a receber [VENCIDA]";
                _result.Rows.Add(row);

                row = _result.NewRow();
                row["valor"] = contasReceber.Where(u => u.situacao == "Aguardando pagamento" && u.vencimento >= hoje).Sum(u => u.valorConta);
                row["descricao"] = "Contas a receber [A VENCER]";
                _result.Rows.Add(row);

                //foreach (var item in items)
                //{
                //    var row = _result.NewRow();
                //    row["idcaixa"] = item.idcaixa;
                //    row["situacao"] = item.situacao;
                //    row["saldoatual"] = item.saldoatual;
                //    row["saldoanterior"] = item.saldoanterior;
                //    row["valor"] = item.valor;
                //    row["dtlancamento"] = item.dtlancamento;
                //    row["nmusuario"] = item.nmusuario;
                //    row["descricao"] = item.descricao;
                //    _result.Rows.Add(row);
                //}
                return _result;
            }
        }
    }
}
