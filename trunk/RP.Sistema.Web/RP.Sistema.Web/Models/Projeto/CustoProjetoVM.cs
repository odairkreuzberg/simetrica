using RP.Util.Class;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using System.Linq;
using System;

namespace RP.Sistema.Web.Models.Projeto
{
    public class CustoProjetoVM
    {
        public class CustoVM
        {
            public string descricao { get; set; }
            public decimal valor { get; set; }
            public string gerarConta { get; set; }
            public DateTime? dtCusto { get; set; }
            public int? idProjetoCusto { get; set; }

            internal static List<CustoVM> GetCustos(ICollection<Model.Entities.ProjetoCusto> collection)
            {
                var _resutl = new List<CustoVM>();
                foreach (var item in collection.Where(u => u.situacao == Model.Entities.ProjetoCusto.NORMAL))
                {
                    _resutl.Add(new CustoVM
                    {
                        valor = item.valor,
                        descricao = item.descricao,
                        gerarConta = item.gerarConta,
                        dtCusto = item.dtCusto,
                        idProjetoCusto = item.idProjetoCusto
                    });
                }
                return _resutl;
            }

            internal static ICollection<Model.Entities.ProjetoCusto> GetCustos(List<CustoVM> list)
            {
                var _result = new List<Model.Entities.ProjetoCusto>();
                if (list != null)
                {
                    foreach (var item in list)
                    {
                        _result.Add(new Model.Entities.ProjetoCusto { dtCusto = item.dtCusto, gerarConta = item.gerarConta, valor = item.valor, descricao =item.descricao, idProjetoCusto = item.idProjetoCusto ??0});
                    }
                }
                return _result;
            }
        }

        public static CustoProjetoVM GetProjeto(Sistema.Model.Entities.Projeto model)
        {
            var _result = new CustoProjetoVM
            {
                descricao = model.descricao,
                idProjeto = model.idProjeto,
                status = model.status,
                dtInicio = model.dtInicio,
                Cliente = Models.Cliente.Consultar.GetModel(model.Cliente),
                Custos = CustoVM.GetCustos(model.ProjetoCustos)
            };

            return _result;
        }
        public int idProjeto { get; set; }

        [Display(Name = "Data de Início")]
        public DateTime? dtInicio { get; set; }

        [Display(Name = "Descrição")]
        public string descricao { get; set; }

        [Display(Name = "Situação")]
        public string status { get; set; }

        public Cliente.Consultar Cliente { get; set; }

        public List<CustoVM> Custos { get; set; }

        internal Sistema.Model.Entities.Projeto GetProjeto()
        {
            var _result = new Sistema.Model.Entities.Projeto
            {
                idProjeto = this.idProjeto,
                ProjetoCustos = CustoProjetoVM.CustoVM.GetCustos(this.Custos)
            };
            return _result;
        }
    }
}