using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace WebApplication1.Models
{
    public class LecturerClaim
    {
        public enum ClaimStatus
        {
            Pending,
            Approved,
            Rejected
        }
           [Key]
            public int ClaimId { get; set; }
           
           
           
        public string UserId { get; set; }
        public IdentityUser User { get; set; }
        public decimal HoursWorked { get; set; }
            public decimal HourlyRate { get; set; }
            public decimal TotalAmount { get; set; }
            public string Notes { get; set; }

            public ClaimStatus Status { get; set; } = ClaimStatus.Pending;
            public DateTime SubmittedAt { get; set; } = DateTime.UtcNow;
            public DateTime? ReviewedAt { get; set; }
            public string? ReviewedBy { get; set; }

            // Relationship with documents
            public ICollection<ClaimDocument> Documents { get; set; }
      

    }
}
