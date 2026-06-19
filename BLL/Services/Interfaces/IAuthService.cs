using BLL.Common;
using BLL.DTOs.AuthDTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Services.Interfaces
{
    public interface IAuthService
    {
        public Task<ApiResponse<object>> RegisterUser(RegisterDTO registerDTO);

        public Task<ApiResponse<object>> LoginUser(LoginDTO loginDTO);



    }
}
