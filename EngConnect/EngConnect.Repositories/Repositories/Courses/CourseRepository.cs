using EngConnect.Entities.Entities;
using EngConnect.Repositories.Repositories.Courses.Filters;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EngConnect.Repositories.Repositories.Courses
{
    public class CourseRepository : ICourseRepository
    {
        private readonly EngConnect.Repositories.Data.EngConnectContext _context;
        public CourseRepository(EngConnect.Repositories.Data.EngConnectContext context)
        {
            _context = context;
        }

        public async Task<Course> AddAsync(Course course, CancellationToken cancellationToken = default)
        {
            await _context.Courses.AddAsync(course, cancellationToken);
            return course;
        }

        public async Task<Course?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            return await _context.Courses
                .AsNoTracking()
                .Include(c => c.Tutor)
                .FirstOrDefaultAsync(c => c.CourseId == id, cancellationToken);
        }

        public async Task<(IReadOnlyList<Course> Items, int TotalCount)> QueryAsync(
            CourseQueryParameters parameters,
            CancellationToken cancellationToken = default)
        {
            IQueryable<Course> query = _context.Courses.AsNoTracking();

            // Default visibility for students: approved + active
            if (parameters.Status is null)
            {
                query = query.Where(c => c.Status == "approved");
            }
            else
            {
                query = query.Where(c => c.Status == parameters.Status);
            }

            if (parameters.IsActive.HasValue)
                query = query.Where(c => c.IsActive == parameters.IsActive.Value);
            else
                query = query.Where(c => c.IsActive); // default only active

            if (!string.IsNullOrWhiteSpace(parameters.Level))
                query = query.Where(c => c.Level == parameters.Level);

            if (!string.IsNullOrWhiteSpace(parameters.TutorId))
                query = query.Where(c => c.TutorId == parameters.TutorId);

            if (parameters.MinPrice.HasValue)
                query = query.Where(c => c.Price >= parameters.MinPrice.Value);

            if (parameters.MaxPrice.HasValue)
                query = query.Where(c => c.Price <= parameters.MaxPrice.Value);

            if (!string.IsNullOrWhiteSpace(parameters.Search))
            {
                string s = parameters.Search.Trim().ToLower();
                query = query.Where(c =>
                    c.Title.ToLower().Contains(s) ||
                    (c.Description != null && c.Description.ToLower().Contains(s)));
            }

            int total = await query.CountAsync(cancellationToken);

            // Sorting
            bool desc = string.Equals(parameters.SortDir, "desc", StringComparison.OrdinalIgnoreCase);
            switch (parameters.SortBy?.ToLower())
            {
                case "title":
                    query = desc ? query.OrderByDescending(c => c.Title) : query.OrderBy(c => c.Title);
                    break;
                case "price":
                    query = desc ? query.OrderByDescending(c => c.Price) : query.OrderBy(c => c.Price);
                    break;
                case "totalsessions":
                    query = desc ? query.OrderByDescending(c => c.TotalSessions) : query.OrderBy(c => c.TotalSessions);
                    break;
                case "createdat":
                default:
                    query = desc ? query.OrderByDescending(c => c.CreatedAt) : query.OrderBy(c => c.CreatedAt);
                    break;
            }

            // Paging
            if (parameters.PageNumber < 1) parameters.PageNumber = 1;
            if (parameters.PageSize < 1) parameters.PageSize = 10;

            List<Course> items = await query
                .Skip(parameters.Skip)
                .Take(parameters.PageSize)
                .ToListAsync(cancellationToken);

            return (items, total);
        }
    }
}