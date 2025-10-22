

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Data;
using WebApplication1.Models;
using static WebApplication1.Models.LecturerClaim; //watch for error
namespace WebApplication1.Controllers
{
    [Authorize(Roles = "Coordinator")]
    public class CoordinatorController : Controller
    {
        private readonly ApplicationDbContext _context;
        public CoordinatorController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Show all pending claims
        public async Task<IActionResult> Index()
        {
            var pendingClaims = await _context.LecturerClaims
                .Include(c => c.User)
                .Include(c => c.Documents)
                .Where(c => c.Status == ClaimStatus.Pending)
                .OrderByDescending(c => c.SubmittedAt)
                .ToListAsync();

            return View(pendingClaims);
        }

        // View claim details
        public async Task<IActionResult> Review(int id)
        {
            var claim = await _context.LecturerClaims
                .Include(c => c.User)
                .Include(c => c.Documents)
                .FirstOrDefaultAsync(c => c.ClaimId == id);

            if (claim == null)
                return NotFound();

            return View(claim);
        }

        // Approve claim
        [HttpPost]
        public async Task<IActionResult> Approve(int id)
        {
            var claim = await _context.LecturerClaims.FindAsync(id);
            if (claim == null) return NotFound();

            claim.Status = LecturerClaim.ClaimStatus.Approved; // this sets status
            claim.ReviewedAt = DateTime.UtcNow;
            claim.ReviewedBy = User.Identity.Name;

            await _context.SaveChangesAsync();
            TempData["Success"] = "Claim approved.";
            return RedirectToAction(nameof(Index));
        }

       
        [HttpPost]
        public async Task<IActionResult> Reject(int id, string reason)
        {
            var claim = await _context.LecturerClaims.FindAsync(id);
            if (claim == null) return NotFound();

            claim.Status = LecturerClaim.ClaimStatus.Rejected; // ← this sets status
            claim.ReviewedAt = DateTime.UtcNow;
            claim.ReviewedBy = User.Identity.Name;
            claim.Notes += $"\n[Rejected Reason]: {reason}";

            await _context.SaveChangesAsync();
            TempData["Error"] = "Claim rejected.";
            return RedirectToAction(nameof(Index));
        }
    }
}