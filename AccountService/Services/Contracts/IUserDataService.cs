using System.Threading.Tasks;
using System.Collections.Generic;
using AccountService.Models;
using AccountService.Models.RequestModels;


namespace AccountService.Services.Contracts
{
    public interface IUserDataService
    {
        Task<User> GetUser(int userId);
        Task<User> GetUserByUsername(string username);


        Task<bool> CheckUserPassword(string username, string inputPassword);


        Task<User> Create(User newUser, string password);

        Task<User> UpdateUser(UpdateUserRequestModel request);

        Task<bool> UpdateUserPassword(UpdateUserPasswordRequestModel request);

        Task<IEnumerable<User>> GetUsersByRole(Role role);
        Task<IEnumerable<User>> GetUsersWithoutRole();

        Task<bool> RemoveUserFromRole(int userId, int roleId);
        Task<bool> AddUserToRole(int userId, int roleId);
    }
}
