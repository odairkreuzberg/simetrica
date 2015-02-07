using RP.DataAccess.Interfaces;
using RP.Sistema.Model.Entities;
using RP.Util;
using System;
using System.Collections.Generic;
using System.Linq;

namespace RP.Sistema.BLL
{
    public class FolhaPagamentoBLL : RP.DataAccess.Repository<FolhaPagamento>
    {
        public FolhaPagamentoBLL(IContext db, int idUsuario) : base(db, idUsuario) { }

        protected override void BeforeInsert(FolhaPagamento bean)
        {
            bean.idUsuario = this._idUsuario;
            bean.dtPagamento = DateTime.Now;

        }
    }
}
