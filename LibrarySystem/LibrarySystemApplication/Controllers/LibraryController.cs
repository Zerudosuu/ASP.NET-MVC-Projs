using LibrarySystemApplication.Data.Services.Interface;
using LibrarySystemApplication.Hubs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;

namespace LibrarySystemApplication.Controllers
{
    public class LibraryController : Controller
    {

        private readonly ILibraryServices _libraryServices;

        public LibraryController(ILibraryServices libraryServices)
        {
            _libraryServices = libraryServices;
        }
        
        public IActionResult Index()
        {
            return View();
        }
    }
}
