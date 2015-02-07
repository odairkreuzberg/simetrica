using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RP.Sistema.Web.Models
{
    public class MenuVM
    {
        public string Modulo { get; set; }
        public string Grupo { get; set; }
        public string Nome { get; set; }
        public string Descricao { get; set; }
        public string Url { get; set; }
        public string Icone { get; set; }
    }
}