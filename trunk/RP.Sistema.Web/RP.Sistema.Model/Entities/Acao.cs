using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Web.Script.Serialization;

namespace RP.Sistema.Model.Entities
{
    public class Acao
	{
	    public Acao()
        {
            this.Perfis = new List<PerfilAcao>();
        }

        public int idAcao { get; set; }
        public string nmAcao { get; set; }
        public string dsAcao { get; set; }
        public string flMenu { get; set; }
        public string nmMenu { get; set; }
        public int idControle { get; set; }
        public int? idMenu { get; set; }
        public Nullable<int> nrOrdem { get; set; }
        public string dsIcone { get; set; }

        public Controle Controle { get; set; }
        public Menu Menu { get; set; }
        public ICollection<PerfilAcao> Perfis { get; set; }

	}
}

