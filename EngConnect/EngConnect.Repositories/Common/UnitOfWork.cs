using EngConnect.Repositories.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EngConnect.Repositories.Common
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly EngConnectContext _context;

        // public ISomethingRepository SomethingRepository { get; private set; }
        // Right here baby


        public UnitOfWork(EngConnectContext context)
        {
            _context = context;

            // SomethingRepository = new SomethingRepository(_context);
            // Right here baby

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

        public void Dispose()
        {
            _context?.Dispose();
        }
    }
}
