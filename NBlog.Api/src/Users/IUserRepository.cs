using NBlog.Api.Categories;

namespace NBlog.Api.Users;

public interface IUserRepository
{
    Task<AppUser> GetByName(string name);
    Task<AppUser> GetByEmail(string email);
    Task<AppUser> GetById(string id);
    Task<FollowAction> Follow(string userName, string otherUserName);
}