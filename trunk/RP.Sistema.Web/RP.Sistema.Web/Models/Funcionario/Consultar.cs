using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RP.Sistema.Web.Models.Funcionario
{
    public class Consultar
    {
        public int? idFuncionario { get; set; }
        public string nome { get; set; }

        internal static Consultar GetModel(Model.Entities.Funcionario Funcionario)
        {
            var _result = new Consultar();

            if(Funcionario !=null)
            {
                _result.idFuncionario = Funcionario.idFuncionario;
                _result.nome = Funcionario.nome;
            }
            return _result;
        }
    }
}