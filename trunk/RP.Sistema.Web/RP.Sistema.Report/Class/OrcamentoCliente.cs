using RP.Sistema.Model.Entities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace RP.Sistema.Report.Class
{
    public class OrcamentoCliente
    {
        public System.Web.Mvc.ActionResult GetReport(Model.Context db, int idProjeto, int _idUsuario)
        {
            String titulo = "<center>Orçamento para Confecção dos Móveis na Linha MDF Masisa<center>";
            return Report.genericReport(
                new Report.genericReportData
                {
                    exportTO = Report.stringTOExportFormatType("PDF"),
                    fileRPT = "OrcamentoCliente.rpt",
                    listData = this.GetReportData(db, idProjeto, _idUsuario),
                    parameters = new Dictionary<string, object> { { "titulo", titulo } }
                }
            );
        }

        public Dictionary<string, DataSet> GetReportData(Model.Context db, int idProjeto, int _idUsuario)
        {
            var listData = new Dictionary<string, DataSet>
            { 
                {"table", GetDataSet(db, idProjeto, _idUsuario)},
                {"subentidade.rpt", RP.Sistema.BLL.EntidadeBLL.getDtSetEntidade(db)}, 
            };

            return listData;
        }

        private DataSet GetDataSet(Model.Context db, int idProjeto, int _idUsuario)
        {
            var _bll = new RP.Sistema.BLL.ProjetoBLL(db,_idUsuario);
            var _projeto = _bll.FindSingle(u => u.idProjeto == idProjeto, u => u.Cliente, u => u.Produtos);

            var dataTable = Relatorio.GetDataTable(_projeto);

            var dataset = new DataSet();
            dataset.Tables.Add(dataTable);

            return dataset;
        }

        private class Relatorio
        {

            internal static DataTable GetDataTable(Projeto _projeto)
            {
                var _result = new DataTable("table");


                _result.Columns.Add("idProjeto", Type.GetType("System.Int32"));
                _result.Columns.Add("dtinicio", Type.GetType("System.DateTime"));
                _result.Columns.Add("descricaoprojeto", Type.GetType("System.String"));
                _result.Columns.Add("status", Type.GetType("System.String"));
                _result.Columns.Add("email", Type.GetType("System.String"));
                _result.Columns.Add("nomecliente", Type.GetType("System.String"));
                _result.Columns.Add("tipo", Type.GetType("System.String"));
                _result.Columns.Add("contato", Type.GetType("System.String"));
                _result.Columns.Add("fonecontato", Type.GetType("System.String"));
                _result.Columns.Add("celularcontato", Type.GetType("System.String"));
                _result.Columns.Add("previsaotermino", Type.GetType("System.String"));


                _result.Columns.Add("idProduto", Type.GetType("System.Int32"));
                _result.Columns.Add("nomeProduto", Type.GetType("System.String"));
                _result.Columns.Add("descricaoProduto", Type.GetType("System.String"));
                _result.Columns.Add("vlDesconto", Type.GetType("System.Decimal"));
                _result.Columns.Add("vlProduto", Type.GetType("System.Decimal"));
                _result.Columns.Add("vlVenda", Type.GetType("System.Decimal"));
                
                    var dias = _projeto.dtFim.Value.Date.Subtract(_projeto.dtInicio.Value.Date);
                foreach (var produto in _projeto.Produtos.ToList())
                {
                    var row = _result.NewRow();
                    row["idProjeto"] = produto.idProjeto;
                    row["dtinicio"] = _projeto.dtInicio;
                    row["descricaoprojeto"] = _projeto.descricao;
                    row["status"] = _projeto.status;
                    row["email"] = _projeto.Cliente.email;
                    row["nomecliente"] = _projeto.Cliente.nome;
                    row["tipo"] = _projeto.Cliente.tipo;
                    row["contato"] = _projeto.Cliente.contato;
                    row["fonecontato"] = _projeto.Cliente.foneContato;
                    row["celularcontato"] = _projeto.Cliente.celularContato;
                    row["previsaotermino"] = dias.TotalDays;

                    row["idProduto"] = produto.idProduto;
                    row["nomeProduto"] = produto.nome;
                    row["descricaoProduto"] = produto.descricao;
                    row["vlDesconto"] = produto.vlDesconto;
                    row["vlVenda"] = produto.vlVenda;
                    row["vlProduto"] = produto.vlProduto;

                    _result.Rows.Add(row);
                }
                return _result;
            }
        }
    }
}
