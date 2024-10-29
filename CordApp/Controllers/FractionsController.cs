using CordApp.Data;
using CordApp.Interface;
using CordApp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace CordApp.Controllers
{
    [Microsoft.AspNetCore.Mvc.Route("api/fractions")]
    [ApiController]
    public class FractionsController : ControllerBase
    {
        private readonly IFractionsRepository _fractionsRepo;
        private readonly ApplicationDBContext _dbContext;
        public FractionsController(IFractionsRepository fractionsRepo, ApplicationDBContext dbContext)
        {
            _fractionsRepo = fractionsRepo;
            _dbContext = dbContext;
        }

        [HttpGet("username/{username}")]
        [Authorize]
        public async Task<IActionResult> GetCurrentFractionByUsername([FromRoute] string username)
        {
            var fraction = await _fractionsRepo.GetCurrentFractionByUsername(username);

            return Ok(fraction);
        }

        [HttpGet("history/{username}")]
        [Authorize]
        public async Task<IActionResult> GetUserDayHistory([FromRoute] string username, [FromQuery] DateTime day)
        {
            var fractions = await _fractionsRepo.GetDayHistoryByUsername(username, day);

            if (fractions == null)
                return NotFound("Username not found.");

            return Ok(fractions);
        }

        [HttpPost("start/{workId:int}")]
        [Authorize]
        public async Task<IActionResult> StartFraction([FromRoute] int workId)
        {
            var userId = User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            var fractionModel = await _fractionsRepo.Start(workId, userId);

            return Ok(fractionModel);
        }

        [HttpPut("stop")]
        [Authorize]
        public async Task<IActionResult> StopCurrentFraction()
        {
            var userId = User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var currentUser = await _dbContext.Users.FindAsync(userId);

            var fractionModel = await _fractionsRepo.Stop(currentUser.FractionInProgressId, userId);

            return Ok(fractionModel);
        }
    }
}
