using System;
using System.Data;
using System.Data.Common;

namespace Website.Data.EF.Repositories
{
    public class RepositoryTransaction : DbTransaction, IDisposable
    {
        protected override DbConnection DbConnection { get; }
        public override IsolationLevel IsolationLevel { get; }
        protected DbTransaction InternalTransaction;
        
        internal RepositoryTransaction(DbConnection dbConnection, IsolationLevel isolationLevel, bool inMemory = false)
        {
            if (!inMemory)
            {
                DbConnection = dbConnection ?? throw new ArgumentNullException(nameof(dbConnection));
                InternalTransaction = DbConnection.BeginTransaction(isolationLevel);
            }
            IsolationLevel = isolationLevel;
        }
        
        public override void Commit()
        {
            InternalTransaction?.Commit();
        }

        public override void Rollback()
        {
            InternalTransaction?.Rollback();
        }
        
        protected override void Dispose(bool disposing)
        {
            InternalTransaction?.Rollback();
        }
    }
}