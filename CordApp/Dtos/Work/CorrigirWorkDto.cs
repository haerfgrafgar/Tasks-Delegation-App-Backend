using System.ComponentModel.DataAnnotations;

namespace CordApp.Dtos.Work
{
    public class CorrigirWorkDto
    {
        [Required]
        public bool Aprovado { get; set; }
        public string? Motivo { get; set; }
    }
}
