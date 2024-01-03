using Component.Data.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Component.Application.System.Roles.RoleVerify
{
    public class RoleVerify
    {
        private readonly IConfiguration _configuration;
        private readonly UserManager<AppUser> _userManager;
        public RoleVerify(IConfiguration configuration, UserManager<AppUser> userManager)
        {
            _configuration = configuration;
            _userManager = userManager;
        }
        public class RolesRequirement : IAuthorizationRequirement
        {
            public string RoleName { get; }

            public RolesRequirement(string roleName)
            {
                RoleName = roleName;
            }
        }

        public class RoleHandler : AuthorizationHandler<RolesRequirement>
        {
            protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, RolesRequirement requirement)
            {
                string roles = context.User.Claims.FirstOrDefault(x => x.Type == ClaimsIdentity.DefaultRoleClaimType).Value;
                if (!string.IsNullOrEmpty(roles))
                {
                    if (roles.Contains(requirement.RoleName))
                    {
                        context.Succeed(requirement);
                    }
                }

                return Task.CompletedTask;
            }
        }
    }
}
