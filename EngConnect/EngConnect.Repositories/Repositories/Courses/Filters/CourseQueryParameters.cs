using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EngConnect.Repositories.Repositories.Courses.Filters
{
    public class CourseQueryParameters
    {
        public string? Search { get; set; }
        public string? Level { get; set; }
        public string? Status { get; set; } // If null -> default restrict to approved
        public string? TutorId { get; set; }
        public decimal? MinPrice { get; set; }
        public decimal? MaxPrice { get; set; }
        public bool? IsActive { get; set; }

        public string? SortBy { get; set; } = "createdAt"; // title | price | totalSessions | createdAt
        public string? SortDir { get; set; } = "desc";      // asc | desc

        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;

        public int Skip => (PageNumber - 1) * PageSize;
    }
}
