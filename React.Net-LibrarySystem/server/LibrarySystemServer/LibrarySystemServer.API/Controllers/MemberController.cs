using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LibrarySystemServer.Controllers
{
    [Authorize(Roles = "Member")]
    [ApiController]
    [Route("api/[controller]")]
    public class MemberController : ControllerBase
    {
      
    }
}
