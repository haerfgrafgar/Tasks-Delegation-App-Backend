using CordApp.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CordApp.Controllers
{
    [Microsoft.AspNetCore.Mvc.Route("api/user")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IWorkRepository _workRepo;
        private readonly IFractionsRepository _fractionsRepo;
        private readonly IUserRepository _userRepo;

        public UserController(IWorkRepository workRepo, IFractionsRepository fractionsRepo, IUserRepository userRepo)
        {
            _workRepo = workRepo;
            _fractionsRepo = fractionsRepo;
            _userRepo = userRepo;
        }

        [HttpGet]
        [Authorize(Roles = "Projetista")]
        public async Task<IActionResult> GetAllUsersGeneralSituation()
        {
            var response = await _userRepo.GetAllUsersGeneralSituation();
            return Ok(response);
        }
    }
}
