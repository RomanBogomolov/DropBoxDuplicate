using CryptoManager = BCrypt.Net.BCrypt;

namespace DropBoxDuplicate.DataAccess.Sql.Hashing.BCryptHashing
{
    public class PasswordHashing
    {
        /// <summary>
        /// Генерация соли
        /// </summary>
        /// <returns></returns>
        private static string GetRandomSalt()
        {
            //2^10 итераций по умолчанию
            return CryptoManager.GenerateSalt();
        }

        /// <summary>
        /// Генерация хеша пароля
        /// </summary>
        /// <param name="password">Пароль</param>
        /// <returns></returns>
        public static string HashPassword(string password)
        {
            return CryptoManager.HashPassword(password, GetRandomSalt());
        }

        /// <summary>
        /// Сравнение пароля с его хешем
        /// </summary>
        /// <param name="password">Пароль</param>
        /// <param name="correctHash">Хеш пароля из БД</param>
        /// <returns></returns>
        public static bool ValidatePassword(string password, string correctHash)
        {
            return CryptoManager.Verify(password, correctHash);
        }

    }
}