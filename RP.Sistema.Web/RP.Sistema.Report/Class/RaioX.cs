using RP.Sistema.BLL;
using RP.Sistema.Model.Entities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace RP.Sistema.Report.Class
{
    public class RaioX
    {
        public System.Web.Mvc.ActionResult GetReport(Model.Context db, int idProjeto, int _idUsuario)
        {
            String titulo = "<center>Raio X do projeto<center>";
            return Report.genericReport(
                new Report.genericReportData
                {
                    exportTO = Report.stringTOExportFormatType("PDF"),
                    fileRPT = "RaioX.rpt",
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
                {"produtosub", getDataProduto(db, idProjeto, _idUsuario)},
                {"custosub", getDataCusto(db, idProjeto, _idUsuario)}
            };

            return listData;
        }

        private DataSet getDataCusto(Model.Context db, int idProjeto, int _idUsuario)
        {
            var dataSet = new DataSet();
            var table = new DataTable("table");
            System.Data.DataRow row;

            table.Columns.Add("descricao", Type.GetType("System.String"));
            table.Columns.Add("valor", Type.GetType("System.Decimal"));
            table.Columns.Add("vlVenda", Type.GetType("System.Decimal"));
            table.Columns.Add("dtCusto", Type.GetType("System.DateTime"));

            var _bll = new ProjetoBLL(db, _idUsuario);

            var _projeto = _bll.FindSingle(u => u.idProjeto == idProjeto,
                u => u.Requisicoes.Select(k => k.Funcionario),
                u => u.Requisicoes.Select(k => k.RequisicaoItens),
                u => u.ProjetoCustos,
                u => u.Produtos);

            foreach (var item in _projeto.ProjetoCustos.Where(u => u.situacao == ProjetoCusto.NORMAL))
            {
                row = table.NewRow();
                row["descricao"] = item.descricao;
                row["valor"] = item.valor;
                row["vlVenda"] = _projeto.vlVenda;
                row["dtCusto"] = item.dtCusto;
                table.Rows.Add(row);

            }

            row = table.NewRow();
            row["descricao"] = "Comissão do vendedor";
            row["valor"] = ((_projeto.porcentagemVendedor * _projeto.vlVenda) / 100);
            row["dtCusto"] = _projeto.dtFim;
            row["vlVenda"] = _projeto.vlVenda;
            table.Rows.Add(row);

            decimal comissaoMarceneito = 0;
            decimal comissaoProjetista = 0;

            foreach (var item in _projeto.Produtos)
            {
                comissaoMarceneito += (((item.porcentagemMarceneiro ?? 0) * (item.vlVenda ?? 0)) / 100);
                comissaoProjetista += (((item.porcentagemProjetista ?? 0) * (item.vlVenda ?? 0)) / 100);
            }
            row = table.NewRow();
            row["descricao"] = "Comissão dos projetistas";
            row["valor"] = comissaoProjetista;
            row["dtCusto"] = _projeto.dtFim;
            row["vlVenda"] = _projeto.vlVenda;
            table.Rows.Add(row);

            row = table.NewRow();
            row["descricao"] = "Comissão dos marceniros";
            row["valor"] = comissaoMarceneito;
            row["dtCusto"] = _projeto.dtFim;
            row["vlVenda"] = _projeto.vlVenda;
            table.Rows.Add(row);

            foreach (var requisicao in _projeto.Requisicoes)
            {
                decimal total = 0;
                foreach (var u in requisicao.RequisicaoItens)
                {
                    total += u.vlPreco * u.nrQuantidade;
                }
                row = table.NewRow();
                row["descricao"] = "Requisição: " + requisicao.dsObservacao + ", responsavél: " + requisicao.Funcionario.nome;
                row["valor"] = total;
                row["dtCusto"] = requisicao.dtRequisicao;
                row["vlVenda"] = _projeto.vlVenda;
                table.Rows.Add(row);
            }

            dataSet.Tables.Add(table);
            return dataSet;
        }

        private DataSet getDataProduto(Model.Context db, int idProjeto, int _idUsuario)
        {
            var dataSet = new DataSet();
            var table = new DataTable("table");
            System.Data.DataRow row;
            table.Columns.Add("idProduto", Type.GetType("System.Int32"));
            table.Columns.Add("descricaoProduto", Type.GetType("System.String"));
            table.Columns.Add("nomeProduto", Type.GetType("System.String"));
            table.Columns.Add("vlVenda", Type.GetType("System.Decimal"));
            table.Columns.Add("vlProduto", Type.GetType("System.Decimal"));
            table.Columns.Add("margemGanhoProduto", Type.GetType("System.Decimal"));
            table.Columns.Add("porcentagemMarceneiro", Type.GetType("System.Decimal"));
            table.Columns.Add("porcentagemProjetista", Type.GetType("System.Decimal"));
            table.Columns.Add("vlDesconto", Type.GetType("System.Decimal"));
            table.Columns.Add("nomeProjetista", Type.GetType("System.String"));
            table.Columns.Add("nomeMarceneiro", Type.GetType("System.String"));
            table.Columns.Add("nmmaterial", Type.GetType("System.String"));
            table.Columns.Add("nmunidademedida", Type.GetType("System.String"));
            table.Columns.Add("nmfabricante", Type.GetType("System.String"));
            table.Columns.Add("vlmaterial", Type.GetType("System.Decimal"));
            table.Columns.Add("margemGanhoMaterial", Type.GetType("System.Decimal"));
            table.Columns.Add("idmaterial", Type.GetType("System.Int32"));
            table.Columns.Add("quantidade", Type.GetType("System.Decimal"));
            table.Columns.Add("quantidade1", Type.GetType("System.Decimal"));
            table.Columns.Add("idprodutomaterial", Type.GetType("System.Int32"));
            var _bll = new ProdutoBLL(db, _idUsuario);
            var _produtos = _bll.Find(u => u.idProjeto == idProjeto,
                u => u.Marceneiro,
                u => u.Projetista,
                u => u.ProdutoMateriais.Select(a => a.Material.UnidadeMedida),
                u => u.ProdutoMateriais.Select(a => a.Material.Fabricante));

            foreach (var produto in _produtos)
            {
                if (produto.ProdutoMateriais.Any())
                {
                    foreach (var item in produto.ProdutoMateriais)
                    {
                        row = table.NewRow();

                        row["idProduto"] = produto.idProduto;
                        row["nomeProduto"] = produto.nome;
                        row["descricaoProduto"] = produto.descricao;
                        row["vlDesconto"] = produto.vlDesconto;
                        row["vlVenda"] = produto.vlVenda;
                        row["vlProduto"] = produto.vlProduto;
                        row["margemGanhoProduto"] = produto.margemGanho ?? 0;
                        row["porcentagemMarceneiro"] = produto.porcentagemMarceneiro;
                        row["porcentagemProjetista"] = produto.porcentagemProjetista;
                        row["nomeProjetista"] = produto.Projetista == null ? "" : produto.Projetista.nome;
                        row["nomeMarceneiro"] = produto.Marceneiro == null ? "" : produto.Marceneiro.nome;

                        row["idprodutomaterial"] = item.idProdutoMaterial;
                        row["nmmaterial"] = item.Material.nome;
                        row["quantidade"] = item.quantidade;
                        row["quantidade1"] = item.quantidade;
                        row["nmunidademedida"] = item.Material.UnidadeMedida.nome;
                        row["nmfabricante"] = item.Material.Fabricante != null ? item.Material.Fabricante.nome : "";
                        row["vlmaterial"] = item.valor;
                        row["idmaterial"] = item.idMaterial;
                        table.Rows.Add(row);

                    }
                }
                else
                {
                    row = table.NewRow();
                    row["idProduto"] = produto.idProduto;
                    row["nomeProduto"] = produto.nome;
                    row["descricaoProduto"] = produto.descricao;
                    row["vlDesconto"] = produto.vlDesconto;
                    row["vlVenda"] = produto.vlVenda;
                    row["vlProduto"] = produto.vlProduto;
                    row["margemGanhoProduto"] = produto.margemGanho ?? 0;
                    row["porcentagemMarceneiro"] = produto.porcentagemMarceneiro;
                    row["porcentagemProjetista"] = produto.porcentagemProjetista;
                    row["nomeProjetista"] = produto.Projetista.nome;
                    row["nomeMarceneiro"] = produto.Marceneiro.nome;
                    table.Rows.Add(row);
                }


            }

            dataSet.Tables.Add(table);
            return dataSet;
        }

        private DataSet GetDataSet(Model.Context db, int idProjeto, int _idUsuario)
        {
            var _bll = new RP.Sistema.BLL.ProjetoBLL(db, _idUsuario);
            var _projeto = _bll.FindSingle(u => u.idProjeto == idProjeto, u => u.Cliente, u => u.Vendedor, u => u.Produtos);

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
                _result.Columns.Add("dtFim", Type.GetType("System.DateTime"));
                _result.Columns.Add("descricao", Type.GetType("System.String"));
                _result.Columns.Add("status", Type.GetType("System.String"));
                _result.Columns.Add("email", Type.GetType("System.String"));
                _result.Columns.Add("nome", Type.GetType("System.String"));
                _result.Columns.Add("tipo", Type.GetType("System.String"));
                _result.Columns.Add("contato", Type.GetType("System.String"));
                _result.Columns.Add("fonecontato", Type.GetType("System.String"));
                _result.Columns.Add("celularcontato", Type.GetType("System.String"));
                _result.Columns.Add("previsaotermino", Type.GetType("System.String"));
                _result.Columns.Add("vlVenda", Type.GetType("System.Decimal"));
                _result.Columns.Add("vlDesconto", Type.GetType("System.Decimal"));
                _result.Columns.Add("vlProjeto", Type.GetType("System.Decimal"));
                _result.Columns.Add("porcentagemVendedor", Type.GetType("System.Decimal"));
                _result.Columns.Add("nomevendedor", Type.GetType("System.String"));

                var dias = _projeto.dtFim.Value.Date.Subtract(_projeto.dtInicio.Value.Date);
                var row = _result.NewRow();
                row["vlVenda"] = _projeto.vlVenda;
                row["vlDesconto"] = _projeto.vlDesconto;
                row["vlProjeto"] = _projeto.vlProjeto;
                row["porcentagemVendedor"] = _projeto.porcentagemVendedor;
                row["nomevendedor"] = _projeto.Vendedor.nome;
                row["dtinicio"] = _projeto.dtInicio;
                row["dtfim"] = _projeto.dtFim;
                row["descricao"] = _projeto.descricao;
                row["status"] = _projeto.status;
                row["email"] = _projeto.Cliente.email;
                row["nome"] = _projeto.Cliente.nome;
                row["tipo"] = _projeto.Cliente.tipo;
                row["contato"] = _projeto.Cliente.contato;
                row["fonecontato"] = _projeto.Cliente.foneContato;
                row["celularcontato"] = _projeto.Cliente.celularContato;
                _result.Rows.Add(row);
                return _result;
            }
        }
    }
}
