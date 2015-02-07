using System.ComponentModel.DataAnnotations.Schema;
using System.Web.Script.Serialization;

namespace RP.Sistema.Model.Entities
{
    public class ModuloUsuario
    {
        public int idModulo { get; set; }
        public int idUsuario { get; set; }

        public Modulo Modulo { get; set; }
        public Usuario Usuario { get; set; }
    }
}
