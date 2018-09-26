
namespace AccountService.Models.RequestModels
{
    public class UpdateUserPasswordRequestModel
    {
        public int UserId { get; set; }
        public string NewPassword { get; set; }
    }
}
