using Microsoft.AspNetCore.Identity;

namespace CordApp.Models
{
    public class AppUser : IdentityUser
    {
        public string CordId { get; set; }
        //public List<int>? ExecIds { get; set; }
        public List<int>? CurrentWorksId { get; set; }
        public required string Position { get; set; }
        public int CompletedWorksNumber { get; set; }
        public int ApprovedWorksNumber { get; set; }
        public int DisaprovedWorksNumber { get; set; }
        public int WasLateWorksNumber { get; set; }
        public int TotalSecondsInWork { get; set; }
        public int TodayIdleWorkId { get; set; }
        public int WorkInProgressId { get; set; }
        public int FractionInProgressId { get; set; }
        public int TotalSecondsInIdle { get; set; }
    }
}
