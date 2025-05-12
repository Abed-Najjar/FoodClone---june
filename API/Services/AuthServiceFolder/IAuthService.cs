using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.AppResponse;
using API.DTOs;

namespace API.Services.TokenServiceFolder.AuthServiceFolder
{
    public interface IAuthService
    {
        Task<AppResponse<UserDto>> Register(UserInputDto dto);
        Task<AppResponse<UserDto>> Login(UserLoginDto dto);
        Task<AppResponse<PasswordResetDto>> ForgotPassword(PasswordResetRequestDto dto);

    }
}