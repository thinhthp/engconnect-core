using EngConnect.Repositories.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EngConnect.Repositories.Repositories.TutorProfile
{
    public class TutorProfileRepository : ITutorProfileRepository
    {
        private readonly EngConnectContext _context;

        public TutorProfileRepository(EngConnectContext context)
        {
            _context = context;
        }

        public async Task<Entities.Entities.TutorProfile?> GetByTutorIdAsync(string tutorId, CancellationToken cancellationToken = default)
        {
            return await _context.TutorProfiles
                .FirstOrDefaultAsync(p => p.TutorId == tutorId, cancellationToken);
        }

        public async Task<bool> ExistsAsync(string tutorId, CancellationToken cancellationToken = default)
        {
            return await _context.TutorProfiles
                .AsNoTracking()
                .AnyAsync(p => p.TutorId == tutorId, cancellationToken);
        }

        public async Task AddAsync(Entities.Entities.TutorProfile profile, CancellationToken cancellationToken = default)
        {
            await _context.TutorProfiles.AddAsync(profile, cancellationToken);
        }
    }
}
