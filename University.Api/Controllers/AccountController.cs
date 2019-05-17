using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using University.Api.ViewModels.Accounts;
using University.Data.Models;
using University.Service.Encriptions;
using University.Service.Users;
using static University.Core.Utilities.AppConstants;

namespace University.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        #region Private Member

        private readonly IMapper _mapper;
        private readonly IUserService _userService;
        private readonly IConfiguration _configuration;

        #endregion

        #region Ctor

        public AccountController(IMapper mapper,
            IConfiguration configuration,
            IUserService userService
            )
        {
            _mapper = mapper;
            _userService = userService;
            _configuration = configuration;
        }
        #endregion

        #region Login

        [Route("login")]
        [HttpPost]
        public async Task<ActionResult> Login([FromBody] LoginViewModel model)
        {
            //get user info by email
            var user = await _userService.GetDetailsByEmail(model.Email);

            if (user == null)
                return Ok(new { IsSuccess = false, Message = "Incorrect email or password.!" });

            //check valid user
            if (ValidateLoggedInUser(model, user))
            {
                var message = "Incorrect email or password, Please try again.";
                return Ok(new { IsSuccess = false, Message = message });
            }

            //creating custom claims
            var claims = new[] {
                    new Claim("Id", user.Id.ToString()),
                    new Claim("CellPhone", user.CellPhone),
                    new Claim("Email", user.Email),
                    new Claim("FirstName", user.FirstName),
                    new Claim("LastName", user.LastName),
                    new Claim("RoleId", user.RoleId.ToString()),
                    new Claim("RoleName", user.UserRole.Name)
                };

            //get signing key
            var signinKey = new SymmetricSecurityKey(
              Encoding.UTF8.GetBytes(_configuration["Jwt:SigningKey"]));

            //get token expire
            int expiryInMinutes = Convert.ToInt32(_configuration["Jwt:ExpiryInMinutes"]);

            //generating a new jwt token with additional information
            var token = new JwtSecurityToken(
              claims: claims,
              issuer: _configuration["Jwt:Site"],
              audience: _configuration["Jwt:Site"],
              expires: DateTime.UtcNow.AddMinutes(expiryInMinutes),
              signingCredentials: new SigningCredentials(signinKey, SecurityAlgorithms.HmacSha256)
            );

            return Ok(
              new
              {
                  token = new JwtSecurityTokenHandler().WriteToken(token),
                  expiration = token.ValidTo
              });
        }        

        #endregion
        
        #region Register

        [Route("register")]
        [HttpPost]
        public async Task<ActionResult> RegisterNewUser([FromBody] UserRegistrationViewModel model)
        {
            var existingUser = await _userService.GetDetailsByEmail(model.Email);

            if(existingUser!=null)
                return Ok(new { IsSuccess = false, Message = $"Email already exist. Please try another!" });

            var newUser = _mapper.Map<User>(model);
            newUser.RoleId = UserRoleConstants.Student;
            newUser.Status = StatusConstants.Active;

            var isRegistered = await _userService.Add(newUser);

            if (!isRegistered)            
                return Ok(new { IsSuccess = false, Message = "There was an error while trying to register!" });
            
            return Ok(new { IsSuccess = true, Message = "You have registered successfully" });
        }

        #endregion

        #region Private Methods

        private bool ValidateLoggedInUser(LoginViewModel model, User user)
        {
            return user.Status != StatusConstants.Active 
                && user.PasswordHash != EncryptionService.HashPassword(model.Password, user.PasswordSalt);
        }

        #endregion
    }
}
