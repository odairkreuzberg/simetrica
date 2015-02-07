using RP.DataAccess.Interfaces;
using RP.Sistema.Model.Entities;
using RP.Util;
using System.Collections.Generic;
using System.Linq;

namespace RP.Sistema.BLL
{
    public class CartaoPontoBLL : RP.DataAccess.Repository<CartaoPonto>
    {
        public CartaoPontoBLL(IContext db, int idUsuario) : base(db, idUsuario) { }
    }
}
