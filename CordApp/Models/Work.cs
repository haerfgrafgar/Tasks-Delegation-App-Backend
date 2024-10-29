using System.ComponentModel.DataAnnotations.Schema;

namespace CordApp.Models
{
    [Table("Work")]
    public class Work
    {
        public int Id { get; set; }
        public string ExecId { get; set; }
        public string CordId { get; set; }
        public bool Finished { get; set; }
        public bool Terminated {  get; set; }
        public DateTime CreationDate { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? DueDate { get; set; }
        public bool? Approved { get; set; }
        public string Description { get; set; }
        public string Title { get; set; }
        public bool WasLate { get; set; }
        public string? ExecResponse { get; set; }
        public string? CordResponse { get; set; }
        public int PreviousWorkVersionId { get; set; }
        public DateTime? DeletedByExec { get; set; }
        public int SecondsTaken { get; set; }
        public bool InProgressNow { get; set; }
        public int Priority { get; set; }
    }
}
