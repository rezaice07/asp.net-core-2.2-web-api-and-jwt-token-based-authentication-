using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Security.Claims;
using University.Data.Models;

namespace University.Api.Infrastructures.Controller
{
    [Authorize]
    [ApiController]
    public class CoreController : ControllerBase
    {
        public User CurrentLoginUser
        {
            get
            {
                var userModel = new User();

                if (!User.Identity.IsAuthenticated)
                    return userModel;

                var claimsIdentity = (ClaimsIdentity)User.Identity;
                var userClaims = claimsIdentity.Claims;

                if (userClaims == null)
                    return new User();

                userModel.Id = Convert.ToInt32(claimsIdentity.Claims.FirstOrDefault(f => f.Type == "Id").Value);
                userModel.Email = claimsIdentity.Claims.FirstOrDefault(f => f.Type == "Email").Value;
                userModel.FirstName = claimsIdentity.Claims.FirstOrDefault(f => f.Type == "FirstName").Value;
                userModel.LastName = claimsIdentity.Claims.FirstOrDefault(f => f.Type == "LastName").Value;
                userModel.RoleId = Convert.ToInt32(claimsIdentity.Claims.FirstOrDefault(f => f.Type == "RoleId").Value);
                userModel.RoleName = claimsIdentity.Claims.FirstOrDefault(f => f.Type == "RoleName").Value;
                userModel.CellPhone = claimsIdentity.Claims.FirstOrDefault(f => f.Type == "CellPhone").Value;
                return userModel;
            }
        }
    }
}
