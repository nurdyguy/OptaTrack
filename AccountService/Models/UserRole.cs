
namespace AccountService.Models
{
    public class UserRole
    {
        public int UserId { get; set; }
        public Role Role
        {
            get
            {
                return _role;
            }
            set
            {
                _role = value;
                _roleId = value.ID;
            }
        }

        public int RoleId
        {
            get
            {
                return _roleId;
            }
            set
            {
                _role = Role.GetRole(value);
                _roleId = value;
            }
        }

        private Role _role { get; set; }
        private int _roleId { get; set; }

    }
}
