using System.ComponentModel.DataAnnotations.Schema;

namespace CordApp.Models
{
    [Table("FractionOfTime")]
    public class FractionOfTime
    {
        public int Id { get; set; }
        public string AppUserId { get; set; }
        public int WorkId { get; set; }
        public DateTime Begin { get; set; }
        public DateTime End { get; set; }
    }
}
