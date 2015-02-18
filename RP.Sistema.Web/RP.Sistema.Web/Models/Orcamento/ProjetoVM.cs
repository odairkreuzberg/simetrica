using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace RP.Sistema.Web.Models.Orcamento
{
    public class ProjetoVM
    {
        public class ProdutoVM
        {
            public string nome { get; set; }
            public string descricao { get; set; }
            public int? idProduto { get; set; }
        }
        [Display(Name = "Projeto")]
        public string descricao { get; set; }
        public int? idProjeto { get; set; }
        public List<ProdutoVM> Produtos {get;set;}
    }
}