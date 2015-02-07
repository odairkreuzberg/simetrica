using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace RP.Sistema.Report.Class
{
    public class Controle
    {
        public System.Web.Mvc.ActionResult GetReport(Model.Context db, string filter, int _idUsuario)
        {
            String titulo = "<center>Relação de Controles<center>";
            return Report.genericReport(
                new Report.genericReportData
                {
                    exportTO = Report.stringTOExportFormatType("PDF"),
                    fileRPT = "relControle.rpt",
                    listData = this.GetReportData(db, filter, _idUsuario),
                    parameters = new Dictionary<string, object> { { "titulo", titulo } }
                }
            );
        }

        public Dictionary<string, DataSet> GetReportData(Model.Context db, string filter, int _idUsuario)
        {
            var listData = new Dictionary<string, DataSet>
            { 
                {"table", GetDataSet(db, filter, _idUsuario)},
                {"subentidade.rpt", RP.Sistema.BLL.EntidadeBLL.getDtSetEntidade(db)}, 
            };

            return listData;
        }

        private DataSet GetDataSet(Model.Context db, string filter, int _idUsuario)
        {
            string sql = @"select * from tbcontrole";

            var item = db.Database.SqlQuery<Relatorio>(sql).ToList();

            var dataTable = Relatorio.GetDataTable(item);

            var dataset = new DataSet();
            dataset.Tables.Add(dataTable);

            return dataset;
        }

        private class Relatorio
        {
            private int idcontrole { get; set; }
            private string dscontrole { get; set; }
            private string nmcontrole { get; set; }

            internal static DataTable GetDataTable(List<Relatorio> items)
            {
                var _result = new DataTable("table");

                _result.Columns.Add("idcontrole", Type.GetType("System.Int32"));
                _result.Columns.Add("dscontrole", Type.GetType("System.String"));
                _result.Columns.Add("nmcontrole", Type.GetType("System.String"));

                foreach (var item in items)
                {
                    var row = _result.NewRow();
                    row["idcontrole"] = item.idcontrole;
                    row["dscontrole"] = item.dscontrole;
                    row["nmcontrole"] = item.nmcontrole;
                    _result.Rows.Add(row);
                }
                return _result;
            }
        }
    }
}
