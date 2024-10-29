using System.ComponentModel.DataAnnotations;

namespace CordApp.Dtos.Task
{
    public class CreateWorkRequestDto
    {
        [Required]
        public string ExecUsername { get; set; }
        [Required]
        public string Description { get; set; }
        [Required]
        public string Title { get; set; }
        [Required]
        [Range(1, 10)]
        public int Priority { get; set; }
        public DateTime? DueDate { get; set; }
        [Required]
        public DateTime StartDate { get; set; }
    }
}
