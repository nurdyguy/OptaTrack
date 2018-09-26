using System.Collections.Generic;
using System.Threading.Tasks;
using AccountService.Models;

namespace AccountService.Repositories.Contracts
{
    public interface IUserRoleRepository
    {
        Task<IEnumerable<UserRole>> GetUserRolesByUser(int userId);
        Task<UserRole> Create(int userId, int roleId);

        Task<IEnumerable<UserRole>> GetUserRolesByRole(int roleId);

        Task<UserRole> Delete(int userId, int roleId);
    }
}
