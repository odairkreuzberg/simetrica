using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace RP.Sistema.Report.Class
{
    public class Caixa
    {
        public System.Web.Mvc.ActionResult GetReport(Model.Context db, string filter, DateTime? dtInicio, DateTime? dtFim, string situacao, int _idUsuario)
        {
            String titulo = "<center>Relação de entradas e saídas<center>";
            return Report.genericReport(
                new Report.genericReportData
                {
                    exportTO = Report.stringTOExportFormatType("PDF"),
                    fileRPT = "Caixa.rpt",
                    listData = this.GetReportData(db, filter, dtInicio, dtFim, situacao, _idUsuario),
                    parameters = new Dictionary<string, object> { { "titulo", titulo } }
                }
            );
        }

        public Dictionary<string, DataSet> GetReportData(Model.Context db, string filter, DateTime? dtInicio, DateTime? dtFim, string situacao, int _idUsuario)
        {
            var listData = new Dictionary<string, DataSet>
            { 
                {"table", GetDataSet(db, filter, dtInicio, dtFim, situacao, _idUsuario)},
                {"subentidade.rpt", RP.Sistema.BLL.EntidadeBLL.getDtSetEntidade(db)}, 
            };

            return listData;
        }

        private DataSet GetDataSet(Model.Context db, string filter, DateTime? dtInicio, DateTime? dtFim, string situacao, int _idUsuario)
        {
            dtFim = dtFim ?? DateTime.Now;
            dtInicio = dtInicio ?? dtFim.Value.AddDays(-11);
            dtFim = dtFim.Value.AddDays(1);
            string sql = @"SELECT     idcaixa, situacao, saldoatual, saldoanterior, valor, descricao, dtlancamento, nmusuario 
                           FROM caixa INNER join tbusuario ON caixa.idusuario = tbusuario.idusuario 
                           where descricao like'%" + filter + "%' ";
            sql += @"and dtlancamento >= '" + dtInicio.Value.ToString("MM/dd/yyyy") + "' and dtlancamento <= '" + dtFim.Value.ToString("MM/dd/yyyy") + "'";
            if (situacao != "Todos")
                sql += @" and situacao = '" +situacao +"'";
            sql += " order by  dtlancamento";
            var item = db.Database.SqlQuery<Relatorio>(sql).ToList();

            var dataTable = Relatorio.GetDataTable(item);

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

            internal static DataTable GetDataTable(List<Relatorio> items)
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

                foreach (var item in items)
                {
                    var row = _result.NewRow();
                    row["idcaixa"] = item.idcaixa;
                    row["situacao"] = item.situacao;
                    row["saldoatual"] = item.saldoatual;
                    row["saldoanterior"] = item.saldoanterior;
                    row["valor"] = item.valor;
                    row["dtlancamento"] = item.dtlancamento;
                    row["nmusuario"] = item.nmusuario;
                    row["descricao"] = item.descricao;
                    _result.Rows.Add(row);
                }
                return _result;
            }
        }
    }
}
