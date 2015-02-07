using RP.DataAccess.Interfaces;
using RP.Sistema.Model.Entities;

namespace RP.Sistema.BLL
{
    public class ModuloUsuarioBLL : DataAccess.Repository<ModuloUsuario>
    {
        public ModuloUsuarioBLL(IContext db, int idUsuario) : base(db, idUsuario) { }
    }
}
