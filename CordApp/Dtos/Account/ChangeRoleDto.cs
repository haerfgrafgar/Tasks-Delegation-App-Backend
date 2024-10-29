using System.ComponentModel.DataAnnotations;

namespace CordApp.Dtos.Account
{
    public class ChangeRoleDto
    {
        [Required]
        public string username { get; set; }
        [Required]
        public string role { get; set; }
    }
}
