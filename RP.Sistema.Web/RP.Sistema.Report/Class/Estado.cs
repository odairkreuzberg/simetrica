using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace RP.Sistema.Report.Class
{
    public class Estado
    {
        public System.Web.Mvc.ActionResult GetReport(Model.Context db, string filter, int _idUsuario)
        {
            String titulo = "<center>Relação de Estados<center>";
            return Report.genericReport(
                new Report.genericReportData
                {
                    exportTO = Report.stringTOExportFormatType("PDF"),
                    fileRPT = "Estado.rpt",
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
            string sql = @"select e.idpais, 
                            e.nome, 
                            e.sigla, 
                            p.nome as pais 
                            from estado as e 
                            inner join pais as p on e.idpais = p.idpais
                            where e.nome like '%" + filter + @"%'";

            var item = db.Database.SqlQuery<Relatorio>(sql).ToList();

            var dataTable = Relatorio.GetDataTable(item);

            var dataset = new DataSet();
            dataset.Tables.Add(dataTable);

            return dataset;
        }

        private class Relatorio
        {
            private int idestado { get; set; }
            private string nome { get; set; }
            private string sigla { get; set; }
            private string pais { get; set; }

            internal static DataTable GetDataTable(List<Relatorio> items)
            {
                var _result = new DataTable("table");

                _result.Columns.Add("idestado", Type.GetType("System.Int32"));
                _result.Columns.Add("nome", Type.GetType("System.String"));
                _result.Columns.Add("sigla", Type.GetType("System.String"));
                _result.Columns.Add("pais", Type.GetType("System.String"));

                foreach (var item in items)
                {
                    var row = _result.NewRow();
                    row["idestado"] = item.idestado;
                    row["nome"] = item.nome;
                    row["sigla"] = item.sigla;
                    row["pais"] = item.pais;
                    _result.Rows.Add(row);
                }
                return _result;
            }
        }
    }
}
