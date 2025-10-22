using System.ComponentModel.DataAnnotations;

namespace WebApplication1.Models
{
    public class CreateClaimViewModel
    {
        [Required, Range(1, 500)]
        public decimal HoursWorked { get; set; }

        [Required, Range(100, 5000)]
        public decimal HourlyRate { get; set; }

        public string Notes { get; set; }

        [Required(ErrorMessage = "Please upload a file")]
        public IFormFile SupportingDocument { get; set; }
    }
}
