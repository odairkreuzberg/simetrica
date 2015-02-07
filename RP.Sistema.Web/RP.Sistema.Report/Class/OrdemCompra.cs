using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace RP.Sistema.Report.Class
{
    public class OrdemCompra
    {
        public System.Web.Mvc.ActionResult GetReport(Model.Context db, int idProjeto, int _idUsuario)
        {
            String titulo = "<center>Ordem de compra<center>";
            return Report.genericReport(
                new Report.genericReportData
                {
                    exportTO = Report.stringTOExportFormatType("PDF"),
                    fileRPT = "OrdemCompra.rpt",
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
            string sql = @"SELECT   projeto.idprojeto, 
		                            projeto.dtinicio, 
		                            projeto.descricao, 
		                            projeto.status, 
		                            cliente.nome, 
		                            cliente.tipo, 
		                            cliente.email, 
		                            cliente.contato, 
		                            cliente.fonecontato, 
		                            cliente.celularcontato,
		                            cliente.cnpjcpf, 
		                            produtomaterial.idprodutomaterial, 
		                            produtomaterial.quantidade, 
		                            produtomaterial.valor as vlprodutomaterial, 
		                            material.nome AS nmmaterial, 
		                            unidademedida.nome AS nmunidademedida, 
		                            fabricante.nome AS nmfabricante, 
		                            material.preco as vlmaterial,
		                            material.idmaterial
                        FROM	    material 
                        INNER JOIN produtomaterial ON material.idmaterial = produtomaterial.idmaterial 
                        INNER JOIN produto ON produtomaterial.idproduto = produto.idproduto 
                        INNER JOIN projeto ON produto.idprojeto = projeto.idprojeto 
                        INNER JOIN cliente ON projeto.idcliente = cliente.idcliente 
                        left JOIN fabricante ON material.idfabricante = fabricante.idfabricante 
                        left JOIN unidademedida ON material.idunidademedida = unidademedida.idunidademedida
                        where projeto.idprojeto = " + idProjeto;

            var item = db.Database.SqlQuery<Relatorio>(sql).ToList();

            var dataTable = Relatorio.GetDataTable(item);

            var dataset = new DataSet();
            dataset.Tables.Add(dataTable);

            return dataset;
        }

        private class Relatorio
        {
            private int idprojeto { get; set; }
            private DateTime dtinicio { get; set; }
            private string descricao { get; set; }
            private string status { get; set; }
            private string nome { get; set; }
            private string tipo { get; set; }
            private string email { get; set; }
            private string contato { get; set; }
            private string fonecontato { get; set; }
            private string celularcontato { get; set; }
            private string cnpjcpf { get; set; }
            private int idprodutomaterial { get; set; }
            private decimal quantidade { get; set; }
            private decimal vlprodutomaterial { get; set; }
            private string nmmaterial { get; set; }
            private string nmunidademedida { get; set; }
            private string nmfabricante { get; set; }
            private decimal vlmaterial { get; set; }
            private int idmaterial { get; set; }

            internal static DataTable GetDataTable(List<Relatorio> items)
            {
                var _result = new DataTable("table");

                _result.Columns.Add("idprojeto", Type.GetType("System.Int32"));
                _result.Columns.Add("dtinicio", Type.GetType("System.DateTime"));
                _result.Columns.Add("descricao", Type.GetType("System.String"));
                _result.Columns.Add("status", Type.GetType("System.String"));
                _result.Columns.Add("email", Type.GetType("System.String"));
                _result.Columns.Add("nome", Type.GetType("System.String"));
                _result.Columns.Add("tipo", Type.GetType("System.String"));
                _result.Columns.Add("contato", Type.GetType("System.String"));
                _result.Columns.Add("fonecontato", Type.GetType("System.String"));
                _result.Columns.Add("celularcontato", Type.GetType("System.String"));
                _result.Columns.Add("idprodutomaterial", Type.GetType("System.Int32"));
                _result.Columns.Add("quantidade", Type.GetType("System.Decimal"));
                _result.Columns.Add("vlprodutomaterial", Type.GetType("System.Decimal"));
                _result.Columns.Add("nmmaterial", Type.GetType("System.String"));
                _result.Columns.Add("nmunidademedida", Type.GetType("System.String"));
                _result.Columns.Add("nmfabricante", Type.GetType("System.String"));
                _result.Columns.Add("vlmaterial", Type.GetType("System.Decimal"));
                _result.Columns.Add("idmaterial", Type.GetType("System.Int32"));

                foreach (var grupo in items.GroupBy(u => u.idmaterial))
                {
                    var item = grupo.First();
                    var row = _result.NewRow();
                    row["idprojeto"] = item.idprojeto;
                    row["dtinicio"] = item.dtinicio;
                    row["descricao"] = item.descricao;
                    row["status"] = item.status;
                    row["email"] = item.email;
                    row["nome"] = item.nome;
                    row["tipo"] = item.tipo;
                    row["contato"] = item.contato;
                    row["fonecontato"] = item.fonecontato;
                    row["celularcontato"] = item.celularcontato;
                    row["idprodutomaterial"] = item.idprodutomaterial;
                    row["nmmaterial"] = item.nmmaterial;
                    row["vlprodutomaterial"] = item.vlprodutomaterial;
                    row["quantidade"] = grupo.ToList().Sum(u => u.quantidade);
                    row["nmunidademedida"] = item.nmunidademedida;
                    row["nmfabricante"] = item.nmfabricante;
                    row["vlmaterial"] = item.vlmaterial;
                    row["idmaterial"] = item.idmaterial;
                    row["vlprodutomaterial"] = item.vlprodutomaterial;
                    _result.Rows.Add(row);
                }
                return _result;
            }
        }
    }
}
