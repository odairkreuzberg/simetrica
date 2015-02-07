using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace RP.Sistema.Report.Class
{
    public class Funcionario
    {
        public System.Web.Mvc.ActionResult GetReport(Model.Context db, string filter, int _idUsuario)
        {
            String titulo = "<center>Relação de Funcionarios<center>";
            return Report.genericReport(
                new Report.genericReportData
                {
                    exportTO = Report.stringTOExportFormatType("PDF"),
                    fileRPT = "Funcionario.rpt",
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
            string sql = @"SELECT	idfuncionario, 
		                            nome,           tipo, 
		                            rg,             cpf, 
		                            email,          observacao, 
		                            numero,         cep, 
		                            logradouro,     bairro, 
		                            fone,           celular, 
		                            dtnascimento,   dtentrada, 
		                            salario,        comissao,
		                            dtsaida,        motivosaida, 
		                            status
                            FROM	funcionario
                            where nome like '%" + filter + "%'";

            var item = db.Database.SqlQuery<Relatorio>(sql).ToList();

            var dataTable = Relatorio.GetDataTable(item);

            var dataset = new DataSet();
            dataset.Tables.Add(dataTable);

            return dataset;
        }

        private class Relatorio
        {
            private int idfuncionario { get; set; }
            private string nome { get; set; }
            private string tipo { get; set; }
            private string rg { get; set; }
            private string cpf { get; set; }
            private string email { get; set; }
            private string observacao { get; set; }
            private string numero { get; set; }
            private string cep { get; set; }
            private string logradouro { get; set; }
            private string bairro { get; set; }
            private string fone { get; set; }
            private string celular { get; set; }
            private DateTime dtnascimento { get; set; }
            private DateTime dtentrada { get; set; }
            private decimal? salario { get; set; }
            private decimal? comissao { get; set; }
            private DateTime? dtsaida { get; set; }
            private string motivosaida { get; set; }
            private string status { get; set; }

            internal static DataTable GetDataTable(List<Relatorio> items)
            {
                var _result = new DataTable("table");

                _result.Columns.Add("idfuncionario", Type.GetType("System.Int32"));
                _result.Columns.Add("nome", Type.GetType("System.String"));
                _result.Columns.Add("tipo", Type.GetType("System.String"));
                _result.Columns.Add("rg", Type.GetType("System.String"));
                _result.Columns.Add("cpf", Type.GetType("System.String"));
                _result.Columns.Add("email", Type.GetType("System.String"));
                _result.Columns.Add("observacao", Type.GetType("System.String"));
                _result.Columns.Add("numero", Type.GetType("System.String"));
                _result.Columns.Add("cep", Type.GetType("System.String"));
                _result.Columns.Add("logradouro", Type.GetType("System.String"));
                _result.Columns.Add("bairro", Type.GetType("System.String"));
                _result.Columns.Add("fone", Type.GetType("System.String"));
                _result.Columns.Add("celular", Type.GetType("System.String"));
                _result.Columns.Add("dtnascimento", Type.GetType("System.DateTime"));
                _result.Columns.Add("dtentrada", Type.GetType("System.DateTime"));
                _result.Columns.Add("salario", Type.GetType("System.Decimal"));
                _result.Columns.Add("comissao", Type.GetType("System.Decimal"));
                _result.Columns.Add("dtsaida", Type.GetType("System.DateTime"));
                _result.Columns.Add("motivosaida", Type.GetType("System.String"));
                _result.Columns.Add("status", Type.GetType("System.String"));

                foreach (var item in items)
                {
                    var row = _result.NewRow();
                    row["idfuncionario"] = item.idfuncionario;
                    row["nome"] = item.nome;
                    row["tipo"] = item.tipo;
                    row["rg"] = item.rg;
                    row["cpf"] = item.cpf;
                    row["email"] = item.email;
                    row["observacao"] = item.observacao;
                    row["numero"] = item.numero;
                    row["cep"] = item.cep;
                    row["logradouro"] = item.logradouro;
                    row["bairro"] = item.bairro;
                    row["fone"] = item.fone;
                    row["celular"] = item.celular;
                    row["dtnascimento"] = item.dtnascimento;
                    row["dtentrada"] = item.dtentrada;
                    if (item.salario != null)
                        row["salario"] = item.salario;
                    if (item.comissao != null)
                        row["comissao"] = item.comissao;
                    if (item.dtsaida != null)
                        row["dtsaida"] = item.dtsaida;
                    row["motivosaida"] = item.motivosaida;
                    row["status"] = item.status;
                    _result.Rows.Add(row);
                }
                return _result;
            }
        }
    }
}
