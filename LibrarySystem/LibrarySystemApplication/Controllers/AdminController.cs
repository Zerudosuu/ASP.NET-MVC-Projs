using Microsoft.AspNetCore.Mvc;

namespace LibrarySystemApplication.Controllers;

public class AdminController : Controller
{
    // GET
    public IActionResult Dashboard()
    {
        return View();
    }
}