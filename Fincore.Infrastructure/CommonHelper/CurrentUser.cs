using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Fincore.Infrastructure.CommonHelper
{
    public class CurrentUser
    {
        public static int GetUserId(ClaimsPrincipal user)
        {
            var v = user?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrWhiteSpace(v))
                throw new UnauthorizedAccessException("UserId claim not found in token.");
            return int.Parse(v);
        }

        public static int GetRoleId(ClaimsPrincipal user)
        {
            var v = user?.FindFirst("RoleId")?.Value;
            if (string.IsNullOrWhiteSpace(v))
                throw new UnauthorizedAccessException("RoleId claim not found in token.");
            return int.Parse(v);
        }

        public static int GetDepartmentId(ClaimsPrincipal user)
        {
            var v = user?.FindFirst("DepartmentId")?.Value;
            if (string.IsNullOrWhiteSpace(v))
                throw new UnauthorizedAccessException("DepartmentId claim not found in token.");
            return int.Parse(v);
        }

        public static string GetRoleName(ClaimsPrincipal user)
            => user?.FindFirst(ClaimTypes.Role)?.Value;

        public static string GetFullName(ClaimsPrincipal user)
            => user?.FindFirst(ClaimTypes.Name)?.Value;
    }
}
