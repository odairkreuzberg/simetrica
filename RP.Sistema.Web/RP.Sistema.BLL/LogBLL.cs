using RP.Sistema.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RP.Sistema.BLL
{
    public class LogBLL
    {
        public static void Insert(Model.Entities.LogDado logDado)
        {
            using (Context db = new Context())
            {
                using (var trans = new RP.DataAccess.RPTransactionScope(db))
                {
                    db.LogDados.Add(logDado);
                    db.SaveChanges(logDado.idUsuario);
                    trans.Complete();
                }
            }
        }
    }
}
