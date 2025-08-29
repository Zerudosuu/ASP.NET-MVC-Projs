using Microsoft.AspNetCore.Mvc;

namespace LibrarySystemApplication.Controllers
{
    public class LibraryController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
