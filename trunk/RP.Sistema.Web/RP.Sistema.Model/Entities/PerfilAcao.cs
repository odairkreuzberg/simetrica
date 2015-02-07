using System.ComponentModel.DataAnnotations.Schema;
using System.Web.Script.Serialization;

namespace RP.Sistema.Model.Entities
{
    public class PerfilAcao
    {
        public int idPerfil { get; set; }
        public int idAcao { get; set; }

        public Perfil Perfil { get; set; }
        public Acao Acao { get; set; }
    }
}
