using DropBoxDuplicate.DataAccess.Sql.Hashing.BCryptHashing;
using Microsoft.AspNet.Identity;

namespace DropBoxDuplicate.Api.Services
{
    /// <summary>
    /// Класс для хеширования пароля.
    /// </summary>
    public class PasswordService : IPasswordHasher
    {
        public string HashPassword(string password)
        {
            return PasswordHashing.HashPassword(password);
        }

        public PasswordVerificationResult VerifyHashedPassword(string hashedPassword, string providedPassword)
        {
            return PasswordHashing.ValidatePassword(providedPassword, hashedPassword)
                ? PasswordVerificationResult.Success
                : PasswordVerificationResult.Failed;
        }
    }
}