using System;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using DropBoxDuplicate.Model;
using Microsoft.AspNet.Identity;
using Dapper;
using DropBoxDuplicate.DataAccess.Sql.Hashing.BCryptHashing;
using DropBoxDuplicate.Log;


namespace DropBoxDuplicate.DataAccess.Sql
{
    public class IdentityUserRepository :
        IUserPasswordStore<IdentityUser, Guid>,
        IUserSecurityStampStore<IdentityUser, Guid>,
        IUserEmailStore<IdentityUser, Guid>

    {
        private readonly string _connectionString;

        public IdentityUserRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public void Dispose(){ }

        #region IUserStore<IdentityUser, Guid>

        public Task CreateAsync(IdentityUser user)
        {
            using (LogWrapper logger = new LogWrapper())
            {
                if (user == null)
                {
                    logger.Error("Пользователь не может быть null.");
                    throw new ArgumentNullException(nameof(user), "Пользователь не может быть null.");
                }
                try
                {
                    return Task.Factory.StartNew(() =>
                    {
                        using (var connection = new SqlConnection(_connectionString))
                        {
                            connection.Open();

                            user.Id = Guid.NewGuid();
                            user.RegDate = DateTimeOffset.Now;
                            user.PasswordHash = PasswordHashing.HashPassword(user.PasswordHash);

                            connection.Execute("upCreate_new_user",
                                new
                                {
                                    user.Id,
                                    user.UserName,
                                    user.PasswordHash,
                                    user.Email,
                                    user.EmailConfirmed,
                                    user.FirstName,
                                    user.SecondName,
                                    user.RegDate,
                                    user.City,
                                    user.BirthDate,
                                    user.SecurityStamp
                                },
                                commandType: CommandType.StoredProcedure);
                        }
                    });
                }
                catch (Exception e)
                {
                    logger.Error(e.Message);
                    return null;
                }
            }
        }

        public Task DeleteAsync(IdentityUser user)
        {
            using (LogWrapper logger = new LogWrapper())
            {
                if (user == null)
                {
                    logger.Error("Пользователь не может быть null.");
                    throw new ArgumentNullException(nameof(user), "Пользователь не может быть null.");
                }
                try
                {
                    return Task.Factory.StartNew(() =>
                    {
                        using (var connection = new SqlConnection(_connectionString))
                        {
                            connection.Open();
                            connection.Execute("upDelete_user", new {user.Id},
                                commandType: CommandType.StoredProcedure);
                        }
                    });
                }

                catch (Exception e)
                {
                    logger.Error(e.Message);
                    return null;
                }
            }
        }

        public Task<IdentityUser> FindByIdAsync(Guid userId)
        {
            using (LogWrapper logger = new LogWrapper())
            {
                if (userId == Guid.Empty)
                {
                    logger.Error("userId не может быть null.");
                    throw new ArgumentNullException(nameof(userId), "userId не может быть null.");
                }
                try
                {
                    return Task.Factory.StartNew(() =>
                    {
                        using (var connection = new SqlConnection(_connectionString))
                        {
                            connection.Open();
                            return
                                connection.Query<IdentityUser>(
                                        "SELECT " +
                                        "Id, " +
                                        "UserName, " +
                                        "PasswordHash, " +
                                        "Email, " +
                                        "EmailConfirmed, " +
                                        "FirstName, " +
                                        "SecondName, " +
                                        "RegDate, " +
                                        "City, " +
                                        "BirthDate, " +
                                        "SecurityStamp FROM ufSelect_user_by_id(@userId)", new {userId})
                                    .SingleOrDefault();
                        }
                    });
                }
                catch (Exception e)
                {
                    logger.Error(e.Message);
                    return null;
                }
            }
        }

        public Task<IdentityUser> FindByNameAsync(string userName)
        {
            using (LogWrapper logger = new LogWrapper())
            {
                if (string.IsNullOrEmpty(userName))
                {
                    logger.Error("Имя пользоватиеля не может быть null.");
                    throw new ArgumentNullException(nameof(userName), "Имя пользоватиеля не может быть null.");
                }
                try
                {
                    return Task.Factory.StartNew(() =>
                    {
                        using (var connection = new SqlConnection(_connectionString))
                        {
                            connection.Open();

                            return
                                connection.Query<IdentityUser>(
                                        "SELECT " +
                                        "Id, " +
                                        "UserName, " +
                                        "PasswordHash, " +
                                        "Email, " +
                                        "EmailConfirmed, " +
                                        "FirstName, " +
                                        "SecondName, " +
                                        "RegDate, " +
                                        "City, " +
                                        "BirthDate, " +
                                        "SecurityStamp FROM ufSelect_user_by_username(@userName)", new {userName})
                                    .SingleOrDefault();
                        }
                    });
                }
                catch (Exception e)
                {
                    logger.Error(e.Message);
                    return null;
                }
            }
        }

        public Task UpdateAsync(IdentityUser user)
        {
            using (LogWrapper logger = new LogWrapper())
            {
                if (user == null)
                {
                    logger.Error("Пользователь не может быть null.");
                    throw new ArgumentNullException(nameof(user), "Пользователь не может быть null.");
                }
                try
                {
                    return Task.Factory.StartNew(() =>
                    {
                        using (var connection = new SqlConnection(_connectionString))
                        {
                            connection.Open();

                            connection.Execute("upUpdate_user", new
                            {
                                user.Id,
                                user.UserName,
                                user.PasswordHash,
                                user.Email,
                                user.EmailConfirmed,
                                user.FirstName,
                                user.SecondName,
                                user.RegDate,
                                user.City,
                                user.BirthDate,
                                user.SecurityStamp
                            }, commandType: CommandType.StoredProcedure);
                        }
                    });
                }
                catch (Exception e)
                {
                    logger.Error(e.Message);
                    throw;
                }
            }
        }

        #endregion

        #region IUserPasswordStore<IdentityUser, Guid>

        public Task<string> GetPasswordHashAsync(IdentityUser user)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user), "Пользователь не может быть null.");
            }

            return Task.FromResult(user.PasswordHash);
        }

        public Task<bool> HasPasswordAsync(IdentityUser user)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user), "Пользователь не может быть null.");
            }
            return Task.FromResult(!string.IsNullOrWhiteSpace(user.PasswordHash));
        }

        public Task SetPasswordHashAsync(IdentityUser user, string passwordHash)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user), "Пользователь не может быть null.");
            }
            if (string.IsNullOrWhiteSpace(passwordHash))
            {
                throw new ArgumentOutOfRangeException(nameof(passwordHash), "Пароль не может быть null.");
            }

            user.PasswordHash = passwordHash;

            return Task.FromResult(0);
        }

        #endregion

        #region IUserSecurityStampStore<IdentityUser, Guid>

        public Task SetSecurityStampAsync(IdentityUser user, string stamp)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user), "Пользователь не может быть null.");
            }
            if (string.IsNullOrWhiteSpace(stamp))
            {
                throw new ArgumentNullException(nameof(stamp),
                    "Штамп состояния (SecurityStamp) не может быть null.");
            }

            user.SecurityStamp = new Guid(stamp);

            return Task.FromResult(0);
        }

        public Task<string> GetSecurityStampAsync(IdentityUser user)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user), "Пользователь не может быть null.");
            }

            return Task.FromResult(user.SecurityStamp.ToString());
        }

        #endregion

        #region IUserEmailStore<IdentityUser, Guid>

        public Task SetEmailAsync(IdentityUser user, string email)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user), "Пользователь не может быть null.");
            }
            if (string.IsNullOrWhiteSpace(email))
            {
                throw new ArgumentNullException(nameof(email), "Email не может быть null.");
            }

            user.Email = email;

            return Task.FromResult(0);
        }

        public Task<string> GetEmailAsync(IdentityUser user)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user), "Пользователь не может быть null.");
            }

            return Task.FromResult(user.Email);
        }

        public Task<bool> GetEmailConfirmedAsync(IdentityUser user)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user), "Пользователь не может быть null.");
            }

            return Task.FromResult(user.EmailConfirmed);
        }

        public Task SetEmailConfirmedAsync(IdentityUser user, bool confirmed)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user), "Пользователь не может быть null.");
            }

            user.EmailConfirmed = confirmed;

            return Task.FromResult(0);
        }

        public Task<IdentityUser> FindByEmailAsync(string email)
        {
            using (LogWrapper logger = new LogWrapper())
            {
                if (string.IsNullOrEmpty(email))
                {
                    logger.Error("Email не может быть null.");
                    throw new ArgumentNullException(nameof(email), "Email не может быть null.");
                }
                try
                {
                    return Task.Factory.StartNew(() =>
                    {
                        using (var connection = new SqlConnection(_connectionString))
                        {
                            connection.Open();
                            return
                                connection.Query<IdentityUser>(
                                        "SELECT " +
                                        "Id, " +
                                        "UserName, " +
                                        "PasswordHash, " +
                                        "Email, " +
                                        "EmailConfirmed, " +
                                        "FirstName, " +
                                        "SecondName, " +
                                        "RegDate, " +
                                        "City, " +
                                        "BirthDate, " +
                                        "SecurityStamp FROM ufSelect_user_by_email(@email)", new {email})
                                    .SingleOrDefault();
                        }
                    });
                }
                catch (Exception e)
                {
                    logger.Error(e.Message);
                    return null;
                }
            }
        }

        #endregion
    }
}
