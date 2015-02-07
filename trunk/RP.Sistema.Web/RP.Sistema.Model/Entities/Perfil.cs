using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Web.Script.Serialization;

namespace RP.Sistema.Model.Entities
{
    public class Perfil
	{
	    public Perfil()
		{
            this.Acoes = new List<PerfilAcao>();
            this.Usuarios = new List<PerfilUsuario>();
		}

        public int idPerfil { get; set; }
        public string nmPerfil { get; set; }

        public ICollection<PerfilAcao> Acoes { get; set; }
        public ICollection<PerfilUsuario> Usuarios { get; set; }
	}
}

