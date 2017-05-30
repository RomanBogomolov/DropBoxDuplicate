using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using DropBoxDuplicate.DataAccess.Sql.Extends;
using DropBoxDuplicate.Model;
using Microsoft.AspNet.Identity;
using Newtonsoft.Json;

namespace DropBoxDuplicate.DataAccess.Sql
{
    public sealed class FileRepository : IFileRepository
    {
        private readonly string _connectionString;
        private readonly IUserStore<IdentityUser, Guid> _usersRepository;
        
        public FileRepository(string connectionString, IUserStore<IdentityUser, Guid> user)
        {
            _connectionString = connectionString;
            _usersRepository = user;
        }

        public Files Add(Files file)
        {
            if (file == null)
            {
                throw new ArgumentNullException(nameof(file), "File не может быть null.");
            }

            using (var connection = new SqlConnection(_connectionString))
            {
                using (var command = new SqlCommand("upCreate_UserFile", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;

                    var fileId = Guid.NewGuid();
                    var createdDate = DateTimeOffset.Now;

                    command.Parameters.AddWithValue("@id", fileId);
                    command.Parameters.AddWithValue("@owner", file.User.Id);
                    command.Parameters.AddWithValue("@fileName", file.FileName);
                    command.Parameters.AddWithValue("@fileType", file.FileType);
                    command.Parameters.AddWithValue("@fileSize", file.FileSize);
                    command.Parameters.AddWithValue("@fileExtension", file.FileExtension);
                    command.Parameters.AddWithValue("@createdDate", createdDate);
                    command.Parameters.AddWithValue("@lastModifyDate", file.LastModifyDate);

                    connection.Open();
                    command.ExecuteNonQuery();

                    file.Id = fileId;
                    file.CreatedDate = createdDate;

                    return file;
                }
            }
        }

        public void Delete(Guid id)
        {
            if (id == Guid.Empty)
            {
                throw new ArgumentNullException(nameof(id), "Id не может быть null.");
            }

            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();

                using (var command = new SqlCommand("upDelete_UserFile", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@id", id);

                    command.ExecuteNonQuery();
                }
            }
        }

        public byte[] GetContent(Guid fileId)
        {
            if (fileId == Guid.Empty)
            {
                throw new ArgumentNullException(nameof(fileId),"Файл не может быть null.");
            }

            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();

                using (var command = connection.CreateCommand())
                {
                    command.CommandText = "SELECT FileContent FROM ufSelect_file_content(@id)";
                    command.Parameters.AddWithValue("@id", fileId);

                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            return reader.GetSqlBinary(reader.GetOrdinal("FileContent")).Value;
                        }

                        throw new ArgumentException($"Файл: {fileId} недоступен.");
                    }
                }
            }
        }

        public Files GetInfo(Guid fileId)
        {
            if (fileId == Guid.Empty)
            {
                throw new ArgumentNullException(nameof(fileId), "fileId не может быть empty.");
            }

            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = "SELECT " +
                                          "Id, " +
                                          "UserId, " +
                                          "FileName, " +
                                          "FileType, " +
                                          "FileSize, " +
                                          "FileExtension, " +
                                          "CreatedDate, " +
                                          "LastModifyDate " +
                                          "FROM ufSelect_file_info_by_id(@id)";

                    command.Parameters.AddWithValue("@id", fileId);

                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var fileInfo = new Files
                            {
                                Id = reader.GetGuid(reader.GetOrdinal("id")),
                                FileName = reader.GetString(reader.GetOrdinal("FileName")),
                                FileType = reader.GetStringSafe(reader.GetOrdinal("FileType")),
                                FileSize = reader.GetDoubleSafe(reader.GetOrdinal("FileSize")),
                                FileExtension = reader.GetStringSafe(reader.GetOrdinal("FileExtension")),
                                CreatedDate = reader.GetDateTimeOffset(reader.GetOrdinal("CreatedDate")),
                                LastModifyDate = reader["LastModifyDate"] != DBNull.Value ? reader.GetDateTimeOffset(reader.GetOrdinal("LastModifyDate")) : default(DateTimeOffset),
                                User = _usersRepository.FindByIdAsync(reader.GetGuid(reader.GetOrdinal("UserId"))).Result,
                            };
                            return fileInfo;
                        }

                        throw new ArgumentException($"Файл: {fileId} недоступен.");
                    }
                }
            }
        }

        public IEnumerable<Files> GetUsersFiles(Guid id)
        {
            if (id == Guid.Empty)
            {
                throw new ArgumentNullException(nameof(id), "Id не может быть empty.");
            }

            var files = new List<Files>();

            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = "SELECT " +
                                          "Id, " +
                                          "UserId, " +
                                          "FileName, " +
                                          "FileType, " +
                                          "FileSize, " +
                                          "FileExtension, " +
                                          "CreatedDate, " +
                                          "LastModifyDate " +
                                          "FROM ufSelect_file_info_by_userId(@userId)";

                    command.Parameters.AddWithValue("@userid", id);

                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            files.Add(GetInfo(reader.GetGuid(reader.GetOrdinal("Id"))));
                        }
                        return files;
                    }
                }
            }
        }

        public void UpdateContent(Guid fileId, byte[] content)
        {
            if (fileId == Guid.Empty)
            {
                throw new ArgumentNullException(nameof(fileId), "Файл не может быть null.");
            }

            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();

                using (var command = new SqlCommand("upUpdate_UserFileContent", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;

                    var lastModifyDate = DateTimeOffset.Now;

                    command.Parameters.AddWithValue("@id", fileId);
                    command.Parameters.AddWithValue("@fileContent", content);
                    command.Parameters.AddWithValue("@lastModifyDate", lastModifyDate);

                    command.ExecuteNonQuery();
                }
            }
        }
        public void UpdateUserFileName(Files file)
        {
            if (file == null)
            {
                throw new ArgumentNullException(nameof(file), "Файл не может быть null.");
            }

            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();

                using (var command = new SqlCommand("upUpdate_UserFileName", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;

                    var lastModifyDate = DateTimeOffset.Now;

                    command.Parameters.AddWithValue("@id", file.Id);
                    command.Parameters.AddWithValue("@fileName", file.FileName);
                    command.Parameters.AddWithValue("@lastModifyDate", lastModifyDate);

                    command.ExecuteNonQuery();
                }
            }
        }

        public IEnumerable<Files> GetShareFiles(Guid userId)
        {
            if (userId == Guid.Empty)
            {
                throw new ArgumentNullException(nameof(userId), "Id не может быть empty.");
            }

            var files = new List<Files>();

            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();

                using (var command = connection.CreateCommand())
                {
                    command.CommandText = "SELECT " +
                                          "idFile, " +
                                          "accessAtribute " +
                                          "FROM ufSelect_shareFiles_for_user(@userId)";

                    command.Parameters.AddWithValue("@userid", userId);

                    //Type = (AccessType)Enum.Parse(typeof(AccessType), reader.GetString(reader.GetOrdinal("accessAtribute")))

                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            files.Add(GetInfo(reader.GetGuid(reader.GetOrdinal("idFile"))));
                        }

                        return files;
                    }
                }
            }
        }

        public void AddfileToShareForUser(Guid fileId, Guid[] userId)
        {
            if (fileId == Guid.Empty)
            {
                throw new ArgumentNullException(nameof(userId), "fileid не может быть empty.");
            }

            if (userId.Length == 0)
            {
                throw new ArgumentNullException(nameof(userId), "usersId не могут быть empty.");
            }

            var saveData = userId.Select(i => new Share(i, fileId));

            var serializer = JsonConvert.SerializeObject(saveData);

            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();

                using (var command = new SqlCommand("up_Insert_users_to_file", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;

                    command.Parameters.AddWithValue("@jsonData", serializer);

                    command.ExecuteNonQuery();
                }
            }
        }

        public bool IsFileShare(Guid userId, Guid fileId)
        {
            if (fileId == Guid.Empty)
            {
                throw new ArgumentNullException(nameof(userId), "fileid не может быть empty.");
            }

            if (userId == Guid.Empty)
            {
                throw new ArgumentNullException(nameof(userId), "userId не может быть empty.");
            }

            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();

                using (var command = connection.CreateCommand())
                {
                    command.CommandText = "SELECT dbo.ufBit_file_is_share_for_user(@fileId,@userId)";

                    command.Parameters.AddWithValue("@fileId", fileId);
                    command.Parameters.AddWithValue("@userId", userId);

                    return (bool)command.ExecuteScalar();
                }
            }
        }

        public void DeleteUserFromShare(Guid fileId, Guid[] userId)
        {
            if (fileId == Guid.Empty)
            {
                throw new ArgumentNullException(nameof(userId), "fileid не может быть empty.");
            }

            if (userId.Length == 0)
            {
                throw new ArgumentNullException(nameof(userId), "userId не может быть empty.");
            }

            var deleteUsers = userId.Select(i => new Share(i, fileId));

            var serializer = JsonConvert.SerializeObject(deleteUsers);

            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();

                using (var command = new SqlCommand("up_Delete_users_from_share", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;

                    command.Parameters.AddWithValue("@jsonData", serializer);

                    command.ExecuteNonQuery();
                }
            }
        }

        public void UpdateAccessToFile(Guid fileId, Guid userId, AccessType type)
        {
            if (fileId == Guid.Empty)
            {
                throw new ArgumentNullException(nameof(fileId), "fileid не может быть empty.");
            }

            if (userId == Guid.Empty)
            {
                throw new ArgumentNullException(nameof(userId), "userId не может быть empty.");
            }

            if (Enum.IsDefined(typeof(AccessType), type))
            {
                throw new ArgumentNullException(nameof(type), "Не указан уровень доступа.");
            }

            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();

                using (var command = new SqlCommand("upUpdate_access_to_file_for_user", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;

                    command.Parameters.AddWithValue("@fileId", fileId);
                    command.Parameters.AddWithValue("@userId", userId);
                    command.Parameters.AddWithValue("@access", type);

                    command.ExecuteNonQuery();
                }
            }
        }
    }
}