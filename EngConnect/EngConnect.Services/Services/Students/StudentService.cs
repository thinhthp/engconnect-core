using EngConnect.Entities.Entities;
using EngConnect.Services.DTOs.Account;
using Microsoft.AspNetCore.Identity;

namespace EngConnect.Services.Services.Students;

public class StudentService : IStudentService
{
    private readonly UserManager<ApplicationUser> _userManager;

    public StudentService(UserManager<ApplicationUser> userManager)
    {
        _userManager = userManager;
    }

    public async Task<List<UserResponse>> GetStudentsAsync(CancellationToken cancellationToken = default)
    {
        
        var users = await _userManager.GetUsersInRoleAsync("Student");


        return users.Select(u => new UserResponse
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
        }).ToList();
    }
}
