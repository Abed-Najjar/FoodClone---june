using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Services.Argon
{
    public interface IArgonHashing
    {
        Task<string> HashPasswordAsync(string password);
        Task<bool> VerifyHashedPasswordAsync(string hashedPassword, string inputPassword);
    }
}