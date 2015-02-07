using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RP.Sistema.Web.Models.Projeto
{
    public class Consultar
    {
        public int? idProjeto { get; set; }
        public string descricao { get; set; }
        public string cliente { get; set; }

        internal static Consultar GetModel(Model.Entities.Projeto model)
        {
            var _result = new Consultar();

            if (model != null)
            {
                _result.idProjeto = model.idProjeto;
                _result.descricao = model.descricao;
                if(model.Cliente != null)
                _result.cliente = model.Cliente.nome;
            }
            return _result;
        }
    }
}