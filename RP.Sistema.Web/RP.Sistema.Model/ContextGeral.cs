using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace RP.Sistema.Model
{
    public partial class Context : DbContext, RP.DataAccess.Interfaces.IContext
    {
        public enum eDataBaseType
        {
            POSTGRESQL,
            SQLSERVER,
            ORACLE,
            MYSQL
        };

        private int _idUsuario;
       // private List<RP.Log.Model.Entities.LogDado> _log;

        public static string Schema { get { return System.Configuration.ConfigurationManager.AppSettings["schema"]; } }

        //TODO IMPLEMENTAR O RESTANTE DAS OPÇOES
        public eDataBaseType DatabaseType
        {
            get
            {
                this.Database.Connection.Open();
                var _temp = this.Database.Connection.ServerVersion ?? string.Empty;
                this.Database.Connection.Close();
                _temp = _temp.ToLower();
                if (_temp.Contains("postgre"))
                {
                    return eDataBaseType.POSTGRESQL;
                }
                return eDataBaseType.SQLSERVER;
            }
        }

        #region IContext Implementation

        public void ChangeState<T>(T entity, EntityState state) where T : class
        {
            Entry<T>(entity).State = state;
        }

        public IDbSet<T> GetEntitySet<T>()
        where T : class
        {
            return Set<T>();
        }

        private static bool IsChanged(DbEntityEntry entity)
        {
            return IsStateEqual(entity, EntityState.Added) ||
                   IsStateEqual(entity, EntityState.Deleted) ||
                   IsStateEqual(entity, EntityState.Modified);
        }

        private static bool IsStateEqual(DbEntityEntry entity, EntityState state)
        {
            return (entity.State & state) == state;
        }

        public void SaveLog()
        {
            using (var tran = new TransactionScope(TransactionScopeOption.Suppress))
            {
                //using (RP.Log.Model.Context contextLog = new Log.Model.Context())
                //{
                //    foreach (RP.Log.Model.Entities.LogDado log in _log)
                //    {
                //        contextLog.LogDado.Add(log);
                //    }
                //    contextLog.SaveChanges();
                //}
            }
        }

        #endregion
    }

}
