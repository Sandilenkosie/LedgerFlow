using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly ISender _sender;
        public AuthController(ISender sender) { _sender = sender; }

        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterCommand cmd)
        {
            var result = await _sender.Send(cmd);
            return Ok(result);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginCommand cmd)
        {
            var result = await _sender.Send(cmd);
            return Ok(result);
        }
    }
}
