using System;
using System.Data;
using System.Data.Common;
using Microsoft.EntityFrameworkCore.Storage;

namespace Website.Data.EF.Repositories
{
    public class RepositoryTransaction : DbTransaction, IDisposable
    {
        protected override DbConnection DbConnection { get; }
        public override IsolationLevel IsolationLevel { get; }
        protected IDbContextTransaction InternalTransaction;
        private bool Completed = false;

        internal RepositoryTransaction(IDbContextTransaction efTransaction, IsolationLevel isolationLevel,
            bool inMemory = false)
        {
            if (!inMemory)
            {
                if (efTransaction == null) throw new ArgumentNullException(nameof(efTransaction));
            }

            DbConnection = efTransaction?.GetDbTransaction().Connection;
            InternalTransaction = efTransaction;
            IsolationLevel = isolationLevel;
        }

        public override void Commit()
        {
            InternalTransaction?.Commit();
            Completed = true;
        }

        public override void Rollback()
        {
            if (!Completed)
                InternalTransaction?.Rollback();
        }

        protected override void Dispose(bool disposing)
        {
            Rollback();
        }
    }
}