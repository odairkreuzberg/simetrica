using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace RP.Sistema.Report.Class
{
    public class ContaReceber
    {
        public System.Web.Mvc.ActionResult GetReport(Model.Context db, string filter, DateTime? dtInicio, DateTime? dtFim, string situacao, int _idUsuario)
        {
            String titulo = "<center>Relação de contas a receber<center>";
            return Report.genericReport(
                new Report.genericReportData
                {
                    exportTO = Report.stringTOExportFormatType("PDF"),
                    fileRPT = "ContaReceber.rpt",
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
            string sql = @" SELECT	idcontareceber, parcela, contareceber.descricao, vencimento, pagamento, valorconta, valorpago,  situacao, flformapagamento, nome, tipo
                            FROM	contareceber 
                            left JOIN cliente ON contareceber.idcliente = cliente.idcliente
                            left JOIN projeto ON contareceber.idprojeto = projeto.idprojeto
                           where (contareceber.descricao like'%" + filter + "%' or projeto.descricao like'%" + filter + "%' or cliente.nome like'%" + filter + "%')";
            sql += @"and vencimento >= '" + dtInicio.Value.ToString("MM/dd/yyyy") + "' and vencimento <= '" + dtFim.Value.ToString("MM/dd/yyyy") + "'";
            if (situacao != "Todos")
                sql += @" and situacao = '" +situacao +"'";
            sql += " order by  vencimento";
            var item = db.Database.SqlQuery<Relatorio>(sql).ToList();

            var dataTable = Relatorio.GetDataTable(item);

            var dataset = new DataSet();
            dataset.Tables.Add(dataTable);

            return dataset;
        }

        private class Relatorio
        {
            private int idcontareceber { get; set; }
            private int parcela { get; set; }
            private string descricao { get; set; }
            private DateTime vencimento { get; set; }
            private DateTime? pagamento { get; set; }
            private decimal valorconta { get; set; }
            private decimal? valorpago { get; set; }
            private string situacao { get; set; }
            private string flformapagamento { get; set; }
            private string nome { get; set; }
            private string tipo { get; set; }

            internal static DataTable GetDataTable(List<Relatorio> items)
            {
                var _result = new DataTable("table");

                _result.Columns.Add("idcontareceber", Type.GetType("System.Int32"));
                _result.Columns.Add("parcela", Type.GetType("System.Int32"));
                _result.Columns.Add("descricao", Type.GetType("System.String"));
                _result.Columns.Add("vencimento", Type.GetType("System.DateTime"));
                _result.Columns.Add("pagamento", Type.GetType("System.DateTime"));
                _result.Columns.Add("valorconta", Type.GetType("System.Decimal"));
                _result.Columns.Add("valorpago", Type.GetType("System.Decimal"));
                _result.Columns.Add("situacao", Type.GetType("System.String"));
                _result.Columns.Add("flformapagamento", Type.GetType("System.String"));
                _result.Columns.Add("nome", Type.GetType("System.String"));
                _result.Columns.Add("tipo", Type.GetType("System.String"));

                foreach (var item in items)
                {
                    var row = _result.NewRow();
                    row["idcontareceber"] = item.idcontareceber;
                    row["parcela"] = item.parcela;
                    row["descricao"] = item.descricao;
                    row["vencimento"] = item.vencimento;
                    if (item.pagamento != null)
                    row["pagamento"] = item.pagamento;
                    row["valorconta"] = item.valorconta;
                    if (item.valorpago != null)
                    row["valorpago"] = item.valorpago;
                    row["situacao"] = item.situacao;
                    row["flformapagamento"] = item.flformapagamento;
                    row["nome"] = item.nome;
                    row["tipo"] = item.tipo;
                    _result.Rows.Add(row);
                }
                return _result;
            }
        }
    }
}
