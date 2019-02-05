using System;
using System.Data;
using System.Data.Common;
using Microsoft.EntityFrameworkCore.Storage;

namespace Website.Data.EF.Repositories
{
    public class EmptyRepositoryTransaction : IDbContextTransaction
    {

        internal EmptyRepositoryTransaction()
        {
        }

        public void Dispose()
        {
        }

        public void Commit()
        {
        }

        public void Rollback()
        {
        }

        public Guid TransactionId { get; }
    }
}