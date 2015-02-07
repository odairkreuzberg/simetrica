using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace RP.Sistema.Web.Models.FolhaPagamento
{
    public class ListVM
    {

        public class FuncionarioVM
        {
            public int? idFolha { get; set; }
            public int idFuncionario { get; set; }
            public string nome { get; set; }
            public string tipo { get; set; }
            public string situacao { get; set; }
            public decimal? salario { get; set; }
        }

        public class ConsultaVM
        {
            public string nome { get; set; }
            public int mes { get; set; }
            public int ano { get; set; }
        }
        public ConsultaVM Consulta { get; set; }
        public RP.DataAccess.PaginatedList<FuncionarioVM> Funcionarios { get; set; }
        public List<SelectListItem> Ano { get; set; }

        internal static DataAccess.PaginatedList<FuncionarioVM> E2VM(DataAccess.PaginatedList<Model.Entities.Funcionario> _list, int mes, int ano)
        {
            var _result = new RP.DataAccess.PaginatedList<FuncionarioVM>(_list.PageIndex, _list.PageSize, _list.TotalCount, _list.TotalPages);

            foreach (var item in _list)
            {
                var folha = item.FolhaPagamentos.FirstOrDefault(u => u.nrAno == ano && u.nrMes == mes);
                _result.Add(new FuncionarioVM
                {
                    idFuncionario = item.idFuncionario,
                    nome = item.nome,
                    tipo = item.tipo,
                    idFolha = folha == null ? null: (int?)folha.idFolhaPagamento,
                    situacao = folha == null ? "Folha não gerada" : folha.situacao,
                    salario = folha == null ? null : (decimal?)folha.total
                });
            }
            return _result;
        }

        internal static List<SelectListItem> GetAnos()
        {
            int fim = DateTime.Now.Year;
            List<SelectListItem> list = new List<SelectListItem>();
            for (int i = 2013; i <= fim; i++)
            {
                SelectListItem selectList = new SelectListItem
                {
                    Text = i.ToString(),
                    Value = i.ToString()
                };
                list.Add(selectList);
            }

            return list;
        }

        public static readonly SelectListItem[] Mes = new[]
        {
            new SelectListItem { Text = "Janeiro", Value = "1" }, 
            new SelectListItem { Text = "Fevereiro", Value = "2" }, 
            new SelectListItem { Text = "Março", Value = "3" }, 
            new SelectListItem { Text = "Abril", Value = "4" }, 
            new SelectListItem { Text = "Maio", Value = "5" }, 
            new SelectListItem { Text = "Junho", Value = "6" }, 
            new SelectListItem { Text = "Julho", Value = "7" }, 
            new SelectListItem { Text = "Agosto", Value = "8" }, 
            new SelectListItem { Text = "Setembro", Value = "9" }, 
            new SelectListItem { Text = "Outubro", Value = "10" }, 
            new SelectListItem { Text = "Novembro", Value = "11" }, 
            new SelectListItem { Text = "Dezembro", Value = "12" }, 
        };
    }
}