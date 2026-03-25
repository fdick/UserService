using Microsoft.AspNetCore.Mvc;
using UserService.Core.Abstractions;
using UserService.Core.Models;

namespace UserService.API.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IKeycloakService _keycloakService;

        public UserController(IKeycloakService keycloakService)
        {
            this._keycloakService = keycloakService;
        }

        [HttpGet("{id:guid}")]
        public async Task<ActionResult<List<User>>> GetUser([FromRoute] Guid id)
        {
            var user = await _keycloakService.GetUserByIdAsync(id);

            var r = new UserResponse(Guid.Parse(user.Id), user.Username, user.Email, user.FirstName, user.LastName);

            return Ok(r);
        }

        [HttpGet("GetAll")]
        public async Task<ActionResult<List<User>>> GetAllUsers()
        {
            var users = await _keycloakService.GetAllUsersAsync();

            var usersResponse = users.Select(x=> new UserResponse(Guid.Parse(x.Id), x.Username, x.Email, x.FirstName, x.LastName));

            return Ok(usersResponse);
        }
    }

    public record UserResponse(Guid Id, string Username, string Email, string FirstName, string LastName);
}
