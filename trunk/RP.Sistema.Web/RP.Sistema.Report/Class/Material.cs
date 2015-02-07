using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace RP.Sistema.Report.Class
{
    public class Material
    {
        public System.Web.Mvc.ActionResult GetReport(Model.Context db, string filter, string saldo, int _idUsuario)
        {
            String titulo = "<center>Relação de materiais<center>";
            return Report.genericReport(
                new Report.genericReportData
                {
                    exportTO = Report.stringTOExportFormatType("PDF"),
                    fileRPT = "Material.rpt",
                    listData = this.GetReportData(db, filter,saldo, _idUsuario),
                    parameters = new Dictionary<string, object> { { "titulo", titulo } }
                }
            );
        }

        public Dictionary<string, DataSet> GetReportData(Model.Context db, string filter, string saldo, int _idUsuario)
        {
            var listData = new Dictionary<string, DataSet>
            { 
                {"table", GetDataSet(db, filter,saldo, _idUsuario)},
                {"subentidade.rpt", RP.Sistema.BLL.EntidadeBLL.getDtSetEntidade(db)}, 
            };

            return listData;
        }

        private DataSet GetDataSet(Model.Context db, string filter, string saldo, int _idUsuario)
        {
            string sql = @"select material.idmaterial, material.nome, material.preco, material.margemganho, material.nrquantidade, 
	                            fabricante.nome as fabricante, unidademedida.nome as unidademedida 
                            from material
                            left join fabricante on material.idfabricante = fabricante.idfabricante
                            left join unidademedida on material.idunidademedida = unidademedida.idunidademedida 
                            where material.nome like '%" + filter + "%'";
            if (saldo != "todos")
            {
                sql += " and material.nrquantidade > 0";
            }

            var item = db.Database.SqlQuery<Relatorio>(sql).ToList();

            var dataTable = Relatorio.GetDataTable(item);

            var dataset = new DataSet();
            dataset.Tables.Add(dataTable);

            return dataset;
        }

        private class Relatorio
        {
            private int idmaterial { get; set; }
            private string nome { get; set; }
            private string unidademedida { get; set; }
            private string fabricante { get; set; }
            private decimal preco { get; set; }
            private decimal nrquantidade { get; set; }
            private decimal margemganho { get; set; }

            internal static DataTable GetDataTable(List<Relatorio> items)
            {
                var _result = new DataTable("table");

                _result.Columns.Add("idmaterial", Type.GetType("System.Int32"));
                _result.Columns.Add("nome", Type.GetType("System.String"));
                _result.Columns.Add("unidademedida", Type.GetType("System.String"));
                _result.Columns.Add("fabricante", Type.GetType("System.String"));
                _result.Columns.Add("preco", Type.GetType("System.Decimal"));
                _result.Columns.Add("margemganho", Type.GetType("System.Decimal"));
                _result.Columns.Add("nrquantidade", Type.GetType("System.Decimal"));

                foreach (var item in items)
                {
                    var row = _result.NewRow();
                    row["idmaterial"] = item.idmaterial;
                    row["nome"] = item.nome;
                    row["unidademedida"] = item.unidademedida;
                    row["fabricante"] = item.fabricante;
                    row["preco"] = item.preco;
                    row["nrquantidade"] = item.nrquantidade;
                    row["margemganho"] = item.margemganho;
                    _result.Rows.Add(row);
                }
                return _result;
            }
        }
    }
}
