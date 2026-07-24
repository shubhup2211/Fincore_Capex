using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Fincore.Domain.Models;

namespace Fincore.Application.Interfaces
{
    public interface IJwtTokenHelper
    {
        string GenerateAccessToken(User user);
        string GenerateRefreshToken();
    }
}
