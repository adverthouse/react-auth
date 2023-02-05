using back_end.Services;
using back_end.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace back_end.Controllers
{
    [ApiVersion("1.0")]  
    [ApiVersion("2.0")]  

    [Authorize]
    [ApiController]
    [Route("api/v{version:apiVersion}/[controller]")]
    public class AuthController : Controller
    {
        private IMemberService _memberService;

        public AuthController(IMemberService memberService)
        {
            _memberService = memberService;
        }

        [AllowAnonymous]
        [HttpPost("authenticate")]
        public IActionResult Authenticate([FromBody] AuthenticateRequest model)
        {
            var response = _memberService.Authenticate(model, ipAddress());

            if (response == null)
                return BadRequest(new { message = "Member name or password is incorrect" });

            return Ok(response);
        }

        [AllowAnonymous ]
        [HttpPost("refresh-token")]
        public IActionResult RefreshToken([FromBody] TokenRequest model)
        { 
            var response = _memberService.RefreshJWTToken(model.Token, ipAddress());

            if (response == null)
                return Unauthorized(new { message = "Invalid token" });

            return Ok(response);
        } 

        [HttpPost("revoke-token")]
        public IActionResult RevokeToken([FromBody] TokenRequest model)
        { 

            if (string.IsNullOrEmpty(model.Token))
                return BadRequest(new { message = "Token is required" });

            var response = _memberService.RevokeToken(model.Token, ipAddress());

            if (!response)
                return NotFound(new { message = "Token not found" });

            return Ok(new { message = "Token revoked" });
        }

        [HttpGet()]
        public IActionResult GetAll()
        {
            var users = _memberService.GetAll();
            return Ok(users);
        }

        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            var user = _memberService.GetById(id);
            if (user == null) return NotFound();

            return Ok(user);
        }

        [HttpGet("{id}/refresh-tokens")]
        public IActionResult GetRefreshTokens(int id)
        {
            var user = _memberService.GetById(id);
            if (user == null) return NotFound();

            return Ok(user.RefreshTokens);
        }

        private string ipAddress()
        {
            if (Request.Headers.ContainsKey("X-Forwarded-For"))
                return Request.Headers["X-Forwarded-For"];
            else
                return HttpContext.Connection.RemoteIpAddress.MapToIPv4().ToString();
        }
    }
}