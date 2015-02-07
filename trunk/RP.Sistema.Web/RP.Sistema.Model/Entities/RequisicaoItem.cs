using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Web.Script.Serialization;

namespace RP.Sistema.Model.Entities
{

    public class RequisicaoItem
    {
        public int idItem { get; set; }
        public int idMaterial { get; set; }
        public decimal nrQuantidade { get; set; }
        public decimal vlPreco { get; set; }
        public int idRequisicao { get; set; }
        public Material Material { get; set; }
        public Requisicao Requisicao { get; set; }
	}
}

