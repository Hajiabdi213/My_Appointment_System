using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace Controller;

[Route("[controller]")]
public class AuthController : ControllerBase
{
    private readonly AppointmentsDbContext _context;

    public AuthController(AppointmentsDbContext context)
    {
        _context = context;

    }

    //POST //Auth/Login
    [HttpPost("login")]
    public async Task<IActionResult> Login(string email)
    {
        var user = await _context.Users.SingleOrDefaultAsync(u=>u.Email==email);
        if(user is null)
        {
            return BadRequest("Invalid login attempt");
        }

        //TODO: validate user information
        var keyInput = "random_text_with_at_least_32_chars";
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(keyInput));
        var now = DateTime.Now;
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, user.Id.ToString()),

            new("Full Name",  user.FullName),
            new("gender",  user.Gender),
            new("email",  user.Email),
            
        };
        var token = new JwtSecurityToken("MyAPI", "MyFrontendApp", claims, now, now.AddHours(1), credentials);
        var handler = new JwtSecurityTokenHandler();
        var jwt = handler.WriteToken(token);
        var result = new
        {
            token = jwt
        };
        return Ok(result);
    }
}