using RP.DataAccess.Interfaces;
using RP.Sistema.Model.Entities;
using RP.Util;
using System.Collections.Generic;
using System.Linq;

namespace RP.Sistema.BLL
{
    public class RequisicaoItemBLL : RP.DataAccess.Repository<RequisicaoItem>
    {
        public RequisicaoItemBLL(IContext db, int idUsuario) : base(db, idUsuario) { }
    }
}
