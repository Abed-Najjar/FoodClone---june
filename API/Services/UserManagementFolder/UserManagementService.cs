using API.AppResponse;
using API.DTOs;
using API.Models;
using API.Services.Argon;
using API.Services.TokenServiceFolder;
using API.UoW;
using Microsoft.AspNetCore.Mvc;

public class UserManagementService : IUserManagementService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IArgonHashing _argonHashing;
    private readonly ITokenService _tokenService;

    public UserManagementService(IUnitOfWork unitOfWork, IArgonHashing argonHashing, ITokenService tokenService)
    {
        _unitOfWork = unitOfWork;
        _argonHashing = argonHashing;
        _tokenService = tokenService;
    }    public async Task<AppResponse<UserDto>> CreateUser([FromBody] UserInputDto dto)
    {
        try
            {
                var existingUser = await _unitOfWork.UserRepository.GetUserByEmailAsync(dto.Email);

                if (existingUser != null)
                {
                    return new AppResponse<UserDto>(null, "User already exists", 404, false);
                }

                var user = new User
                {
                    FirstName = dto.FirstName,
                    LastName = dto.LastName,
                    Email = dto.Email,
                    Address = dto.Address,
                    PasswordHash = await _argonHashing.HashPasswordAsync(dto.Password),
                    CreatedAt = DateTime.UtcNow
                };

                await _unitOfWork.UserRepository.CreateUserAsync(user);
                await _unitOfWork.CompleteAsync();


                var userDto = new UserDto
                {
                    Id = user.Id,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Email = user.Email,
                    Rolename = user.Role.ToString(),
                    Token = _tokenService.CreateToken(user)
                };

                return new AppResponse<UserDto>(userDto, "User created successfully", 201, true);
            }
            catch (Exception ex)
            {
                return new AppResponse<UserDto>(null, ex.Message, 500, false);
            }
    }    public async Task<AppResponse<bool>> DeleteUser(int id)
    {
        try
            {
                var user = await _unitOfWork.UserRepository.FindAsync(id);

                if (user == null)
                {
                    return new AppResponse<bool>(false, "User not found", 404, false);
                }

                await _unitOfWork.UserRepository.DeleteUserAsync(id);
                await _unitOfWork.CompleteAsync();

                return new AppResponse<bool>(true, "User deleted successfully", 200, true);

            }
            catch (Exception ex)
            {
                return new AppResponse<bool>(false, ex.Message, 500, false);
            }
    }    public async Task<AppResponse<List<UserDto>>> GetAllUsers()
    {
        try
            {
                var users = await _unitOfWork.UserRepository.GetAllUsersExceptRoleAsync(API.Enums.Roles.Admin);

                if (users == null || !users.Any())
                {
                    return new AppResponse<List<UserDto>>(null, "No users found", 404, false);
                }

                var userDtos = users.Select(user => new UserDto
                {
                    Id = user.Id,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Email = user.Email,
                    Rolename = user.Role.ToString(),
                    Token = _tokenService.CreateToken(user)
                }).ToList();

                return new AppResponse<List<UserDto>>(userDtos, "Users retrieved successfully", 200, true);
            }
            catch (Exception ex)
            {
                return new AppResponse<List<UserDto>>(null, ex.Message, 500, false);
            }
    }
    public async Task<AppResponse<UserDto>> GetUser(int id)
    {
        try
            {
                var user = await _unitOfWork.UserRepository.FindAsync(id);

                if (user == null)
                {
                    return new AppResponse<UserDto>(null, "User not found", 404, false);
                }

                var userDto = new UserDto
                {
                    Id = user.Id,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Email = user.Email,
                    Rolename = user.Role.ToString(),
                    Token = _tokenService.CreateToken(user)
                };

                return new AppResponse<UserDto>(userDto, "User retrieved successfully", 200, true);
            }
            catch (Exception ex)
            {
                return new AppResponse<UserDto>(null, ex.Message, 500, false);
            }
    }
    public async Task<AppResponse<UserDto>> UpdateUser(int id, [FromBody] UserInputDto dto)
    {
        try
            {
                var user = await _unitOfWork.UserRepository.FindAsync(id);

                if (user == null)
                {
                    return new AppResponse<UserDto>(null, "User not found", 404, false);
                }

                user.FirstName = dto.FirstName;
                user.LastName = dto.LastName;
                user.Email = dto.Email;
                user.Address = dto.Address;
                user.Role = dto.Role != null ? (API.Enums.Roles)Enum.Parse(typeof(API.Enums.Roles), dto.Role) : user.Role;
                user.PasswordHash = await _argonHashing.HashPasswordAsync(dto.Password);

                await _unitOfWork.UserRepository.UpdateUserAsync(user);
                await _unitOfWork.CompleteAsync();

                var userDto = new UserDto
                {
                    Id = user.Id,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Email = user.Email,
                    Address = user.Address,
                    Rolename = user.Role.ToString(),
                    Token = _tokenService.CreateToken(user),
                    Createdat = user.CreatedAt
                    
                };

                return new AppResponse<UserDto>(userDto, "User updated successfully", 200, true);
            }
            catch (Exception ex)
            {
                return new AppResponse<UserDto>(null, ex.Message, 500, false);
            }
    }

}
