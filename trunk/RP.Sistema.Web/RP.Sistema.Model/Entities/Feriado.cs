using System;
using System.Collections.Generic;

namespace RP.Sistema.Model.Entities
{
    public partial class Feriado
    {

        public int idFeriado { get; set; }
        public int nrDia { get; set; }
        public int nrMes { get; set; }
        public string nmFeriado { get; set; }
    }
}
