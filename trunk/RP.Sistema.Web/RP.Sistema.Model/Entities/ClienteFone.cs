using System;
using System.Collections.Generic;

namespace RP.Sistema.Model.Entities
{
    public partial class ClienteFone
    {
        public int idClienteFone { get; set; }
        public int idCliente { get; set; }
        public string tipo { get; set; }
        public string numero { get; set; }
        public Cliente Cliente { get; set; }
    }
}
