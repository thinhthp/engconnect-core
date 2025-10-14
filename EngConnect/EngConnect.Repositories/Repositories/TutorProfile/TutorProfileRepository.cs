using EngConnect.Repositories.Data;
using EngConnect.Repositories.Repositories.TutorProfile.Filters;
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

        public async Task<(List<Entities.Entities.TutorProfile> Items, int Total)> QueryAsync(
            TutorProfileQueryParameters parameters,
            CancellationToken cancellationToken = default)
        {
            var query = _context.TutorProfiles.AsNoTracking().AsQueryable();

            if (!string.IsNullOrWhiteSpace(parameters.Search))
            {
                var s = parameters.Search.Trim();
                query = query.Where(p =>
                    (p.TutorId != null && p.TutorId.Contains(s)) ||
                    (p.Bio != null && p.Bio.Contains(s)) ||
                    (p.Language != null && p.Language.Contains(s)));
            }

            if (parameters.Approved.HasValue)
                query = query.Where(p => p.Approved == parameters.Approved.Value);

            if (parameters.IsActive.HasValue)
                query = query.Where(p => p.IsActive == parameters.IsActive.Value);

            if (!string.IsNullOrWhiteSpace(parameters.Language))
                query = query.Where(p => p.Language == parameters.Language);

            if (!string.IsNullOrWhiteSpace(parameters.TutorId))
                query = query.Where(p => p.TutorId == parameters.TutorId);

            // Sorting (default newest by CreatedAt desc)
            var sortBy = (parameters.SortBy ?? "createdAt").ToLowerInvariant();
            var sortDir = (parameters.SortDir ?? "desc").ToLowerInvariant();

            query = (sortBy, sortDir) switch
            {
                ("experienceyears", "asc") => query.OrderBy(p => p.ExperienceYears).ThenByDescending(p => p.CreatedAt),
                ("experienceyears", "desc") => query.OrderByDescending(p => p.ExperienceYears).ThenByDescending(p => p.CreatedAt),
                ("language", "asc") => query.OrderBy(p => p.Language).ThenByDescending(p => p.CreatedAt),
                ("language", "desc") => query.OrderByDescending(p => p.Language).ThenByDescending(p => p.CreatedAt),
                ("approved", "asc") => query.OrderBy(p => p.Approved).ThenByDescending(p => p.CreatedAt),
                ("approved", "desc") => query.OrderByDescending(p => p.Approved).ThenByDescending(p => p.CreatedAt),
                ("createdat", "asc") => query.OrderBy(p => p.CreatedAt),
                _ => query.OrderByDescending(p => p.CreatedAt)
            };

            var total = await query.CountAsync(cancellationToken);

            var page = parameters.PageNumber <= 0 ? 1 : parameters.PageNumber;
            var size = parameters.PageSize <= 0 ? 10 : parameters.PageSize;

            var items = await query
                .Skip((page - 1) * size)
                .Take(size)
                .ToListAsync(cancellationToken);

            return (items, total);
        }
    }
}