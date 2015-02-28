using RP.Sistema.BLL;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;

namespace RP.Sistema.Report.Class
{
    public class FolhaPagamento
    {

        public System.Web.Mvc.ActionResult GetReport(Model.Context db, int ano, int mes, int _idUsuario)
        {
            var culture = new CultureInfo("pt-BR");
            var dtfi = culture.DateTimeFormat;
            string dsMes = culture.TextInfo.ToTitleCase(dtfi.GetMonthName(mes));
            string titulo = "<center>FOLHA DE PAGAMENTO - " + dsMes.ToUpper() + "  -  " + ano + "<center>";
            return Report.genericReport(
                new Report.genericReportData
                {
                    exportTO = Report.stringTOExportFormatType("PDF"),
                    fileRPT = "FolhaPagamento.rpt",
                    listData = this.GetReportData(db, ano, mes, _idUsuario),
                    parameters = new Dictionary<string, object> { { "titulo", titulo } }
                }
            );

        }

        public Dictionary<string, DataSet> GetReportData(Model.Context db, int ano, int mes, int _idUsuario)
        {
            var listData = new Dictionary<string, DataSet>
            { 
                {"table", GetDataSet(db, ano, mes, _idUsuario)},
                {"subentidade.rpt", RP.Sistema.BLL.EntidadeBLL.getDtSetEntidade(db)}
            };

            return listData;
        }

        private DataSet GetDataSet(Model.Context db, int ano, int mes, int _idUsuario)
        {
            var _bll = new FolhaPagamentoBLL(db, _idUsuario);

            var _folhas = _bll.Find(u => u.nrAno == ano && u.nrMes == mes && u.situacao == RP.Sistema.Model.Entities.FolhaPagamento.PAGO, u => u.Funcionario).ToList();

            var dataTable = GetDataTable(_folhas);

            var dataset = new DataSet();
            dataset.Tables.Add(dataTable);

            return dataset;
        }

        private DataTable GetDataTable(List<RP.Sistema.Model.Entities.FolhaPagamento> folhas)
        {
            var _result = new DataTable("table");

            _result.Columns.Add("nome", Type.GetType("System.String"));
            _result.Columns.Add("total", Type.GetType("System.Decimal"));
            _result.Columns.Add("comissao", Type.GetType("System.Decimal"));
            _result.Columns.Add("salario", Type.GetType("System.Decimal"));
            _result.Columns.Add("bonificacao", Type.GetType("System.Decimal"));
            _result.Columns.Add("outrosDescontos", Type.GetType("System.Decimal"));
            _result.Columns.Add("horaExtra", Type.GetType("System.Decimal"));
            _result.Columns.Add("inss", Type.GetType("System.Decimal"));
            _result.Columns.Add("fgts", Type.GetType("System.Decimal"));
            _result.Columns.Add("vale", Type.GetType("System.Decimal"));
            foreach (var item in folhas)
            {
                var row = _result.NewRow();
                row["nome"] = item.Funcionario.nome;
                row["total"] = item.total;
                row["salario"] = item.salario == 0 ? item.comissao : item.salario;
                row["outrosDescontos"] = item.outrosDescontos;
                row["bonificacao"] = item.bonificacao;
                row["horaExtra"] = item.horaExtra ?? 0;
                row["inss"] = item.inss;
                row["fgts"] = item.FGTS;
                row["vale"] = item.vale;
                _result.Rows.Add(row);

            }
            return _result;
        }
    }
}
