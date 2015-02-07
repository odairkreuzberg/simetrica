using RP.Sistema.BLL;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;

namespace RP.Sistema.Report.Class
{
    public class FolhaFrequencia
    {
        public System.Web.Mvc.ActionResult GetReport(Model.Context db, int idFuncionario, int ano, int mes, int _idUsuario)
        {
            var competencia = new DateTime(ano, mes, 1);
            var culture = new CultureInfo("pt-BR");
            var dtfi = culture.DateTimeFormat;
            string dsMes = culture.TextInfo.ToTitleCase(dtfi.GetMonthName(mes));

            string titulo = "<center>FOLHA PONTO - " + dsMes.ToUpper() + "  -  " + ano + "<center>";
            return Report.genericReport(
                new Report.genericReportData
                {
                    exportTO = Report.stringTOExportFormatType("PDF"),
                    fileRPT = "FolhaFrequencia.rpt",
                    listData = this.GetReportData(db, idFuncionario, competencia, _idUsuario),
                    parameters = new Dictionary<string, object> { { "titulo", titulo } }
                }
            );
        }

        public Dictionary<string, DataSet> GetReportData(Model.Context db, int idFuncionario, DateTime competencia, int _idUsuario)
        {
            var listData = new Dictionary<string, DataSet>
            { 
                {"table", GetDataSet(db, idFuncionario, competencia, _idUsuario)},
                {"subentidade.rpt", RP.Sistema.BLL.EntidadeBLL.getDtSetEntidade(db)}, 
            };

            return listData;
        }

        private DataSet GetDataSet(Model.Context db, int idFuncionario, DateTime competencia, int _idUsuario)
        {
            var _bll = new FuncionarioBLL(db, _idUsuario);
            var funcionario = _bll.FindSingle(u => u.idFuncionario == idFuncionario, u => u.CartaoPontos);

            var dataTable = Relatorio.GetDataTable(funcionario, competencia);

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
            private decimal salario { get; set; }
            private decimal comissao { get; set; }
            private DateTime? dtsaida { get; set; }
            private string motivosaida { get; set; }
            private string status { get; set; }

            internal static DataTable GetDataTable(RP.Sistema.Model.Entities.Funcionario funcionario, DateTime competencia)
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
                _result.Columns.Add("ctps", Type.GetType("System.String"));

                _result.Columns.Add("entradaExtra", Type.GetType("System.String"));
                _result.Columns.Add("entradaManha", Type.GetType("System.String"));
                _result.Columns.Add("entradaTarde", Type.GetType("System.String"));
                _result.Columns.Add("saidaManha", Type.GetType("System.String"));
                _result.Columns.Add("saidaTarde", Type.GetType("System.String"));
                _result.Columns.Add("saidaExtra", Type.GetType("System.String"));
                _result.Columns.Add("dsDia", Type.GetType("System.String"));
                _result.Columns.Add("dsObservacao", Type.GetType("System.String"));
                _result.Columns.Add("flSituacao", Type.GetType("System.String"));
                var inicio = competencia;
                var fim = competencia.AddMonths(1);
                foreach (var item in funcionario.CartaoPontos.Where(u => u.dtPonto >= inicio && u.dtPonto < fim))
                {
                    string dsDia = competencia.Day + "º - " + new CultureInfo("pt-BR").DateTimeFormat.GetDayName(competencia.DayOfWeek);
                    competencia = competencia.AddDays(1);
                    var row = _result.NewRow();
                    row["idfuncionario"] = funcionario.idFuncionario;
                    row["nome"] = funcionario.nome;
                    row["tipo"] = funcionario.tipo;
                    row["rg"] = funcionario.rg;
                    row["cpf"] = funcionario.cpf;
                    row["email"] = funcionario.email;
                    row["observacao"] = funcionario.observacao;
                    row["numero"] = funcionario.numero;
                    row["cep"] = funcionario.cep;
                    row["logradouro"] = funcionario.logradouro;
                    row["bairro"] = funcionario.bairro;
                    row["fone"] = funcionario.fone;
                    row["celular"] = funcionario.celular;
                    row["dtnascimento"] = funcionario.dtNascimento;
                    row["dtentrada"] = funcionario.dtEntrada;
                    row["ctps"] = funcionario.ctps;

                    row["entradaExtra"] = item.entradaExtra;
                    row["entradaManha"] = item.entradaManha;
                    row["entradaTarde"] = item.entraTarde;
                    row["saidaManha"] = item.saidaManha;
                    row["saidaTarde"] = item.saidaTarde;
                    row["saidaExtra"] = item.saidaExtra;
                    row["dsObservacao"] = item.dsObservacao;
                    row["flSituacao"] = item.flSituacao;
                    row["dsDia"] = dsDia;
                    _result.Rows.Add(row);
                }
                return _result;
            }
        }
    }
}
