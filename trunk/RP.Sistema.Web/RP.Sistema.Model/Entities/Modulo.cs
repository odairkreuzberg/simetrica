using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Web.Script.Serialization;

namespace RP.Sistema.Model.Entities
{
    public class Modulo 
	{
	    public Modulo()
		{
			this.Areas = new List<Area>();
            this.Usuarios = new List<ModuloUsuario>();
		}

        public int idModulo { get; set; }
        public string nmModulo { get; set; }
        public string dsModulo { get; set; }
        public byte[] btImageMenu { get; set; }
        public Nullable<int> nrOrdem { get; set; }
        public string nmURL { get; set; }

        public ICollection<Area> Areas { get; set; }
        public ICollection<ModuloUsuario> Usuarios { get; set; }
    }
}

