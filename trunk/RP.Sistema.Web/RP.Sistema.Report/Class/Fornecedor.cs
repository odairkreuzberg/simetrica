using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace RP.Sistema.Report.Class
{
    public class Fornecedor
    {
        public System.Web.Mvc.ActionResult GetReport(Model.Context db, string filter, int _idUsuario)
        {
            String titulo = "<center>Relação de Fornecedores<center>";
            return Report.genericReport(
                new Report.genericReportData
                {
                    exportTO = Report.stringTOExportFormatType("PDF"),
                    fileRPT = "Fornecedor.rpt",
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
            string sql = @"SELECT	idfornecedor, 
		                            nome, 
		                            tipo, 
		                            cnpjcpf, 
		                            email, 
		                            numero, 
		                            logradouro, 
		                            bairro, 
		                            contato
                            FROM	fornecedor
                            where nome like '%" + filter + "%'";

            var item = db.Database.SqlQuery<Relatorio>(sql).ToList();

            var dataTable = Relatorio.GetDataTable(item);

            var dataset = new DataSet();
            dataset.Tables.Add(dataTable);

            return dataset;
        }

        private class Relatorio
        {
            private int idfornecedor { get; set; }
            private string nome { get; set; }
            private string tipo { get; set; }
            private string cnpjcpf { get; set; }
            private string email { get; set; }
            private string numero { get; set; }
            private string logradouro { get; set; }
            private string bairro { get; set; }
            private string contato { get; set; }

            internal static DataTable GetDataTable(List<Relatorio> items)
            {
                var _result = new DataTable("table");

                _result.Columns.Add("idfornecedor", Type.GetType("System.Int32"));
                _result.Columns.Add("nome", Type.GetType("System.String"));
                _result.Columns.Add("tipo", Type.GetType("System.String"));
                _result.Columns.Add("cnpjcpf", Type.GetType("System.String"));
                _result.Columns.Add("numero", Type.GetType("System.String"));
                _result.Columns.Add("logradouro", Type.GetType("System.String"));
                _result.Columns.Add("bairro", Type.GetType("System.String"));
                _result.Columns.Add("contato", Type.GetType("System.String"));
                _result.Columns.Add("email", Type.GetType("System.String"));

                foreach (var item in items)
                {
                    var row = _result.NewRow();
                    row["idfornecedor"] = item.idfornecedor;
                    row["nome"] = item.nome;
                    row["tipo"] = item.tipo;
                    row["cnpjcpf"] = item.cnpjcpf;
                    row["email"] = item.email;
                    row["numero"] = item.numero;
                    row["logradouro"] = item.logradouro;
                    row["bairro"] = item.bairro;
                    row["contato"] = item.contato;
                    _result.Rows.Add(row);
                }
                return _result;
            }
        }
    }
}
