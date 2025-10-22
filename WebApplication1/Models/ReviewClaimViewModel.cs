namespace WebApplication1.Models
{
    public class ReviewClaimViewModel
    {
        public int ClaimId { get; set; }
        public string LecturerName { get; set; }
        public decimal TotalAmount { get; set; }
        public string Notes { get; set; }
        public string Status { get; set; }
        public string RejectionReason { get; set; }
    }
}
