using RP.Util.Class;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using System.Linq;
using System;

namespace RP.Sistema.Web.Models.Projeto
{
    public class AprovarVM
    {
        public static readonly SelectListItem[] Status = new[]
        {
            new SelectListItem { Text = Sistema.Model.Entities.Projeto.ORCAMENTO, Value = Sistema.Model.Entities.Projeto.ORCAMENTO }, 
            new SelectListItem { Text = Sistema.Model.Entities.Projeto.VENDIDO, Value = Sistema.Model.Entities.Projeto.VENDIDO }, 
            new SelectListItem { Text = Sistema.Model.Entities.Projeto.NAO_VENDIDO, Value = Sistema.Model.Entities.Projeto.NAO_VENDIDO }
        };

        public class Parcela
        {
            public string flFormaPagamento { get; set; }
            public int nrParcela { get; set; }
            public string dsObservacao { get; set; }
            public DateTime dtVencimento { get; set; }
            public decimal vlParcela { get; set; }
        }

        public class ProdutoVM
        {
            public string nome { get; set; }
            public string descricao { get; set; }
            public int? idProjetista { get; set; }
            public int? idMarceneiro { get; set; }
            public int? idProduto { get; set; }

            public string projetista { get; set; }
            public string marceneiro { get; set; }
            public decimal comissaoMarceneiro { get; set; }
            public decimal comissaoProjetista { get; set; }
            public decimal porcentagemMarceneiro { get; set; }
            public decimal porcentagemProjetista { get; set; }

            public decimal vlVenda { get; set; }
            public decimal vlProduto { get; set; }
            public decimal vlLucro { get; set; }
            public decimal vlLiquido { get; set; }
            public decimal vlLiquidoDesconto { get; set; }
            public decimal vlDesconto { get; set; }
            public decimal margemGanho { get; set; }


            public decimal vlLiquidoMaterial { get; set; }
            public decimal vlCustoMaterial { get; set; }
            public decimal vlTotalMaterial { get; set; }


            internal static List<ProdutoVM> GetProdutos(ICollection<Model.Entities.Produto> collection, List<Model.Entities.Compra> list)
            {
                var _resutl = new List<ProdutoVM>();
                decimal teste = 0;
                foreach (var item in list.Where(u => u.flCancelado == "Não"))
                {
                    teste += item.total;
                }

                foreach (var item in collection)
                {
                    decimal vlCustoMaterial = 0;
                    decimal vlTotalMaterial = 0;
                    decimal vlLiquidoMaterial = 0;
                    if (item.ProdutoMateriais.Any())
                    {
                        foreach (var material in item.ProdutoMateriais)
                        {
                            decimal custo = material.quantidade * material.valor;
                            vlCustoMaterial += custo;
                            decimal ganho = ((material.margemGanho / 100) * custo) + custo;
                            vlTotalMaterial += ganho;
                            vlLiquidoMaterial += ganho - custo;
                        }
                    }
                    decimal vlLiquido = (((item.margemGanho ?? 0) / 100) * vlTotalMaterial);
                    decimal vlProduto = vlLiquido + vlTotalMaterial;
                    decimal porcentagemProjetista = item.porcentagemProjetista ?? (item.Projetista.comissao ?? 0);
                    decimal porcentagemMarceneiro = item.porcentagemMarceneiro ?? (item.Marceneiro.comissao ?? 0);

                    decimal comissaoMarceineiro = (porcentagemMarceneiro / 100) * vlProduto;
                    decimal comissaoProjetista = (porcentagemProjetista / 100) * vlProduto;
                    decimal vlVenda = vlProduto - (item.vlDesconto ?? 0);

                    _resutl.Add(new ProdutoVM
                    {
                        nome = item.nome,
                        descricao = item.descricao,
                        idMarceneiro = item.idMarceneiro,
                        idProjetista = item.idProjetista,
                        idProduto = item.idProduto,

                        marceneiro = item.Marceneiro == null ?"": item.Marceneiro.nome,
                        projetista = item.Projetista == null ? "" : item.Projetista.nome,
                        comissaoMarceneiro = comissaoMarceineiro,
                        comissaoProjetista = comissaoProjetista,

                        porcentagemMarceneiro = porcentagemMarceneiro,
                        porcentagemProjetista = porcentagemProjetista,

                        vlVenda = vlVenda,
                        vlProduto = vlProduto,
                        vlLiquido = vlLiquido,
                        vlLiquidoDesconto = vlVenda - vlProduto,
                        vlDesconto = item.vlDesconto ?? 0,
                        margemGanho = item.margemGanho ?? 0,
                        vlLiquidoMaterial = vlLiquidoMaterial,
                        vlCustoMaterial = vlCustoMaterial,
                        vlTotalMaterial = vlTotalMaterial,
                        vlLucro = vlVenda - ((item.vlDesconto ?? 0) + comissaoMarceineiro + comissaoProjetista + vlCustoMaterial)
                    });
                }
                return _resutl;
            }

            internal static ICollection<Model.Entities.Produto> GetProdutos(List<ProdutoVM> list)
            {
                var _resutl = new List<Model.Entities.Produto>();
                foreach (var item in list)
                {
                    _resutl.Add(new Model.Entities.Produto
                    {
                        nome = item.nome,
                        descricao = item.descricao,
                        idMarceneiro = item.idMarceneiro,
                        idProjetista = item.idProjetista,
                        porcentagemProjetista = item.porcentagemProjetista,
                        porcentagemMarceneiro = item.porcentagemMarceneiro,
                        vlDesconto = item.vlDesconto,
                        vlProduto = item.vlProduto,
                        vlVenda = item.vlVenda,
                        margemGanho = item.margemGanho,
                        idProduto = item.idProduto ?? 0,

                    });
                }
                return _resutl;
            }
        }

        public Sistema.Model.Entities.Projeto GetProjeto()
        {
            var _result = new Sistema.Model.Entities.Projeto
            {
                descricao = this.descricao,
                dtInicio = this.dtInicio,
                dtFim = this.dtFim,
                idProjeto = this.idProjeto,
                status = this.status,
                idCliente = this.Cliente.idCliente ?? 0,
                idVendedor = this.Vendedor.idFuncionario ?? 0,
                porcentagemVendedor = this.porcentagemVendedor,
                vlDesconto = this.vlDescontoProjeto,
                vlVenda = this.vlTotalProjeto,
                vlProjeto = this.vlProjeto,
                Produtos = ProdutoVM.GetProdutos(this.Produtos)
            };
            return _result;
        }

        public static AprovarVM GetProjeto(Sistema.Model.Entities.Projeto model)
        {
            var _result = new AprovarVM
            {
                descricao = model.descricao,
                idProjeto = model.idProjeto,
                status = model.status,
                dtInicio = model.dtInicio,
                dtFim = model.dtFim,
                Cliente = Models.Cliente.Consultar.GetModel(model.Cliente),
                Vendedor = Models.Funcionario.Consultar.GetModel(model.Vendedor),
                Produtos = ProdutoVM.GetProdutos(model.Produtos, model.Compras.ToList()),
                porcentagemVendedor = model.porcentagemVendedor ?? (model.Vendedor.comissao ?? 0),
                vlCustos = model.ProjetoCustos.Sum(u => u.valor)
            };

            return _result;
        }
        public int idProjeto { get; set; }

        [Display(Name = "Data de Início")]
        public DateTime? dtInicio { get; set; }

        [Display(Name = "Data de término")]
        public DateTime? dtFim { get; set; }

        [Display(Name = "Descrição")]
        [Required(ErrorMessage = "Informe a descrição")]
        public string descricao { get; set; }

        [Display(Name = "Situação")]
        public string status { get; set; }

        [Display(Name = "Comissão")]
        public decimal comissaoVendedor { get; set; }

        [Display(Name = "Margem de ganho (material)")]
        public decimal? margemGanhoMaterial { get; set; }

        [Display(Name = "Comissão (%)")]
        public decimal porcentagemVendedor { get; set; }

        public decimal vlProjeto { get; set; }
        public decimal vlTotalProjeto { get; set; }
        public decimal vlDescontoProjeto { get; set; }
        public decimal vlCustos { get; set; }

        public Cliente.Consultar Cliente { get; set; }
        public Funcionario.Consultar Vendedor { get; set; }

        [EnsureMinimumElements(1, ErrorMessage = "Imforme um produto")]
        public List<ProdutoVM> Produtos { get; set; }
        public List<Parcela> Parcelas { get; set; }

    }
}