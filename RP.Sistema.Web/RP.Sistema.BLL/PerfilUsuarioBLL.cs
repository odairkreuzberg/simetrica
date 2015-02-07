using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.Entity;
using RP.Sistema.Model.Entities;
using RP.DataAccess.Interfaces;

namespace RP.Sistema.BLL
{
    public class PerfilUsuarioBLL : DataAccess.Repository<PerfilUsuario>
    {
        public PerfilUsuarioBLL(IContext db, int idUsuario) : base(db, idUsuario) { }
    }
}
