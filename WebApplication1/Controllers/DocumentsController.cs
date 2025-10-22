using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using WebApplication1.Data;
using WebApplication1.Models;

public class DocumentsController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly IWebHostEnvironment _env;

    public DocumentsController(ApplicationDbContext context, IWebHostEnvironment env)
    {
        _context = context;
        _env = env;
    }

    // GET: Upload form
    [HttpGet]
    public IActionResult Upload()
    {
        return View();
    }

    // POST: Upload file
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Upload(IFormFile file, int claimId)
    {
        // Validate file
        if (file == null || file.Length == 0)
        {
            ModelState.AddModelError("", "Please select a file.");
            return View();
        }

        //Check that claim exists
        var claim = await _context.LecturerClaims.FindAsync(claimId);
        if (claim == null)
        {
            ModelState.AddModelError("", "The specified claim does not exist.");
            return View();
        }

        //Create uploads folder if it doesn't exist
        var uploadPath = Path.Combine(_env.WebRootPath, "uploads");
        if (!Directory.Exists(uploadPath))
            Directory.CreateDirectory(uploadPath);

        //  Save file with unique name
        var fileName = Path.GetFileName(file.FileName);
        var uniqueFileName = $"{Guid.NewGuid()}_{fileName}";
        var filePath = Path.Combine(uploadPath, uniqueFileName);

        using (var stream = new FileStream(filePath, FileMode.Create))
        {
            await file.CopyToAsync(stream);
        }

        // Save document info to database
        var document = new ClaimDocument
        {
            ClaimId = claimId,
            LecturerClaim = claim,   // attach navigation property
            FileName = fileName,
            FilePath = "/uploads/" + uniqueFileName, // relative path for download
            ContentType = file.ContentType,
            FileSize = file.Length,
            UploadedAt = DateTime.UtcNow
        };

        _context.ClaimDocuments.Add(document);
        await _context.SaveChangesAsync();

        // Redirect to claims page or documents index
        return RedirectToAction("Index", "Claims");
    }
}
