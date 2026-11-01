using EngConnect.Repositories.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Threading.Tasks;

namespace EngConnect.Repositories.Common
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly EngConnectContext _context;
        private IDbContextTransaction? _transaction;

        public UnitOfWork(EngConnectContext context)
        {
            _context = context;
        }

        public async Task<int> SaveChangesAsync()
        {
            try
            {
                return await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException ex)
            {
                throw new Exception("A concurrency error occurred while saving changes.", ex);
            }
            catch (DbUpdateException ex)
            {
                throw new Exception("An error occurred while updating the database.", ex);
            }
        }

        public async Task BeginTransactionAsync()
        {
            if (_transaction != null)
            {
                return;
            }

            _transaction = await _context.Database.BeginTransactionAsync();
        }

        public async Task CommitAsync()
        {
            if (_transaction == null)
            {
                return;
            }

            await _transaction.CommitAsync();
            await _transaction.DisposeAsync();
            _transaction = null;
        }

        public async Task RollbackAsync()
        {
            if (_transaction == null)
            {
                return;
            }

            await _transaction.RollbackAsync();
            await _transaction.DisposeAsync();
            _transaction = null;
        }

        public void Dispose()
        {
            _transaction?.Dispose();
            _context.Dispose();
        }
    }
}