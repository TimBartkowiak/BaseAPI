using System.Collections.Generic;
using BaseAPI.Entities;
using BaseAPI.Exceptions;
using BaseAPI.Models;
using BaseAPI.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BaseAPI.Controllers
{
    [Authorize]
    [Route("[controller]")]
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

        [HttpGet("")]
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
        [HttpPost("login")]
        public IActionResult Login()
        {
            try
            {
                string token = _authenticationService.login(Request.Headers["Authorization"]);
                return token != null ? Ok(token) : Unauthorized();
            }
            catch (UnAuthorizedException ae)
            {
                return new ForbiddenResult(ae.Message);
            }
        }

        [AllowAnonymous]
        [HttpPost("register")]
        public IActionResult Register()
        {
            try
            {
                string token = _authenticationService.register(Request.Headers["Authorization"]);
                return token != null ? Ok(token) : Unauthorized();
            }
            catch (UnAuthorizedException ae)
            {
                return new ForbiddenResult(ae.Message);
            }
        }
    }
}