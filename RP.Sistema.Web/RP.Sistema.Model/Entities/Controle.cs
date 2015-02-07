using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Web.Script.Serialization;

namespace RP.Sistema.Model.Entities
{
    public class Controle
	{
	    public Controle()
        {
            this.Acoes = new List<Acao>();
        }

        public int idControle { get; set; }
        public string nmControle { get; set; }
        public string dsControle { get; set; }
        public int idArea { get; set; }

        public ICollection<Acao> Acoes { get; set; }
		public Area Area { get; set; }
	}
}

