using API.Services.Argon;
using Konscious.Security.Cryptography;
using System.Security.Cryptography;
using System.Text;

public class ArgonHashing : IArgonHashing
{
    private const int SaltSize = 16;  // 128-bit
    private const int HashSize = 32;  // 256-bit
    private const int Iterations = 4;
    private const int MemorySize = 65536; // 64MB
    private const int DegreeOfParallelism = 2;

    public async Task<string> HashPasswordAsync(string password)
    {
        var salt = RandomNumberGenerator.GetBytes(SaltSize);
        var hash = await HashWithArgon2Async(password, salt);

        var result = Convert.ToBase64String(salt) + "." + Convert.ToBase64String(hash);
        return result;
    }

    public async Task<bool> VerifyHashedPasswordAsync(string hashedPassword, string inputPassword)
    {
        try
        {
            var parts = hashedPassword.Split('.');
            if (parts.Length != 2)
                return false;

            var salt = Convert.FromBase64String(parts[0]);
            var originalHash = Convert.FromBase64String(parts[1]);
            var inputHash = await HashWithArgon2Async(inputPassword, salt);

            return CryptographicOperations.FixedTimeEquals(originalHash, inputHash);
        }
        catch
        {
            return false;
        }
    }

    private async Task<byte[]> HashWithArgon2Async(string password, byte[] salt)
    {
        var argon2 = new Argon2id(Encoding.UTF8.GetBytes(password))
        {
            Salt = salt,
            DegreeOfParallelism = DegreeOfParallelism,
            MemorySize = MemorySize,
            Iterations = Iterations
        };

        return await argon2.GetBytesAsync(HashSize);
    }
}
