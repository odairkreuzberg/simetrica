using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.Entity;
using System.Transactions;
using System.Data.Objects;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Data.Entity.Infrastructure;
using RP.DataAccess.Interfaces;
using System.ComponentModel.Composition;
using System.Data;
using RP.Sistema.Model.Mapping;

namespace RP.Sistema.Model
{
    public partial class Context : DbContext, RP.DataAccess.Interfaces.IContext
    {
        public Context() : base("Entities.RP.Sistema") 
        {
            //log = new List<Log.Model.Entities.LogDado>();
            Database.SetInitializer<Context>(null);
            this.Configuration.ValidateOnSaveEnabled = false;
        }
 
        public int SaveChanges(int idUsuario)
        {
            lock(this)
            {
                this._idUsuario = idUsuario;
                var _changeEntries = this.ChangeTracker.Entries().Where(p => p.State == System.Data.EntityState.Added
                        || p.State == System.Data.EntityState.Deleted
                        || p.State == System.Data.EntityState.Modified);

                //foreach (var entity in _changeEntries)
                //{
                //    List<RP.Log.Model.Entities.LogDado> logs = RP.Log.Model.Reg.GetData(this ,entity, _idUsuario);
                //    if(logs != null)
                //        _log.AddRange(logs);
                //}

                return base.SaveChanges();
            }
        }

        public override int SaveChanges()
        {
            throw new InvalidOperationException("É necessário informar o usuario.");
        }

    }
}