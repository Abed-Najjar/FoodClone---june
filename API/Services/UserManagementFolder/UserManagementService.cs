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
                    Address = dto.Address ?? new List<string>(),
                    Role = dto.Role != null ? (API.Enums.Roles)Enum.Parse(typeof(API.Enums.Roles), dto.Role) : API.Enums.Roles.User,
                    Status = dto.IsActive.HasValue && dto.IsActive.Value ? API.Enums.UserStatus.Active : API.Enums.UserStatus.Active,
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
                    Token = _tokenService.CreateToken(user),
                    Address = user.Address,
                    Createdat = user.CreatedAt,
                    PhoneNumber = user.PhoneNumber,
                    DateOfBirth = user.DateOfBirth,
                    Gender = user.Gender,
                    Bio = user.Bio,
                    LastLogin = user.LastLogin,
                    ProfileImageUrl = user.ImageUrl,
                    Status = user.Status.ToString(),
                    IsActive = user.Status == API.Enums.UserStatus.Active
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
            catch (InvalidOperationException ex)
            {
                // Handle specific case where user has related orders
                return new AppResponse<bool>(false, ex.Message, 400, false);
            }
            catch (Microsoft.EntityFrameworkCore.DbUpdateException ex)
            {
                // Handle Entity Framework specific exceptions
                var innerException = ex.InnerException?.Message ?? "No inner exception details";
                var fullMessage = $"Database update failed: {ex.Message}. Inner exception: {innerException}";
                
                Console.WriteLine($"DbUpdateException when deleting user {id}: {fullMessage}");
                
                if (ex.Message.Contains("foreign key constraint") || ex.Message.Contains("REFERENCE constraint") ||
                    innerException.Contains("foreign key constraint") || innerException.Contains("REFERENCE constraint"))
                {
                    return new AppResponse<bool>(false, "Cannot delete user because they have associated data in the system. Please ensure all related records are removed first.", 400, false);
                }
                
                return new AppResponse<bool>(false, $"Database error while deleting user: {innerException}", 500, false);
            }
            catch (Exception ex)
            {
                // Handle any other exceptions
                var innerException = ex.InnerException?.Message ?? "No inner exception details";
                var fullMessage = $"Unexpected error: {ex.Message}. Inner exception: {innerException}";
                
                Console.WriteLine($"General exception when deleting user {id}: {fullMessage}");
                
                return new AppResponse<bool>(false, fullMessage, 500, false);
            }
    }    public async Task<AppResponse<PagedResultDto<UserDto>>> GetAllUsers(PaginationDto? paginationDto = null)
    {
        try
            {
                var users = await _unitOfWork.UserRepository.GetAllUsersExceptRoleAsync(API.Enums.Roles.Admin);

                var userDtos = users.Select(user => new UserDto
                {
                    Id = user.Id,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Email = user.Email,
                    Rolename = user.Role.ToString(),
                    Token = _tokenService.CreateToken(user),
                    Address = user.Address,
                    Createdat = user.CreatedAt,
                    PhoneNumber = user.PhoneNumber,
                    DateOfBirth = user.DateOfBirth,
                    Gender = user.Gender,
                    Bio = user.Bio,
                    LastLogin = user.LastLogin,
                    ProfileImageUrl = user.ImageUrl,
                    Status = user.Status.ToString(),
                    IsActive = user.Status == API.Enums.UserStatus.Active
                }).ToList();

                // Always return paginated result
                var totalItems = userDtos.Count;
                var pageNumber = paginationDto?.PageNumber ?? 1;
                var pageSize = paginationDto?.PageSize ?? totalItems; // If no pagination, return all items
                
                var paginatedData = userDtos
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .ToList();

                var pagedResult = new PagedResultDto<UserDto>(paginatedData, totalItems, pageNumber, pageSize);
                return new AppResponse<PagedResultDto<UserDto>>(pagedResult, "Users retrieved successfully", 200, true);
            }
            catch (Exception ex)
            {
                return new AppResponse<PagedResultDto<UserDto>>(null, ex.Message, 500, false);
            }
    }

    public async Task<AppResponse<PagedResultDto<UserDto>>> GetAllUsersIncludingAdmins(PaginationDto? paginationDto = null)
    {
        try
        {
            var users = await _unitOfWork.UserRepository.GetAllAsync();

            var userDtos = users.Select(user => new UserDto
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                Rolename = user.Role.ToString(),
                Token = _tokenService.CreateToken(user),
                Address = user.Address,
                Createdat = user.CreatedAt,
                LastLogin = user.LastLogin,
                PhoneNumber = user.PhoneNumber,
                DateOfBirth = user.DateOfBirth,
                Gender = user.Gender,
                Bio = user.Bio,
                ProfileImageUrl = user.ImageUrl,
                Status = user.Status.ToString(),
                IsActive = user.Status == API.Enums.UserStatus.Active
            }).ToList();

            // Always return paginated result
            var totalItems = userDtos.Count;
            var pageNumber = paginationDto?.PageNumber ?? 1;
            var pageSize = paginationDto?.PageSize ?? totalItems; // If no pagination, return all items
            
            var paginatedData = userDtos
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            var pagedResult = new PagedResultDto<UserDto>(paginatedData, totalItems, pageNumber, pageSize);
            return new AppResponse<PagedResultDto<UserDto>>(pagedResult, "All users retrieved successfully", 200, true);
        }
        catch (Exception ex)
        {
            return new AppResponse<PagedResultDto<UserDto>>(null, ex.Message, 500, false);
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
                    Token = _tokenService.CreateToken(user),
                    Address = user.Address,
                    Createdat = user.CreatedAt,
                    PhoneNumber = user.PhoneNumber,
                    DateOfBirth = user.DateOfBirth,
                    Gender = user.Gender,
                    Bio = user.Bio,
                    LastLogin = user.LastLogin,
                    ProfileImageUrl = user.ImageUrl,
                    Status = user.Status.ToString(),
                    IsActive = user.Status == API.Enums.UserStatus.Active
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
                user.Status = dto.IsActive.HasValue && dto.IsActive.Value ? API.Enums.UserStatus.Active : API.Enums.UserStatus.Inactive;
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
                    Createdat = user.CreatedAt,
                    PhoneNumber = user.PhoneNumber,
                    DateOfBirth = user.DateOfBirth,
                    Gender = user.Gender,
                    Bio = user.Bio,
                    LastLogin = user.LastLogin,
                    ProfileImageUrl = user.ImageUrl,
                    Status = user.Status.ToString(),
                    IsActive = user.Status == API.Enums.UserStatus.Active
                };

                return new AppResponse<UserDto>(userDto, "User updated successfully", 200, true);
            }
            catch (Exception ex)
            {
                return new AppResponse<UserDto>(null, ex.Message, 500, false);
            }
    }

    public async Task<AppResponse<bool>> ToggleUserStatus(int id)
    {
        try
        {
            var user = await _unitOfWork.UserRepository.FindAsync(id);

            if (user == null)
            {
                return new AppResponse<bool>(false, "User not found", 404, false);
            }

            // Toggle the status
            user.Status = user.Status == API.Enums.UserStatus.Active 
                ? API.Enums.UserStatus.Inactive 
                : API.Enums.UserStatus.Active;

            await _unitOfWork.UserRepository.UpdateUserAsync(user);
            await _unitOfWork.CompleteAsync();

            var statusText = user.Status == API.Enums.UserStatus.Active ? "activated" : "deactivated";
            return new AppResponse<bool>(true, $"User {statusText} successfully", 200, true);
        }
        catch (Exception ex)
        {
            return new AppResponse<bool>(false, ex.Message, 500, false);
        }
    }

}
