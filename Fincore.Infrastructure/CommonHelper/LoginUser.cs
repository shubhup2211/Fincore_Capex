using Fincore.Infrastructure.Data;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fincore.Infrastructure.CommonHelper
{
    public class LoginUser
    {
        IHttpContextAccessor context;
        AppDbContext db;
        public LoginUser(IHttpContextAccessor context, AppDbContext db)
        {
            this.context = context;
            this.db = db;
        }
        public int getUserId()
        {
            return context.HttpContext?.Session.GetInt32("UserId") ?? 0;
        }

        public int getRoleId()
        {
            return context.HttpContext?.Session.GetInt32("RoleId") ?? 0;
        }
        public string getRoleName()
        {
             var roleId = getRoleId();
            if (roleId == 0) return "Not Available";
            return db.Roles
                .Where(x => x.RoleId == roleId)
                .Select(x => x.RoleName)
                .FirstOrDefault() ?? "Not Available";
        }
        public int getDepartmentId()
        {
            var userId = getUserId();
            if (userId == 0) return 0;

            return  db.Employees
                .Where(x=> x.UserId == userId)
                .Select(x=> x.DepartmentId)
                .FirstOrDefault();
        }

        public string getUserName()
        {
            return context.HttpContext?.Session.GetString("Username") ?? "Not Available";
        }
    }
}
