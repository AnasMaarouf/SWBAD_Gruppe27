
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
[Authorize]
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

    private string GenerateToken(string username) {
        var claims = new Claim[] {
            new Claim(ClaimTypes.Name, username),
            new Claim(JwtRegisteredClaimNames.Nbf, new DateTimeOffset(DateTime.Now).ToUnixTimeSeconds().ToString()),
            new Claim(JwtRegisteredClaimNames.Exp, new DateTimeOffset(DateTime.Now.AddDays(1)).ToUnixTimeSeconds().ToString())
        };
        
        var token = new JwtSecurityToken( new JwtHeader(new SigningCredentials( new SymmetricSecurityKey(Encoding.UTF8.GetBytes("the secret that needs to be at least 16 characeters long for HmacSha256")), SecurityAlgorithms.HmacSha256)), new JwtPayload(claims));
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

    [HttpPost]
    [Route("Login")] 
    [AllowAnonymous]
    public async Task<ActionResult> Login(LoginDTO input)
    {
        var user = await _userManager.FindByEmailAsync(input.Email);
        if (user == null) {
            ModelState.AddModelError(string.Empty, "Invalid login");
            return BadRequest(ModelState);
        }
        var passwordSignInResult = await _signInManager.CheckPasswordSignInAsync(user, input.Password, false);
        
        if (passwordSignInResult.Succeeded)
            return new ObjectResult(GenerateToken(input.Email));
        
        return BadRequest("Invalid login");
    }
}
