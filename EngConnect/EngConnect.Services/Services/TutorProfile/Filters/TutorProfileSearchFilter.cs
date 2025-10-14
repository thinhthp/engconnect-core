using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EngConnect.Services.Services.TutorProfile.Filters
{
    public class TutorProfileSearchFilter
    {
        public string? Search { get; set; }
        public bool? Approved { get; set; }
        public bool? IsActive { get; set; }
        public string? Language { get; set; }
        public string? TutorId { get; set; }

        // Sorting: default newest
        public string? SortBy { get; set; } = "createdAt"; // createdAt | experienceYears | language | approved
        public string? SortDir { get; set; } = "desc";     // asc | desc

        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }
}