using JwtToken.Data.Entities;
using JwtToken.DTOs.AccountDts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace JwtToken.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<AppUser> _userManager;

        public AccountController( RoleManager<IdentityRole> roleManager, UserManager<AppUser> userManager)
        {
            _roleManager = roleManager;
            _userManager = userManager;
        }
        [HttpGet("created")]
        public async Task<IActionResult> RoleCreated()
        {
            await _roleManager.CreateAsync(new IdentityRole("admin"));
            await _roleManager.CreateAsync(new IdentityRole("Superadmin"));
            await _roleManager.CreateAsync(new IdentityRole("member"));
            return Ok("Role created");
        }
        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterDto registerDto)
        {
            AppUser appUser = await _userManager.FindByNameAsync(registerDto.UserName);
            if (appUser != null) return StatusCode(409);

            AppUser user = new AppUser
            {
                FullName = registerDto.FullName,
                UserName = registerDto.UserName
            };
            var result = await _userManager.CreateAsync(user, registerDto.Password);
            if (!result.Succeeded)
            {
                return BadRequest(result.Errors);
            }
            var roleResult= await _userManager.AddToRoleAsync(user, "admin");
            if (!roleResult.Succeeded)
            {
                return BadRequest(result.Errors);
            }
            return StatusCode(201);
           
        }
        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDto loginDto)
        {
            AppUser appUser = await _userManager.FindByNameAsync(loginDto.UserName);
            if (appUser == null) return NotFound();

            if (!await _userManager.CheckPasswordAsync(appUser, loginDto.Password))
            {
                return Unauthorized();
            }
            string token = await GenerateToke(appUser);

            return Ok(new { token=token });
           
        }

       
        [Authorize(Roles ="member,admin")]
        [HttpGet("get")]
        public async Task<IActionResult> Get()
        {
            AppUser user=await _userManager.FindByNameAsync(User.Identity.Name);
            return Ok(new { user.Id, user.FullName, user.UserName });
        }

        private async Task<string> GenerateToke(AppUser appUser)
        {
            List<Claim> claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name,appUser.UserName),
                new Claim(ClaimTypes.NameIdentifier,appUser.Id),
                new Claim("FullName",appUser.FullName)
            };
            var roles = await _userManager.GetRolesAsync(appUser);
            claims.AddRange(roles.Select(x => new Claim(ClaimTypes.Role, x)));

            //foreach (var item in roles)
            //{
            //    claims.Add(new Claim(ClaimTypes.Role, item));
            //}

            string secretKey = "210970df-59d0-487d-b90c-f864e759ab17";

            SymmetricSecurityKey securityKey = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(secretKey));
            SigningCredentials creds = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            JwtSecurityToken jwtSecurityToken = new JwtSecurityToken(
                claims: claims,
                issuer: "https://localhost:44391",
                audience: "https://localhost:44391",
                signingCredentials: creds,
                expires: DateTime.Now.AddDays(1)
                );

            string token = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken);
            return token;
        }
    }

}
