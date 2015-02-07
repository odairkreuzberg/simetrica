using RP.DataAccess.Interfaces;
using RP.Sistema.Model.Entities;
using RP.Util;
using System;
using System.Collections.Generic;
using System.Linq;

namespace RP.Sistema.BLL
{
    public class ProdutoBLL : RP.DataAccess.Repository<Produto>
    {
        public ProdutoBLL(IContext db, int idUsuario) : base(db, idUsuario) { }
    }

}
