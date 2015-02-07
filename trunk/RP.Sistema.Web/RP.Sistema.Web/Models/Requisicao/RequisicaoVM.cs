using RP.Sistema.Model.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace RP.Sistema.Web.Models.Requisicao
{
    public class RequisicaoVM
    {
        public class RequisicaoItemVM
        {
            public int idMaterial { get; set; }
            public string nome { get; set; }
            public decimal quantidade { get; set; }

            internal static ICollection<Model.Entities.RequisicaoItem> GetItens(List<RequisicaoItemVM> list)
            {
                var _result = new List<Model.Entities.RequisicaoItem>();
                if (list != null)
                {
                    foreach (var item in list)
                    {
                        _result.Add(new Model.Entities.RequisicaoItem { idMaterial = item.idMaterial, nrQuantidade = item.quantidade });
                    }
                }
                return _result;
            }

            internal static List<RequisicaoItemVM> GetItens(List<Model.Entities.Produto> list)
            {
                var _result = new List<RequisicaoItemVM>();
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
                            _result.Add(new RequisicaoItemVM
                            {
                                idMaterial = material.idMaterial,
                                nome = material.Material.nome,
                                quantidade = material.quantidade
                            });
                        }
                    }
                }
                return _result;
            }

            internal static List<RequisicaoItemVM> GetItens(List<RequisicaoItem> list)
            {
                var _result = new List<RequisicaoItemVM>();
                foreach (var item in list)
                {
                    _result.Add(new RequisicaoItemVM
                    {
                        idMaterial = item.idMaterial,
                        nome = item.Material.nome,
                        quantidade = item.nrQuantidade
                    });
                }
                return _result;
            }
        }
        public Sistema.Model.Entities.Requisicao GetRequisicao()
        {
            var _result = new Sistema.Model.Entities.Requisicao
            {
                dsObservacao = this.dsObservacao,
                idRequisicao = this.idRequisicao,
                idFuncionario = this.Funcionario.idFuncionario ?? 0,
                idProjeto = this.Projeto.idProjeto ?? 0,
                RequisicaoItens = RequisicaoItemVM.GetItens(this.Itens)
            };
            return _result;
        }

        public static RequisicaoVM GetRequisicao(Sistema.Model.Entities.Requisicao model)
        {
            var _result = new RequisicaoVM
            {
                dsObservacao = model.dsObservacao,
                idRequisicao = model.idRequisicao,
                Projeto = Models.Projeto.Consultar.GetModel(model.Projeto),
                Funcionario = Models.Funcionario.Consultar.GetModel(model.Funcionario),
                Itens = RequisicaoItemVM.GetItens(model.RequisicaoItens.ToList())
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

        public int idRequisicao { get; set; }

        [Display(Name = "Observação")]
        public string dsObservacao { get; set; }

        public Funcionario.Consultar Funcionario { get; set; }
        public Projeto.Consultar Projeto { get; set; }

        public List<RequisicaoItemVM> Itens { get; set; }
    }
}