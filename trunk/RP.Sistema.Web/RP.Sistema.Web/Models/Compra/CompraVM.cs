using RP.Sistema.Model.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace RP.Sistema.Web.Models.Compra
{
    public class CompraVM
    {
        public class ItemCompraVM
        {
            public int idMaterial { get; set; }
            public string nome { get; set; }
            public string fornecedor { get; set; }
            public decimal valor { get; set; }
            public decimal total { get; set; }
            public decimal quantidade { get; set; }

            internal static ICollection<Model.Entities.ItemCompra> GetItens(List<ItemCompraVM> list)
            {
                var _result = new List<Model.Entities.ItemCompra>();
                if (list != null)
                {
                    foreach (var item in list)
                    {
                        _result.Add(new Model.Entities.ItemCompra { valor = item.valor, idMaterial = item.idMaterial, quantidade = item.quantidade });
                    }
                }
                return _result;
            }

            //    internal static List<ItemCompraVM> GetItens(List<ICollection<Model.Entities.ProdutoMaterial>> list)
            //    {
            //        var _resutl = new List<ItemCompraVM>();
            //        foreach (var item in list)
            //        {
            //            _resutl.Add(new ItemCompraVM
            //            {
            //                valor = item.valor,
            //                idMaterial = item.idMaterial,
            //                nome = item.Material.nome,
            //                quantidade = item.quantidade
            //            });
            //        }
            //        return _resutl;
            //    }

            internal static List<ItemCompraVM> GetItens(List<Model.Entities.Produto> list)
            {
                var _result = new List<ItemCompraVM>();
                foreach (var item in list)
                {
                    foreach (var material in item.ProdutoMateriais.ToList())
                    {
                        if (_result.Any(u => u.idMaterial == material.idMaterial))
                        {
                            _result.First(u => u.idMaterial == material.idMaterial).quantidade += material.quantidade;
                        }
                        else
                        {
                            _result.Add(new ItemCompraVM
                            {
                                idMaterial = material.idMaterial,
                                nome = material.Material.nome,
                                quantidade = material.quantidade,
                                valor = material.valor
                            });
                        }
                    }
                }
                return _result;
            }

            internal static List<ItemCompraVM> GetItens(List<Model.Entities.Compra> list)
            {
                var _result = new List<ItemCompraVM>();
                foreach (var compra in list)
                {
                    foreach (var item in compra.ItensCompra.ToList())
                    {
                        _result.Add(new ItemCompraVM
                        {
                            idMaterial = item.idMaterial,
                            nome = item.Material.nome,
                            quantidade = item.quantidade,
                            valor = item.valor,
                            fornecedor = compra.Fornecedor.nome
                        });                        
                    }
                }
                return _result;
            }
        }
        public Sistema.Model.Entities.Compra GetCompra()
        {
            var _result = new Sistema.Model.Entities.Compra
                {
                    descricao = this.descricao,
                    idCompra = this.idCompra,
                    total = this.total,
                    idFornecedor = this.Fornecedor.idFornecedor ?? 0,
                    idProjeto = this.Projeto == null ? null : this.Projeto.idProjeto,
                    ItensCompra = ItemCompraVM.GetItens(this.Itens)
                };
            return _result;
        }

        public static CompraVM GetCompra(Sistema.Model.Entities.Compra model)
        {
            var _result = new CompraVM
            {
                descricao = model.descricao,
                idCompra = model.idCompra,
                total = model.total,
                Parcelas = Parcela.GetParcelas(model.ContasPagar.ToList()),
                Projeto = Models.Projeto.Consultar.GetModel(model.Projeto),
                Fornecedor = Models.Fornecedor.Consultar.GetModel(model.Fornecedor)
            };

            return _result;
        }

        public class Parcela
        {
            public string flFormaPagamento { get; set; }
            public int nrParcela { get; set; }
            public string dsObservacao { get; set; }
            public string flSituacao { get; set; }
            public DateTime dtVencimento { get; set; }
            public decimal vlParcela { get; set; }

            internal static List<Parcela> GetParcelas(List<RP.Sistema.Model.Entities.ContaPagar> list)
            {
                var _result = new List<Parcela>();
                if (list != null)
                {
                    foreach (var item in list)
                    {
                        _result.Add(new Parcela 
                        { 
                            vlParcela = item.valorConta, 
                            nrParcela = item.parcela, 
                            dtVencimento = item.vencimento,
                            dsObservacao = item.descricao,
                            flSituacao = item.situacao,
                            flFormaPagamento = item.flFormaPagamento
                        });
                    }
                }
                return _result;
            }
        }

        public int idCompra { get; set; }

        [Display(Name = "Descrição")]
        [Required(ErrorMessage = "Informe o descricao")]
        public string descricao { get; set; }

        [Display(Name = "Total")]
        [Required(ErrorMessage = "Informe o total")]
        public decimal total { get; set; }

        public Fornecedor.Consultar Fornecedor { get; set; }
        public Projeto.Consultar Projeto { get; set; }
        public List<ItemCompraVM> Itens { get; set; }
        public List<ItemCompraVM> Materiais { get; set; }
        public List<Parcela> Parcelas { get; set; }
    }
}