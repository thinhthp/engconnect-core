using EngConnect.Entities.Entities;
using EngConnect.Services.DTOs.Account;
using Microsoft.AspNetCore.Identity;
using EngConnect.Entities.Common;
using System.Linq;

namespace EngConnect.Services.Services.Students;

public class StudentService : IStudentService
{
    private readonly UserManager<ApplicationUser> _userManager;

    public StudentService(UserManager<ApplicationUser> userManager)
    {
        _userManager = userManager;
    }

    public async Task<PagedResult<UserResponse>> GetStudentsAsync(int pageNumber = 1, int pageSize = 10, CancellationToken cancellationToken = default)
    {
        var usersInRole = await _userManager.GetUsersInRoleAsync("Student");

        
        var filtered = usersInRole
            .Where(u => u != null && u.EmailConfirmed)
            .ToList();

        var total = filtered.Count;

        if (pageNumber < 1) pageNumber = 1;
        if (pageSize < 1) pageSize = 10;

        var paged = filtered
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .Select(u => new UserResponse
            {
                Id = u.Id,
                Name = u.UserName,
                Email = u.Email,
                Phone = u.PhoneNumber,
                IsActive = u.IsActive,
                CreateDate = u.CreatedAt,
                UpdateDate = u.UpdateDate,
                CreateBy = u.CreateBy,
                UpdateBy = u.UpdateBy
            })
            .ToList();

        return new PagedResult<UserResponse>
        {
            Items = paged,
            TotalCount = total,
            PageNumber = pageNumber,
            PageSize = pageSize
        };
    }
}
