using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LibrarySystemServer.Controllers
{
    [Authorize(Roles = "Member")]
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        //TODO: Setup member or user controller
    }
}
