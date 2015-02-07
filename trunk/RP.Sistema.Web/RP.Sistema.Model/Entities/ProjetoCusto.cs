using System;
using System.Collections.Generic;

namespace RP.Sistema.Model.Entities
{
    public partial class ProjetoCusto
    {
        public static string CANCELADO = "Cancelado";
        public static string NORMAL = "Normal";
        public int idProjetoCusto { get; set; }
        public string descricao { get; set; }
        public decimal valor { get; set; }
        public string gerarConta { get; set; }
        public string situacao { get; set; }
        public int idProjeto { get; set; }
        public int? idContaPagar { get; set; }
        public DateTime? dtCusto { get; set; }
        public Projeto projeto { get; set; }
    }
}
