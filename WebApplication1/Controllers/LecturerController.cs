using Microsoft.AspNetCore.Mvc;

namespace WebApplication1.Controllers
{ 
    public class LecturerController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
