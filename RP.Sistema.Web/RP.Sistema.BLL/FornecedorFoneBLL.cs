using RP.DataAccess.Interfaces;
using RP.Sistema.Model.Entities;
using RP.Util;
using System.Collections.Generic;
using System.Linq;

namespace RP.Sistema.BLL
{
    public class FornecedorFoneBLL : RP.DataAccess.Repository<FornecedorFone>
    {
        public FornecedorFoneBLL(IContext db, int idUsuario) : base(db, idUsuario) { }
    }
}
