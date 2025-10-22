using System.ComponentModel.DataAnnotations;

namespace WebApplication1.Models
{
    public class ClaimDocument
    {
        [Key]
        public int DocumentId { get; set; }
        public int ClaimId { get; set; }
        public LecturerClaim LecturerClaim { get; set; }

        public string FileName { get; set; }
        public string FilePath { get; set; }
        public string ContentType { get; set; }
        public long FileSize { get; set; }
        public DateTime UploadedAt { get; set; } = DateTime.UtcNow;
    }

}
