using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Web.Script.Serialization;

namespace RP.Sistema.Model.Entities
{
    public class Menu
	{
        public Menu()
		{
			this.Acoes = new List<Acao>();
		}

        public int idMenu { get; set; }
        public string nmMenu { get; set; }
        public int? nrOrdem { get; set; }
        public string dsCor { get; set; }

        public ICollection<Acao> Acoes { get; set; }
	}
}

