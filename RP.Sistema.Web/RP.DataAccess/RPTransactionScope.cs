using RP.DataAccess.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace RP.DataAccess
{
    public sealed class RPTransactionScope : IDisposable
    {
        private TransactionScope _transactionScope = null;
        private IContext _db;

        #region Overloaded Constructors
        public RPTransactionScope(IContext db)
        {
            _db = db;
            _transactionScope = new TransactionScope();
        }

        public RPTransactionScope(IContext db, Transaction transactionToUse)
        {
            _db = db;
            _transactionScope = new TransactionScope(transactionToUse);
        }

        public RPTransactionScope(IContext db, TransactionScopeOption scopeOption)
        {
            _db = db;
            _transactionScope = new TransactionScope(scopeOption);
        }

        #endregion

        public void Complete()
        {
            _transactionScope.Complete();
            _db.SaveLog();
        }

        public void Dispose()
        {
            _transactionScope.Dispose();
        }
    }
}
