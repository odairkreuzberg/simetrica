using RP.DataAccess.Interfaces;
using RP.Sistema.Model.Entities;

namespace RP.Sistema.BLL
{
    public class PerfilAcaoBLL: DataAccess.Repository<PerfilAcao>
    {
        public PerfilAcaoBLL(IContext db, int idUsuario) : base(db, idUsuario) { }
    }
}
