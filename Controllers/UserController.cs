using System.Collections.Generic;
using BaseAPI.Entities;
using BaseAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BaseAPI.Controllers
{
    [Authorize]
    [Microsoft.AspNetCore.Components.Route("api/[controller]")]
    [ApiController]
    public class UserController : AbstractController<UserModel, UserEntity>
    {
        private readonly UserService _userService;
        private readonly AuthenticationService _authenticationService;

        public UserController(UserService userService, AuthenticationService authenticationService)
        {
            _userService = userService;
            _authenticationService = authenticationService;
        }

        protected override AbstractService<UserModel, UserEntity> getService()
        {
            return _userService;
        }

        [HttpGet]
        public List<UserModel> AllUsers()
        {
            return _userService.getAll();
        }

        [HttpGet("{id}")]
        public UserModel GetById(string id)
        {
            return getById(id);
        }

        [AllowAnonymous]
        [HttpPost("authentication")]
        public IActionResult Login()
        {
            string token = _authenticationService.login(Request.Headers["Authorization"]);
            return token != null ? Ok(token) : Unauthorized();
        }
    }
}