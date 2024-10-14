using Domain.Enums;

namespace Presentation.Helpers
{
    public static class RoleHelper
    {
        public static string CombineRoles(params Role[] roles)
        {
            return string.Join(",", roles.Select(r => r.ToString()));
        }
    }
}