using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Web.Script.Serialization;

namespace RP.Sistema.Model.Entities
{

    public class Area
	{
	    public Area()
        {
            this.Controles = new List<Controle>();
        }

        public int idArea { get; set; }
        public string nmArea { get; set; }
        public string dsArea { get; set; }
        public int idModulo { get; set; }
        public string flUsaURL { get; set; }

        public Modulo Modulo { get; set; }
        public ICollection<Controle> Controles { get; set; }
	}
}

