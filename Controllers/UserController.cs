// UserController.cs

using InventoryManagementSystem.Model;
using InventoryManagementSystem.Services;
using Microsoft.AspNetCore.Mvc;
using RestfulAPIEndpoints.Model;
using System.Threading.Tasks;

namespace InventoryManagementSystem.Controllers
{
    [ApiController]
    [Route("/user")]
    public class UserController : ControllerBase
    {
        private readonly IUser _userService;

        public UserController(IUser userService)
        {
            _userService = userService;
        }

        [HttpPost("signup")]
        public async Task<ActionResult> SignUp([FromBody] User user)
        {
             await _userService.CreateNewAccount(user);
            return Ok("User created successfully");
        }

        [HttpPost("login")]
        public async Task<ActionResult> Login([FromBody] Login loginData)
        {
            (bool isAuthenticated ,bool is_admin)= await _userService.LogIn(loginData.UserName, loginData.Password);
            if (isAuthenticated)
            {
                if (is_admin)
                {
                    return Ok("Admin");
                }
                else
                {
                    return Ok("User");

                }
            }
            return Unauthorized("Invalid credentials");
        }
    }
}
