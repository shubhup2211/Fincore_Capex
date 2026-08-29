using AutoMapper;
using AutoMapper.QueryableExtensions;
using Fincore.Application.DTO;
using Fincore.Application.DTO.Capex;
using Fincore.Application.DTO.Login;
using Fincore.Application.Interfaces.ICapex;
using Fincore.Infrastructure.CommonHelper;
using Fincore.Infrastructure.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fincore.Infrastructure.Services.Capex
{
    public class LoginService : ILoginService
    {
        AppDbContext db;
        IMapper map;
        IHttpContextAccessor  httpContext;
        public LoginService(AppDbContext db, IMapper map, IHttpContextAccessor httpContext) { 
            this.db  = db;
            this.map = map;
            this.httpContext = httpContext;
        }

        public async Task<ApiResponse<UserDTOGet>> Login(string email, string password)
        {
                 var user = await db.Users.FirstOrDefaultAsync(x => x.Email == email);

            if (user == null)
            {
                return ApiResponseHelper.Failure<UserDTOGet>(
                    "User Not Found", "NOT_FOUND", "User is not registered");
            }
            else
            {
                if(user.Email == email && user.PasswordHash == password)
                {                    
                    var userData = await db.Users
                        .Where(x => x.UserId == user.UserId)
                        .ProjectTo<UserDTOGet>(map.ConfigurationProvider)
                        .FirstOrDefaultAsync();

                    httpContext.HttpContext?.Session.SetInt32("UserId", user.UserId); 
                    httpContext.HttpContext?.Session.SetInt32("RoleId", user.RoleId);
                    httpContext.HttpContext?.Session.SetString("Username", user.FullName);

                    return ApiResponseHelper.SuccessRes<UserDTOGet>(
                        userData, "Login Successfull");
                }
                else
                {
                    return ApiResponseHelper.Failure<UserDTOGet>(
                        "Invalid Credentials", "Unauthorize", "Try again with Correct Credentials");
                }
            }

            

        }
    }
}
