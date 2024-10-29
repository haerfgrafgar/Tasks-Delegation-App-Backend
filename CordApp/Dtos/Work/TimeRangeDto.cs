using System.ComponentModel.DataAnnotations;

namespace CordApp.Dtos.Work
{
    public class TimeRangeDto
    {
        [Required]
        public DateTime dateStart { get; set; }
        [Required]
        public DateTime dateEnd { get; set; }
    }
}
