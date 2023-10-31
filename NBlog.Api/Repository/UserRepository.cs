using Microsoft.AspNetCore.Identity;
using NBlog.Api.Entities;
using NBlog.Api.Exceptions;

namespace NBlog.Api.Repository;

public class UserRepository : IUserRepository
{
    private readonly UserManager<AppUser> _userManager;

    public UserRepository(UserManager<AppUser> userManager)
    {
        _userManager = userManager;
    }

    public async Task<AppUser> GetByName(string name)
        => await _userManager.FindByNameAsync(name) ?? throw new ResourceNotFoundException("user could not be found");

    public async Task<AppUser> GetByEmail(string email)
        => await _userManager.FindByEmailAsync(email) ?? throw new ResourceNotFoundException("user could not be found");

    public async Task<AppUser> GetById(string id)
        => await _userManager.FindByIdAsync(id) ?? throw new ResourceNotFoundException("user could not be found");
}