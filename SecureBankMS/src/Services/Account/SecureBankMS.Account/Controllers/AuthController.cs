using BCrypt.Net;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SecureBankMS.Account.DAL;
using SecureBankMS.Account.Dtos;
using SecureBankMS.Account.Models;

namespace SecureBankMS.Account.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController(AccountDbContext _dbContext) : ControllerBase
    {
        [HttpPost("register")]
        public async Task<IActionResult> Register(UserRegisterDto dto)
        {
            if (await _dbContext.Users.AnyAsync(x => x.Username == dto.Username))
                return BadRequest("İstifadəçi adı artıq mövcuddur.");

            if (await _dbContext.Users.AnyAsync(x => x.Email == dto.Email))
                return BadRequest("E-poçt artıq mövcuddur.");

            var passwordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password + dto.Username);

            var user = new User
            {
                Username = dto.Username,
                Email = dto.Email,
                PasswordHash = passwordHash,
            };

            await _dbContext.Users.AddAsync(user);
            await _dbContext.SaveChangesAsync();

            return Ok("Qeydiyyat prosesi uğurla yekunlaşdı.");
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(UserLoginDto dto)
        {
            var user = await _dbContext.Users.SingleOrDefaultAsync(x => x.Username == dto.Username);
            if (user == null)
                return Unauthorized("Yanlış istifadəçi adı və ya parol.");

            bool verified = BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash);

            if (!verified)
                return Unauthorized("Yanlış istifadəçi adı və ya parol.");

            return Ok("Uğurlu giriş");
        }

    }
}
