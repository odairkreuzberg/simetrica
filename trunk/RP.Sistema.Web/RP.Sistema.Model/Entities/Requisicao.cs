using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Web.Script.Serialization;

namespace RP.Sistema.Model.Entities
{

    public class Requisicao
	{
        public Requisicao()
        {
            this.RequisicaoItens = new List<RequisicaoItem>();
        }

        public int idRequisicao { get; set; }
        public int idProjeto { get; set; }
        public int idFuncionario { get; set; }
        public string dsObservacao { get; set; }
        public int idUsuario { get; set; }
        public System.DateTime dtRequisicao { get; set; }
        public Funcionario Funcionario { get; set; }
        public Projeto Projeto { get; set; }
        public ICollection<RequisicaoItem> RequisicaoItens { get; set; }
	}
}

