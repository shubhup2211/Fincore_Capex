using Fincore.Application.DTO.Login;
using Fincore.Application.Interfaces.ICapex;
using Fincore.Infrastructure.CommonHelper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Fincore.API.Controllers.Capex
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class LoginController : ControllerBase
    {
        ILoginService loginService;
        LoginUser loginUser;
        public LoginController(ILoginService loginService, LoginUser loginUser) 
        {
            this.loginService = loginService;
            this.loginUser = loginUser;
        }

        [HttpPost]
        public async Task<IActionResult> login(string email, string password)
        {
            var res = await loginService.Login(email, password);
            return Ok(res);
        }

        [HttpGet("getLoginDetails")]
        public IActionResult TestMySession()
        {
            var savedUserId = HttpContext.Session.GetInt32("UserId");
            var deptId = loginUser.getDepartmentId();
            var UserName = loginUser.getUserName();
            var Role = loginUser.getRoleId();
            var RoleName = loginUser.getRoleName();

            if (savedUserId == null)
            {
                return Ok(new
                {
                    status = "Failure",
                    message = "Cookie exists, but 'UserId' data was never set or expired."
                });
            }

            return Ok(new
            {
                status = "Success",
                message = "Session is actively storing data!",
                UserId = savedUserId,
                DeptId = deptId,
                UserName = UserName,
                Role = Role,
                RoleName = RoleName

            });


        }

        [HttpPost("Logout")]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();

            return Ok(new
            {
                success = true,
                message = "Logout Successful"
            });
        }
    }
}
