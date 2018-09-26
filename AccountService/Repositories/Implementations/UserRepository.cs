using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Dapper;

using AccountService.Models;
using AccountService.Repositories.Contracts;


namespace AccountService.Repositories.Implementations
{
    public class UserRepository : SqlRepository, IUserRepository
    {
        public UserRepository(AccountServiceOptions options) : base(options)
        {
        }

        public async Task<User> GetUser(int userId)
        {
            using (var conn = await GetConnection())
            {
                string query = @"Select * From [Users] Where [UserId] = @userId ";
                var result = await conn.QueryAsync<User>(query, new { userId });
                return result.SingleOrDefault();
            }
        }

        public async Task<User> GetUserByEmail(string email)
        {
            using (var conn = await GetConnection())
            {
                string query = @"Select * From [Users] Where [Email] = @email ";
                var result = await conn.QueryAsync<User>(query, new { email });
                return result.SingleOrDefault();
            }
        }

        public async Task<User> Create(User newUser)
        {
            using (var conn = await GetConnection())
            {
                string query = @"Insert into [Users] 
                                   (Email, Salt, HashedPassword, IsActive, RegisteredOn)
                                Output Inserted.*
                                    Values (@Email, @Salt, @HashedPassword, @IsActive, @RegisteredOn)";
                var result = await conn.QueryAsync<User>(query,
                    new
                    {
                        Email = newUser.Email,
                        Salt = newUser.Salt,
                        HashedPassword = newUser.HashedPassword,
                        IsActive = 1,
                        RegisteredOn = DateTime.Now
                    });
                return result.SingleOrDefault();
            }
        }

        public async Task<User> UpdateUser(User user)
        {
            using (var conn = await GetConnection())
            {
                string query = @"Update [Users]
                                set 
                                    FirstName = @Firstname,
                                    LastName = @LastName,
                                    Email = @Email,
                                    PhoneNumber = @PhoneNumber                                
                                Output Inserted.*
                                Where [UserId] = @UserId ";
                var result = await conn.QueryAsync<User>(query, user);
                return result.SingleOrDefault();
            }
        }

        public async Task<bool> UpdateUserPassword(int userId, string salt, string newHashedPassword)
        {
            using (var conn = await GetConnection())
            {
                string query = @"Update [Users]
                                set 
                                    Salt = @salt,
                                    HashedPassword = @newHashedPassword                                
                                Output Inserted.*
                                Where [UserId] = @userId ";
                var result = await conn.QueryAsync<User>(query,
                    new
                    {
                        salt,
                        newHashedPassword,
                        userId
                    });
                return result?.SingleOrDefault()?.UserId == userId;
            }
        }


        public async Task<IEnumerable<User>> GetUsersWithoutUserRole()
        {
            using (var conn = await GetConnection())
            {
                string query = @"Select * From [Users] u
                                    left join UserRoles ur on ur.UserId = u.UserId 
                                 where ur.UserId is null";
                var result = await conn.QueryAsync<User>(query);
                return result;
            }
        }
    }
}
