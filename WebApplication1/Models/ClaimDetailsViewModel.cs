namespace WebApplication1.Models
{
    public class ClaimDetailsViewModel
    {
        public int ClaimId { get; set; }
        public string LecturerName { get; set; }
        public decimal HoursWorked { get; set; }
        public decimal HourlyRate { get; set; }
        public decimal TotalAmount { get; set; }
        public string Notes { get; set; }
        public string Status { get; set; }
        public List<string> Documents { get; set; } 
    }
}
