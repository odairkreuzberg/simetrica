using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RP.Sistema.Web.Models.Usuario
{
    /**************************************************************************
     * Classe criada para trafegar dados do usuario de forma simples e segura
     **************************************************************************/
    public class UsuarioModel
    {
        public struct Perfil
        {
            public int idPerfil { get; set; }
            public string nmPerfil { get; set; }
        }

        public int idUsuario { get; set; }
        public string nmUsuario { get; set; }
        public string dsEmail { get; set; }
        public string dsLogin { get; set; }
        public Nullable<DateTime> dtValidade { get; set; }
        public string flAtivo { get; set; }
        public List<Perfil> Perfis { get; set; }
    }
}