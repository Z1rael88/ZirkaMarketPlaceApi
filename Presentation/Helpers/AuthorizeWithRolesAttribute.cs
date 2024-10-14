using Domain.Enums;
using Microsoft.AspNetCore.Authorization;

namespace Presentation.Helpers;

public class AuthorizeWithRolesAttribute : AuthorizeAttribute
{
    public AuthorizeWithRolesAttribute(params Role[] roles)
    {
        Roles = RoleHelper.CombineRoles(roles);
    }
}