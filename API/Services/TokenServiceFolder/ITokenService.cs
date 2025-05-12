using API.Models;

namespace API.Services.TokenServiceFolder
{
    public interface ITokenService
    {
         string CreateToken(User user);
    }
}