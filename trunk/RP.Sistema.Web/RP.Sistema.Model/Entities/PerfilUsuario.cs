using System.ComponentModel.DataAnnotations.Schema;
using System.Web.Script.Serialization;

namespace RP.Sistema.Model.Entities
{
    public class PerfilUsuario
    {
        public int idPerfil { get; set; }
        public int idUsuario { get; set; }

        public Perfil Perfil { get; set; }
        public Usuario Usuario { get; set; }
    }
}
