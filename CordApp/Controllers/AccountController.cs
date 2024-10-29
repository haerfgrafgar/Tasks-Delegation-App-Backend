using CordApp.Dtos.Account;
using CordApp.Interface;
using CordApp.Models;
using CordApp.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CordApp.Controllers
{
    [Route("api/account")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ITokenService _tokenService;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly IFractionsRepository _fractionsRepo;
        public AccountController(UserManager<AppUser> userManager, RoleManager<IdentityRole> roleManager, IFractionsRepository fractionRepo ,ITokenService tokenService, SignInManager<AppUser> signInManager)
        {
            _userManager = userManager;
            _tokenService = tokenService;
            _signInManager = signInManager;
            _fractionsRepo = fractionRepo;
            _roleManager = roleManager;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDto loginDto)
        {
            if(!ModelState.IsValid)
                return BadRequest(ModelState);

            var user = await _userManager.Users.FirstOrDefaultAsync(x => x.UserName == loginDto.Username.ToLower());

            if (user == null)
                return Unauthorized("Credentials not found.");

            var result = await _signInManager.CheckPasswordSignInAsync(user, loginDto.Password, false);

            if (!result.Succeeded)
                return Unauthorized("Credentials not found.");

            return Ok(
                new NewUserDto
                {
                    UserId = user.Id,
                    UserName = user.UserName,
                    Token = _tokenService.CreateToken(user)
                });
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto registerDto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var appUser = new AppUser
                {
                    UserName = registerDto.Username,
                    Email = registerDto.Email,
                    Position = registerDto.Position,
                    CordId = "0", //Keep this here for now
                    TodayIdleWorkId = 1,
                    WorkInProgressId = 1,
                };
                var createdUser = await _userManager.CreateAsync(appUser, registerDto.Password);
                

                if (createdUser.Succeeded)
                {
                    var roleResult = await _userManager.AddToRoleAsync(appUser, "Desenhista");

                    if (roleResult.Succeeded)
                    {
                        await _fractionsRepo.CreateFirstUserFraction(appUser.Id);
                        return Ok(
                            new NewUserDto
                            {
                                UserId = appUser.Id,
                                UserName = appUser.UserName,
                                Token = _tokenService.CreateToken(appUser)
                            }
                            );
                    }
                    else
                    {
                        return StatusCode(500, roleResult.Errors);
                    }
                }
                else
                {
                    return StatusCode(500, createdUser.Errors);
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex);
            }
        }

        [HttpPut("changeRole")]
        [Authorize(Roles = "Projetista")]
        public async Task<IActionResult> ChangeRole([FromBody] ChangeRoleDto changeRoleDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var user = await _userManager.FindByNameAsync(changeRoleDto.username);

            if (user == null)
            {
                return NotFound("Username does not exist.");
            }

            var roleId = await _roleManager.Roles
                .Where(r => r.Name == changeRoleDto.role)
                .Select(r => r.Id)
                .FirstOrDefaultAsync();

            if (roleId == null)
            {
                return NotFound("Role name does not exist.");
            }

            var currentRoles = await _userManager.GetRolesAsync(user);
            await _userManager.RemoveFromRolesAsync(user, currentRoles);

            var result = await _userManager.AddToRoleAsync(user, changeRoleDto.role);
            if (result.Succeeded)
            {
                return Ok(result);
            }
            return StatusCode(500, "Could not add role.");
        }
    }
}
