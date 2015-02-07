using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RP.DataAccess.Interfaces
{
    public interface IContext : IDisposable
    {
        IDbSet<T> GetEntitySet<T>() where T : class;
        void ChangeState<T>(T entity, EntityState state) where T : class;
        int SaveChanges(int idUsuario);
        void SaveLog();
    }
}
