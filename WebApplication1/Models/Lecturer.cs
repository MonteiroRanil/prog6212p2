using System.ComponentModel.DataAnnotations;

namespace WebApplication1.Models
{
    public class Lecturer
    {
        [Key] 
        public int LecturerId { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }

        // relaationship with claims
        public ICollection<LecturerClaim> Claims { get; set; }
    }
}
