using System.Collections.Generic;
using RP.Sistema.Web.Models.Usuario;

namespace RP.Sistema.Web.Models
{
    public class SistemaVM
    {
        public List<MenuVM> Menu { get; set; }
        public EditarSenhaVM EditarSenha { get; set; }
        public bool AlterarSenha { get; set; }
        public PainelVM Painel { get; set; }
    }

    public struct PainelVM
    {
        public Model.Entities.Usuario.Preferencias Preferencias { get; set; }
    }
}