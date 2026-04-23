
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using BCrypt.Net;

[ApiController]
[Route("api/[controller]")]
public class AccountController : ControllerBase
{
    private readonly MainDBContext _context;
    private readonly ILogger<AccountController> _logger;
    private readonly IConfiguration _configuration;
    private readonly UserManager<ApiUser> _userManager;
    private readonly SignInManager<ApiUser> _signInManager;

    public AccountController(ILogger<AccountController> logger, IConfiguration configuration, UserManager<ApiUser> userManager, SignInManager<ApiUser> signInManager, MainDBContext context) {
        _context = context;
        _logger = logger;
        _configuration = configuration;
        _userManager = userManager;
        _signInManager = signInManager;
    }


    private async Task<string> GenerateToken(ApiUser user)
    {
        var key = _configuration["JWT:SigningKey"];
        var issuer = _configuration["JWT:Issuer"];
        var audience = _configuration["JWT:Audience"];

        var roles = await _userManager.GetRolesAsync(user);

        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id),
            new Claim(ClaimTypes.Name, user.Email),
            new Claim(JwtRegisteredClaimNames.Email, user.Email),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        foreach (var role in roles)
        {
            claims.Add(new Claim(ClaimTypes.Role, role));
        }

        var credentials = new SigningCredentials(
            new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key)),
            SecurityAlgorithms.HmacSha256
        );

        var token = new JwtSecurityToken(
            issuer: issuer,
            audience: audience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(30),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
    
    [HttpPost]
    [Route("Register")]
    [AllowAnonymous]
    public async Task<ActionResult> Register(RegisterDTO input)
    {
        try {
            if (ModelState.IsValid) {
                var newUser = new ApiUser();

                newUser.UserName = input.Email;
                newUser.Email = input.Email;
                newUser.FullName = input.FullName;
                
                var result = await _userManager.CreateAsync(newUser, input.Password);

                if (result.Succeeded) {
                    _logger.LogInformation("User {userName} ({email}) has been created.", newUser.UserName, newUser.Email);
                    return StatusCode(201, $"User '{newUser.UserName}' has been created.");
                }
                else {
                    throw new Exception(string.Format("Error: {0}", string.Join(" ", result.Errors.Select(e => e.Description))));
                }
            } else {
                var details = new ValidationProblemDetails(ModelState);
                details.Type = "https://tools.ietf.org/html/rfc7231#section-6.5.1";
                details.Status = StatusCodes.Status400BadRequest;
                return new BadRequestObjectResult(details);
            }
        }
        catch (Exception e)
        {
            var exceptionDetails = new ProblemDetails();
            exceptionDetails.Detail = e.Message;
            exceptionDetails.Status =
            StatusCodes.Status500InternalServerError;
            exceptionDetails.Type =
            "https://tools.ietf.org/html/rfc7231#section-6.6.1";
            return StatusCode(
            StatusCodes.Status500InternalServerError,
            exceptionDetails);
        }
    }

    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<ActionResult> Login(LoginDTO input)
    {
        var user = await _userManager.FindByEmailAsync(input.Email);

        if (user == null)
            return Unauthorized("user not found");

        var result = await _signInManager.CheckPasswordSignInAsync(user, input.Password, false);

        if (!result.Succeeded)
            return Unauthorized();

        var token = await GenerateToken(user);

        return Ok(new
        {
            token
        });
    }
}

