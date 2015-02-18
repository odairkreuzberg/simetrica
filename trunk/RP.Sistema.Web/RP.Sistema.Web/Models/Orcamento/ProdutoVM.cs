using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace RP.Sistema.Web.Models.Orcamento
{
    public class ProdutoVM
    {
        public class MaterialVM
        {
            public string nome { get; set; }
            public int? idMaterial { get; set; }
            public decimal? quantidade { get; set; }
        }
        [Display(Name = "Produto")]
        public string nome { get; set; }
        [Display(Name = "Descrição")]
        public string descricao { get; set; }

        public int? idProduto { get; set; }
        public int? idProjeto { get; set; }
        public List<MaterialVM> Itens { get; set; }
    }
}