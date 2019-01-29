using System;
using System.Data;
using System.Data.Common;
using Microsoft.EntityFrameworkCore.Storage;

namespace Website.Data.EF.Repositories
{
    public class EmptyRepositoryTransaction : DbTransaction, IDisposable
    {
        protected override DbConnection DbConnection { get; }
        public override IsolationLevel IsolationLevel { get; }

        internal EmptyRepositoryTransaction()
        {
        }

        public override void Commit()
        {
        }

        public override void Rollback()
        {
        }

        protected override void Dispose(bool disposing)
        {
        }
    }
}