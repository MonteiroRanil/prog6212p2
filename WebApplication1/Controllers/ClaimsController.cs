using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Data;
using WebApplication1.Models;
using static WebApplication1.Models.LecturerClaim;

namespace WebApplication1.Controllers
{
    [Authorize(Roles = "Lecturer")]
    public class ClaimsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IWebHostEnvironment _env;

        public ClaimsController(ApplicationDbContext context, UserManager<IdentityUser> userManager, IWebHostEnvironment env)
        {
            _context = context;
            _userManager = userManager;
            _env = env;
        }

        public async Task<IActionResult> Index()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var claims = await _context.LecturerClaims
                .Where(c => c.UserId == userId)
                .Include(c => c.Documents)
                .OrderByDescending(c => c.SubmittedAt)
                .ToListAsync();

            bool updated = false;
            foreach (var claim in claims)
            {
                if (!Enum.IsDefined(typeof(LecturerClaim.ClaimStatus), claim.Status))
                {
                    claim.Status = LecturerClaim.ClaimStatus.Pending;
                    updated = true;
                }
            }

            if (updated)
            {
                await _context.SaveChangesAsync();
            }

            return View(claims);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(decimal HoursWorked, decimal HourlyRate, string Notes, IFormFile SupportingDocument)
        {
            if (HoursWorked <= 0 || HourlyRate <= 0)
            {
                ModelState.AddModelError("", "Please enter valid numbers for hours and rate.");
                return View();
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var claim = new LecturerClaim
            {
                UserId = userId,
                HoursWorked = HoursWorked,
                HourlyRate = HourlyRate,
                TotalAmount = HoursWorked * HourlyRate,
                Notes = Notes,
                Status = ClaimStatus.Pending,
                SubmittedAt = DateTime.UtcNow
            };

            _context.LecturerClaims.Add(claim);
            await _context.SaveChangesAsync();

            if (SupportingDocument != null && SupportingDocument.Length > 0)
            {
                var allowedTypes = new[] {
                    "application/pdf",
                    "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
                    "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"
                };

                if (!allowedTypes.Contains(SupportingDocument.ContentType))
                {
                    ModelState.AddModelError("", "Unsupported file type. Only PDF, DOCX, or XLSX allowed.");
                    return View();
                }

                var uploadPath = Path.Combine(_env.WebRootPath, "uploads", claim.ClaimId.ToString());
                Directory.CreateDirectory(uploadPath);

                var filePath = Path.Combine(uploadPath, Path.GetFileName(SupportingDocument.FileName));
                using (var stream = new FileStream(filePath, FileMode.Create))
                    await SupportingDocument.CopyToAsync(stream);

                var document = new ClaimDocument
                {
                    ClaimId = claim.ClaimId,
                    FileName = SupportingDocument.FileName,
                    FilePath = filePath,
                    ContentType = SupportingDocument.ContentType,
                    FileSize = SupportingDocument.Length
                };

                _context.ClaimDocuments.Add(document);
                await _context.SaveChangesAsync();
            }

            TempData["Success"] = "Claim submitted successfully!";
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Details(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var claim = await _context.LecturerClaims
                .Include(c => c.Documents)
                .FirstOrDefaultAsync(c => c.ClaimId == id && c.UserId == userId);

            if (claim == null) return NotFound();

            return View(claim);
        }

        public async Task<IActionResult> Download(int id)
        {
            var document = await _context.ClaimDocuments.FindAsync(id);
            if (document == null || !System.IO.File.Exists(document.FilePath))
                return NotFound();

            var fileBytes = await System.IO.File.ReadAllBytesAsync(document.FilePath);
            return File(fileBytes, document.ContentType, document.FileName);
        }
    }
}
