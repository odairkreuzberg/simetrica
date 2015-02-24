using RP.Sistema.BLL;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace RP.Sistema.Report.Class
{
    public class ContaPagar
    {
        public System.Web.Mvc.ActionResult GetReport(Model.Context db, List<int?> list, DateTime? dtInicio, DateTime? dtFim, int _idUsuario, string tipo)
        {
            string de = dtInicio != null ? " apartir de " + dtInicio.Value.ToString("dd/MM/yyyy") : "";
            string ate = dtFim != null ? " até " + dtFim.Value.ToString("dd/MM/yyyy") : "";
            String titulo = "<center>Relação de contas a pagar "+de + ate+"<center>";
            string report = tipo == "Resumido" ? "ContaPagarResumido.rpt" : "ContaPagar.rpt";
            return Report.genericReport(
                new Report.genericReportData
                {
                    exportTO = Report.stringTOExportFormatType("PDF"),
                    fileRPT = report,
                    listData = this.GetReportData(db, list, dtInicio, dtFim, _idUsuario, tipo),
                    parameters = new Dictionary<string, object> { { "titulo", titulo } }
                }
            );
        }

        public Dictionary<string, DataSet> GetReportData(Model.Context db, List<int?> list, DateTime? dtInicio, DateTime? dtFim, int _idUsuario, string tipo)
        {
            var listData = new Dictionary<string, DataSet>
            { 
                {"table", GetDataSet(db, list,  dtInicio, dtFim, _idUsuario, tipo)},
                {"subentidade.rpt", RP.Sistema.BLL.EntidadeBLL.getDtSetEntidade(db)}, 
            };

            return listData;
        }

        private DataSet GetDataSet(Model.Context db, List<int?> list, DateTime? dtInicio, DateTime? dtFim, int _idUsuario, string tipo)
        {
            var bll = new ContaPagarBLL(db, _idUsuario);

            var query = bll.Find(u => u.situacao == "Aguardando pagamento");

            if (dtInicio != null)
            {
                query = query.Where(u => u.vencimento >= dtInicio);
            }

            if (dtFim != null)
            {
                dtFim = dtFim.Value.AddDays(1);
                query = query.Where(u => u.vencimento < dtFim);
            }
            query = query.Where(u => list.Any(k => k == u.idFornecedor));
              var itens = query.Select(u => new Relatorio
                {
                    idcontapagar = u.idContaPagar,
                    parcela = u.parcela,
                    descricao = u.descricao,
                    vencimento = u.vencimento,
                    pagamento = u.pagamento,
                    valorconta = u.valorConta,
                    valorpago = u.valorPago,
                    situacao = u.situacao,
                    flformapagamento = u.flFormaPagamento,
                    nome = u.Fornecedor.nome,
                    tipo = u.Fornecedor.tipo
                }).ToList();
            var dataTable = Relatorio.GetDataTable(itens, tipo);

            var dataset = new DataSet();
            dataset.Tables.Add(dataTable);

            return dataset;
        }

        private class Relatorio
        {
            public int idcontapagar { get; set; }
            public int parcela { get; set; }
            public string descricao { get; set; }
            public DateTime vencimento { get; set; }
            public DateTime? pagamento { get; set; }
            public decimal valorconta { get; set; }
            public decimal? valorpago { get; set; }
            public string situacao { get; set; }
            public string flformapagamento { get; set; }
            public string nome { get; set; }
            public string tipo { get; set; }

            internal static DataTable GetDataTable(List<Relatorio> items, string tipo)
            {
                var _result = new DataTable("table");

                _result.Columns.Add("idcontapagar", Type.GetType("System.Int32"));
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

                if (tipo == "Detalhado")
                {
                    foreach (var item in items)
                    {
                        var row = _result.NewRow();
                        row["idcontapagar"] = item.idcontapagar;
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
                }
                else
                {
                    foreach (var grupo in items.GroupBy(u => u.nome))
                    {
                        var item = grupo.FirstOrDefault();
                        var row = _result.NewRow();
                        row["idcontapagar"] = item.idcontapagar;
                        row["parcela"] = item.parcela;
                        row["descricao"] = item.descricao;
                        row["vencimento"] = item.vencimento;
                        row["valorconta"] = grupo.Sum(u => u.valorconta);
                        row["situacao"] = item.situacao;
                        row["flformapagamento"] = item.flformapagamento;
                        row["nome"] = item.nome;
                        row["tipo"] = item.tipo;
                        _result.Rows.Add(row);
                    }
                }
                return _result;
            }
        }
    }
}
