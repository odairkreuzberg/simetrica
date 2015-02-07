using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RP.Sistema.Web.Models.Material
{
    public class Consultar
    {
        public int? idMaterial { get; set; }
        public string nome { get; set; }
        public decimal preco { get; set; }
        public decimal margemGanho { get; set; }

        internal static Consultar GetModel(Model.Entities.Material model)
        {
            var _result = new Consultar();

            if(model !=null)
            {
                _result.idMaterial = model.idMaterial;
                _result.nome = model.nome;
                _result.preco = model.preco;
                _result.margemGanho = model.margemGanho;
            }
            return _result;
        }
    }
}