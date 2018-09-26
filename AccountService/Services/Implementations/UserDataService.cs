using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Security.Cryptography;
using System.Linq;
using AccountService.Repositories.Contracts;
using AccountService.Services.Contracts;
using AccountService.Models;
using AccountService.Models.RequestModels;


namespace AccountService.Services.Implementations
{
    public class UserDataService : IUserDataService
    {
        private readonly IUserRepository _userRepo;
        private readonly IUserRoleRepository _userRoleRepo;

        public UserDataService(IUserRepository userRepo, IUserRoleRepository userRoleRepo)
        {
            _userRepo = userRepo;
            _userRoleRepo = userRoleRepo;
        }

        public async Task<User> GetUser(int userId)
        {
            var user = await _userRepo.GetUser(userId);
            if (user != null)
                await BuildUpUser(user);

            return user;
        }

        public async Task<User> GetUserByUsername(string username)
        {
            var user = await _userRepo.GetUserByEmail(username);
            if (user != null)
                await BuildUpUser(user);

            return user;
        }

        public async Task<bool> CheckUserPassword(string email, string inputPassword)
        {
            if (String.IsNullOrEmpty(email) || String.IsNullOrEmpty(inputPassword))
                return false;

            var user = await _userRepo.GetUserByEmail(email);
            if (user == null)
                return false;

            return CompareHash(inputPassword, user.Salt, user.HashedPassword) || String.Equals(inputPassword, user.TempPassword);
        }

        public async Task<User> Create(User newUser, string password)
        {
            ValidateEmail(newUser.Email);
            ValidatePassword(password);

            newUser.Salt = GenerateSalt();
            newUser.HashedPassword = HashPassword(password, newUser.Salt);

            var createdUser = await _userRepo.Create(newUser);

            var CreatedUserRole = await _userRoleRepo.Create(createdUser.UserId, Role.User.ID);

            createdUser.Roles = new List<Role>() { Role.User };

            return createdUser;
        }


        public async Task<User> UpdateUser(UpdateUserRequestModel request)
        {
            var user = await _userRepo.GetUser(request.UserId);
            if (user == null)
                return user;

            user.FirstName = request.FirstName;
            user.LastName = request.LastName;
            user.Email = request.Email;
            user.PhoneNumber = request.PhoneNumber;

            user = await _userRepo.UpdateUser(user);

            await BuildUpUser(user);
            return user;
        }

        public async Task<bool> UpdateUserPassword(UpdateUserPasswordRequestModel request)
        {
            var newSalt = GenerateSalt();
            var newHashedPassword = HashPassword(request.NewPassword, newSalt);
            var result = await _userRepo.UpdateUserPassword(request.UserId, newSalt, newHashedPassword);
            return result;
        }

        public async Task<IEnumerable<User>> GetUsersByRole(Role role)
        {
            var userRoles = await _userRoleRepo.GetUserRolesByRole(role.ID);
            var users = new List<User>();
            if (userRoles != null)
            {
                users = (await Task.WhenAll(userRoles.Select(ur => GetUser(ur.UserId)))).ToList();
            }
            return users;
        }

        public async Task<IEnumerable<User>> GetUsersWithoutRole()
        {
            var users = await _userRepo.GetUsersWithoutUserRole();

            // don't need to build up UserRole
            //await Task.WhenAll(users.Select(u => BuildUpUser(u)));
            return users;
        }

        public async Task<bool> RemoveUserFromRole(int userId, int roleId)
        {
            if (userId == 0)
                throw new Exception("Invalid userId");
            if (roleId == 0)
                throw new Exception("Invalid roleId");

            var removedUserRole = await _userRoleRepo.Delete(userId, roleId);

            if (removedUserRole != null)
                return true;
            else
                return false;
        }

        public async Task<bool> AddUserToRole(int userId, int roleId)
        {
            if (userId == 0)
                throw new Exception("Invalid userId");
            if (roleId == 0)
                throw new Exception("Invalid roleId");

            var newUserRole = await _userRoleRepo.Create(userId, roleId);

            if (newUserRole != null)
                return true;
            else
                return false;
        }







        private bool CompareHash(string raw, string salt, string currentPwHash)
        {
            return String.Equals(HashPassword(raw, salt), currentPwHash);
        }

        public string GenerateSalt()
        {
            using (RandomNumberGenerator rng = RandomNumberGenerator.Create())
            {
                byte[] salt = new byte[4];
                rng.GetBytes(salt);
                string saltString = string.Empty;
                foreach (byte x in salt)
                {
                    saltString += string.Format("{0:x2}", x);
                }
                return saltString;
            }
        }

        public string HashPassword(string password, string salt)
        {
            string fullPassword = password + "" + salt;
            byte[] clearText = Encoding.Unicode.GetBytes(fullPassword);
            using (SHA256 test = SHA256.Create())
            {
                byte[] hash = test.ComputeHash(clearText);
                string hashString = string.Empty;
                foreach (byte x in hash)
                {
                    hashString += string.Format("{0:x2}", x);
                }
                return hashString;
            }
        }

        private async Task BuildUpUser(User user)
        {
            var userRoles = await _userRoleRepo.GetUserRolesByUser(user.UserId);
            user.Roles = userRoles.Select(ur => ur.Role);
        }

        private void ValidateEmail(string email)
        {
            string emailPattern = @"^[\w!#$%&'*+\-/=?\^_`{|}~]+(\.[\w!#$%&'*+\-/=?\^_`{|}~]+)*@((([\-\w]+\.)+[a-zA-Z]{2,15})|(([0-9]{1,3}\.){3}[0-9]{1,3}))$";

            Regex emailRX = new Regex(emailPattern);
            if (email.Length > 100)
            {
                throw new ArgumentException("Email address is too long");
            }
            else if (email != emailRX.Match(email).Value)
            {
                throw new ArgumentException("Email address is invalid format.");
            }
        }

        private void ValidatePassword(string password)
        {
            if (string.IsNullOrEmpty(password) || password.Length < 8)
            {
                throw new ArgumentException("Password is too short.");
            }
            else if (password.Length > 22)
            {
                throw new ArgumentException("Password is too long");
            }
            else if (password.Contains(" "))
            {
                throw new ArgumentException("Password cannot contain spaces.");
            }
        }
    }
}
