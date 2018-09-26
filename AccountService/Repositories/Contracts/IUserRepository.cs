using System.Threading.Tasks;
using System.Collections.Generic;
using AccountService.Models;


namespace AccountService.Repositories.Contracts
{
    public interface IUserRepository
    {
        Task<User> GetUser(int userId);
        Task<User> GetUserByEmail(string email);

        Task<User> Create(User newUser);

        Task<User> UpdateUser(User user);

        Task<bool> UpdateUserPassword(int userId, string salt, string hashedPassword);


        Task<IEnumerable<User>> GetUsersWithoutUserRole();
    }
}
