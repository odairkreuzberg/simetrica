using RP.Sistema.BLL;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace RP.Sistema.Report.Class
{
    public class Requisicao
    {
        public System.Web.Mvc.ActionResult GetReport(Model.Context db, int idRequisicao, int _idUsuario)
        {
            String titulo = "<center>Requisição Nº. " + idRequisicao.ToString().PadLeft(8, '0') + "<center>";
            return Report.genericReport(
                new Report.genericReportData
                {
                    exportTO = Report.stringTOExportFormatType("PDF"),
                    fileRPT = "Requisicao.rpt",
                    listData = this.GetReportData(db, idRequisicao, _idUsuario),
                    parameters = new Dictionary<string, object> { { "titulo", titulo } }
                }
            );
        }

        public Dictionary<string, DataSet> GetReportData(Model.Context db, int idRequisicao, int _idUsuario)
        {
            var listData = new Dictionary<string, DataSet>
            { 
                {"table", GetDataSet(db, idRequisicao, _idUsuario)},
                {"subentidade.rpt", RP.Sistema.BLL.EntidadeBLL.getDtSetEntidade(db)}, 
            };

            return listData;
        }

        private DataSet GetDataSet(Model.Context db, int idRequisicao, int _idUsuario)
        {
            var _bll = new RequisicaoBLL(db, _idUsuario);

            var requisicao = _bll.FindSingle(u => u.idRequisicao == idRequisicao,
                                             u => u.Projeto, u => u.Funcionario,
                                             u => u.RequisicaoItens.Select(k => k.Material),
                                             u => u.RequisicaoItens.Select(k => k.Material.Fabricante),
                                             u => u.RequisicaoItens.Select(k => k.Material.UnidadeMedida));


            var dataTable = Relatorio.GetDataTable(requisicao);

            var dataset = new DataSet();
            dataset.Tables.Add(dataTable);

            return dataset;
        }

        private class Relatorio
        {

            internal static DataTable GetDataTable(RP.Sistema.Model.Entities.Requisicao requisicao)
            {
                var _result = new DataTable("table");

                _result.Columns.Add("idquisicao", Type.GetType("System.Int32"));
                _result.Columns.Add("projeto", Type.GetType("System.String"));
                _result.Columns.Add("status", Type.GetType("System.String"));
                _result.Columns.Add("funcionario", Type.GetType("System.String"));
                _result.Columns.Add("tipo", Type.GetType("System.String"));
                _result.Columns.Add("descricao", Type.GetType("System.String"));
                _result.Columns.Add("material", Type.GetType("System.String"));
                _result.Columns.Add("quantidade", Type.GetType("System.Decimal"));
                _result.Columns.Add("unidademedida", Type.GetType("System.String"));
                _result.Columns.Add("fabricante", Type.GetType("System.String"));

                foreach (var item in requisicao.RequisicaoItens.ToList())
                {
                    var row = _result.NewRow();
                    row["idquisicao"] = requisicao.idRequisicao;
                    row["projeto"] = requisicao.Projeto.descricao;
                    row["status"] = requisicao.Projeto.status;
                    row["funcionario"] = requisicao.Funcionario.nome;
                    row["tipo"] = requisicao.Funcionario.tipo;
                    row["descricao"] = requisicao.dsObservacao;
                    row["material"] = item.Material.nome;
                    row["quantidade"] = item.nrQuantidade;
                    row["unidademedida"] = item.Material.UnidadeMedida.nome;
                    //row["fabricante"] = item.Material.Fabricante.nome;
                    _result.Rows.Add(row);
                }
                return _result;
            }
        }
    }
}
