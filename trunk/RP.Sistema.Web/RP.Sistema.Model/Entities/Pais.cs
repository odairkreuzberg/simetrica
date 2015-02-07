using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Web.Script.Serialization;

namespace RP.Sistema.Model.Entities
{
    public class Pais
	{
        public Pais()
        {
            this.Estados = new List<Estado>();
        }

        public int idPais { get; set; }
        public string nome { get; set; }
        public string sigla { get; set; }
        public ICollection<Estado> Estados { get; set; }

	}
}

