using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.Extensions.Options;
using Dapper;
using AccountService.Models;
using AccountService.Repositories.Contracts;


namespace AccountService.Repositories.Implementations
{
    public class UserRoleRepository : SqlRepository, IUserRoleRepository
    {
        public UserRoleRepository(AccountServiceOptions options) : base(options)
        {
        }

        public async Task<IEnumerable<UserRole>> GetUserRolesByUser(int userId)
        {
            using (var conn = await GetConnection())
            {
                string query = @"Select * From [UserRoles] 
                                    Where [UserId] = @userId ";
                var result = await conn.QueryAsync<UserRole>(query, new { userId });
                return result;
            }
        }

        public async Task<UserRole> Create(int userId, int roleId)
        {
            using (var conn = await GetConnection())
            {
                string query = @"Insert into [UserRoles] 
                                    (UserId, RoleId)
                                Output Inserted.*
                                    values (@userId, @roleId) ";
                var result = await conn.QueryAsync<UserRole>(query, new { userId, roleId });
                return result.Single();
            }
        }

        public async Task<IEnumerable<UserRole>> GetUserRolesByRole(int roleId)
        {
            using (var conn = await GetConnection())
            {
                string query = @"Select * From [UserRoles] 
                                    Where [RoleId] = @roleId ";
                var result = await conn.QueryAsync<UserRole>(query, new { roleId });
                return result;
            }
        }

        public async Task<UserRole> Delete(int userId, int roleId)
        {
            using (var conn = await GetConnection())
            {
                string query = @"Delete From [UserRoles] 
                                 output deleted.*
                                    Where [RoleId] = @roleId 
                                    and [UserId] = @userId";
                var result = await conn.QueryAsync<UserRole>(query, new { roleId, userId });
                return result.SingleOrDefault();
            }
        }

    }
}
