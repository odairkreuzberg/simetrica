using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Web.Script.Serialization;

namespace RP.Sistema.Model.Entities
{
    public class CartaoPonto
    {
        public int idPonto { get; set; }
        public System.TimeSpan? entradaManha { get; set; }
        public System.TimeSpan? saidaManha { get; set; }
        public System.TimeSpan? entraTarde { get; set; }
        public System.TimeSpan? saidaTarde { get; set; }
        public System.TimeSpan? entradaExtra { get; set; }
        public System.TimeSpan? saidaExtra { get; set; }
        public int idFuncionario { get; set; }
        public DateTime dtPonto { get; set; }
        public string flSituacao { get; set; }
        public string dsObservacao { get; set; }
        public Funcionario Funcionario { get; set; }


        public static string TRABALHOU = "Trabalhou";
    }
}

