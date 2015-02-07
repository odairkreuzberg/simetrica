using System;
using System.Collections.Generic;

namespace RP.Sistema.Model.Entities
{
    public partial class HoraExtra
    {

        public int idHora { get; set; }
        public System.TimeSpan? inicioHora { get; set; }
        public System.TimeSpan? fimHora { get; set; }
        public decimal porcentagem { get; set; }
        public string flTipo { get; set; }
    }
}
