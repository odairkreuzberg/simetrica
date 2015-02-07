using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace RP.Sistema.Report.Class
{
    public class Cidade
    {
        public System.Web.Mvc.ActionResult GetReport(Model.Context db, string filter, int _idUsuario)
        {
            String titulo = "<center>Relação de Cidades<center>";
            return Report.genericReport(
                new Report.genericReportData
                {
                    exportTO = Report.stringTOExportFormatType("PDF"),
                    fileRPT = "Cidade.rpt",
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
            string sql = @"select c.idcidade, 
                            c.nome,
                            e.nome as estado,
                            p.nome as pais 
                            from cidade as c 
                            inner join estado as e on c.idestado = e.idestado
                            inner join pais as p on e.idpais = p.idpais
                            where c.nome like '%" + filter + @"%'";

            var item = db.Database.SqlQuery<Relatorio>(sql).ToList();

            var dataTable = Relatorio.GetDataTable(item);

            var dataset = new DataSet();
            dataset.Tables.Add(dataTable);

            return dataset;
        }

        private class Relatorio
        {
            private int idcidade { get; set; }
            private string nome { get; set; }
            private string estado { get; set; }
            private string pais { get; set; }

            internal static DataTable GetDataTable(List<Relatorio> items)
            {
                var _result = new DataTable("table");

                _result.Columns.Add("idcidade", Type.GetType("System.Int32"));
                _result.Columns.Add("nome", Type.GetType("System.String"));
                _result.Columns.Add("estado", Type.GetType("System.String"));
                _result.Columns.Add("pais", Type.GetType("System.String"));

                foreach (var item in items)
                {
                    var row = _result.NewRow();
                    row["idcidade"] = item.idcidade;
                    row["nome"] = item.nome;
                    row["estado"] = item.estado;
                    row["pais"] = item.pais;
                    _result.Rows.Add(row);
                }
                return _result;
            }
        }
    }
}
