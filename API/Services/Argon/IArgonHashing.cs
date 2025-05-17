namespace API.Services.Argon
{
    public interface IArgonHashing
    {
        Task<string> HashPasswordAsync(string password);
        Task<bool> VerifyHashedPasswordAsync(string hashedPassword, string inputPassword);
    }
}