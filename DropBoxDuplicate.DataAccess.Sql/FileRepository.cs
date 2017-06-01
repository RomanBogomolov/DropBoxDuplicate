using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using DropBoxDuplicate.DataAccess.Sql.Extends;
using DropBoxDuplicate.Log;
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
            using (LogWrapper logger = new LogWrapper())
            {
                if (file == null)
                {
                    logger.Error("File не может быть null.");
                    throw new ArgumentNullException(nameof(file), "File не может быть null.");
                }
                try
                {
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
                catch (Exception e)
                {
                    logger.Error(e.Message);
                    return null;
                }
            }
        }

        public void Delete(Guid id)
        {
            using (LogWrapper logger = new LogWrapper())
            {
                if (id == Guid.Empty)
                {
                    logger.Error("Id не может быть null.");
                    throw new ArgumentNullException(nameof(id), "Id не может быть null.");
                }
                try
                {
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
                catch (Exception e)
                {
                    logger.Error(e.Message);
                }
            }
        }

        public byte[] GetContent(Guid fileId)
        {
            using (LogWrapper logger = new LogWrapper())
            {
                if (fileId == Guid.Empty)
                {
                    logger.Error("Файл не может быть null.");
                    throw new ArgumentNullException(nameof(fileId), "Файл не может быть null.");
                }
                try
                {
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
                catch (Exception e)
                {
                    logger.Error(e.Message);
                    return null;
                }
            }
        }

        public Files GetInfo(Guid fileId)
        {
            using (LogWrapper logger = new LogWrapper())
            {
                if (fileId == Guid.Empty)
                {
                    logger.Error("fileId не может быть empty.");
                    throw new ArgumentNullException(nameof(fileId), "fileId не может быть empty.");
                }
                try
                {
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
                                        LastModifyDate = reader["LastModifyDate"] != DBNull.Value
                                            ? reader.GetDateTimeOffset(reader.GetOrdinal("LastModifyDate"))
                                            : default(DateTimeOffset),
                                        User = _usersRepository
                                            .FindByIdAsync(reader.GetGuid(reader.GetOrdinal("UserId")))
                                            .Result,
                                    };
                                    return fileInfo;
                                }

                                throw new ArgumentException($"Файл: {fileId} недоступен.");
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    logger.Error(e.Message);
                    return null;
                }
            }
        }

        public IEnumerable<Files> GetUsersFiles(Guid id)
        {
            using (LogWrapper logger = new LogWrapper())
            {
                if (id == Guid.Empty)
                {
                    logger.Error("Id не может быть empty.");
                    throw new ArgumentNullException(nameof(id), "Id не может быть empty.");
                }
                try
                {
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
                catch (Exception e)
                {
                    logger.Error(e.Message);
                    return null;
                }
            }
        }

        public void UpdateContent(Guid fileId, byte[] content)
        {
            using (LogWrapper logger = new LogWrapper())
            {
                if (fileId == Guid.Empty)
                {
                    logger.Error("Файл не может быть null.");
                    throw new ArgumentNullException(nameof(fileId), "Файл не может быть null.");
                }
                try
                {
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
                catch (Exception e)
                {
                    logger.Error(e.Message);
                }
            }
        }

        public void UpdateUserFileName(Files file)
        {
            using (LogWrapper logger = new LogWrapper())
            {
                if (file == null)
                {
                    logger.Error("Файл не может быть null.");
                    throw new ArgumentNullException(nameof(file), "Файл не может быть null.");
                }
                try
                {
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
                catch (Exception e)
                {
                    logger.Error(e.Message);
                }
            }
        }

        public IDictionary<Files, AccessType> GetShareFiles(Guid userId)
        {
            using (LogWrapper logger = new LogWrapper())
            {
                if (userId == Guid.Empty)
                {
                    logger.Error("Id не может быть empty.");
                    throw new ArgumentNullException(nameof(userId), "Id не может быть empty.");
                }

                try
                {
                    var files = new Dictionary<Files, AccessType>();

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

                            using (var reader = command.ExecuteReader())
                            {
                                while (reader.Read())
                                {
                                    files.Add(GetInfo(reader.GetGuid(reader.GetOrdinal("idFile"))),
                                        (AccessType) Enum.Parse(typeof(AccessType),
                                            reader.GetString(reader.GetOrdinal("accessAtribute"))));
                                }
                                return files;
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    logger.Error(e.Message);
                    return null;
                }
            }
        }

        public void AddfileToShareForUser(Guid fileId, Guid[] userId)
        {
            using (LogWrapper logger = new LogWrapper())
            {
                if (fileId == Guid.Empty)
                {
                    logger.Error("fileid не может быть empty.");
                    throw new ArgumentNullException(nameof(userId), "fileid не может быть empty.");
                }
                if (userId.Length == 0)
                {
                    logger.Error("userIds не могут быть empty.");
                    throw new ArgumentNullException(nameof(userId), "usersId не могут быть empty.");
                }
                try
                {
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
                catch (Exception e)
                {
                    logger.Error(e.Message);
                }
            }
        }

        public bool IsFileShare(Guid userId, Guid fileId)
        {
            using (LogWrapper logger = new LogWrapper())
            {
                if (fileId == Guid.Empty)
                {
                    logger.Error("fileid не может быть empty.");
                    throw new ArgumentNullException(nameof(userId), "fileid не может быть empty.");
                }

                if (userId == Guid.Empty)
                {
                    logger.Error("userId не может быть empty.");
                    throw new ArgumentNullException(nameof(userId), "userId не может быть empty.");
                }
                try
                {
                    using (var connection = new SqlConnection(_connectionString))
                    {
                        connection.Open();

                        using (var command = connection.CreateCommand())
                        {
                            command.CommandText = "SELECT dbo.ufBit_file_is_share_for_user(@fileId,@userId)";

                            command.Parameters.AddWithValue("@fileId", fileId);
                            command.Parameters.AddWithValue("@userId", userId);

                            return (bool) command.ExecuteScalar();
                        }
                    }
                }
                catch (Exception e)
                {
                    logger.Error(e.Message);
                    throw;
                }
            }
        }

        public void DeleteUserFromShare(Guid fileId, Guid[] userId)
        {
            using (LogWrapper logger = new LogWrapper())
            {
                if (fileId == Guid.Empty)
                {
                    logger.Error("fileid не может быть empty.");
                    throw new ArgumentNullException(nameof(userId), "fileid не может быть empty.");
                }

                if (userId.Length == 0)
                {
                    logger.Error("userIds не могут быть empty.");
                    throw new ArgumentNullException(nameof(userId), "userId не может быть empty.");
                }
                try
                {
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
                catch (Exception e)
                {
                    logger.Error(e.Message);
                }
            }
        }

        public void UpdateAccessToFile(Share share)
        {
            using (LogWrapper logger = new LogWrapper())
            {
                if (share.FileId == Guid.Empty)
                {
                    logger.Error("fileid не может быть empty.");
                    throw new ArgumentNullException(nameof(share.FileId), "fileid не может быть empty.");
                }

                if (share.UserId == Guid.Empty)
                {
                    logger.Error("userId не может быть empty.");
                    throw new ArgumentNullException(nameof(share.UserId), "userId не может быть empty.");
                }

                if (!Enum.IsDefined(typeof(AccessType), share.AccessAtribute))
                {
                    logger.Error("Не указан уровень доступа.");
                    throw new ArgumentNullException(nameof(share.AccessAtribute), "Не указан уровень доступа.");
                }
                try
                {
                    using (var connection = new SqlConnection(_connectionString))
                    {
                        connection.Open();

                        using (var command = new SqlCommand("upUpdate_access_to_file_for_user", connection))
                        {
                            command.CommandType = CommandType.StoredProcedure;

                            command.Parameters.AddWithValue("@fileId", share.FileId);
                            command.Parameters.AddWithValue("@userId", share.UserId);
                            command.Parameters.AddWithValue("@access", (byte) share.AccessAtribute);

                            command.ExecuteNonQuery();
                        }
                    }
                }
                catch (Exception e)
                {
                    logger.Error(e.Message);
                }
            }
        }

        public Comment GetCommentInfo(Guid id)
        {
            using (LogWrapper logger = new LogWrapper())
            {
                if (id == Guid.Empty)
                {
                    logger.Error("id не может быть empty");
                    throw new ArgumentNullException(nameof(id), "id не может быть empty");
                }

                try
                {
                    using (var connection = new SqlConnection(_connectionString))
                    {
                        connection.Open();
                        using (var command = connection.CreateCommand())
                        {
                            command.CommandText = "SELECT " +
                                                  "Id, " +
                                                  "userId, " +
                                                  "fileId, " +
                                                  "Text, " +
                                                  "PostDate FROM ufSelect_comment_by_id(@id)";

                            command.Parameters.AddWithValue("@id", id);

                            using (var reader = command.ExecuteReader())
                            {
                                while (reader.Read())
                                {
                                    var commentIfo = new Comment
                                    {
                                        Id = reader.GetGuid(reader.GetOrdinal("id")),
                                        Text = reader.GetString(reader.GetOrdinal("text")),
                                        PostDate = reader.GetDateTimeOffset(reader.GetOrdinal("PostDate")),
                                        User = _usersRepository
                                            .FindByIdAsync(reader.GetGuid(reader.GetOrdinal("userId")))
                                            .Result,
                                        File = GetInfo(reader.GetGuid(reader.GetOrdinal("fileId")))
                                    };

                                    return commentIfo;
                                }
                                logger.Error($"Комментарий: {id} недоступен.");
                                throw new ArgumentException($"Комментарий: {id} недоступен.");
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    logger.Error(e.Message);
                    return null;
                }
            }
        }

        public Comment AddCommentToFile(Guid fileId, Guid userId, Comment comment)
        {
            using (LogWrapper logger = new LogWrapper())
            {
                if (comment == null)
                {
                    logger.Error("Комментарий не может быть null.");
                    throw new ArgumentNullException(nameof(comment), "Комментарий не может быть null.");
                }
                if (fileId == Guid.Empty)
                {
                    logger.Error("fileId не может быть empty.");
                    throw new ArgumentNullException(nameof(fileId), "fileId не может быть empty.");
                }
                if (userId == Guid.Empty)
                {
                    logger.Error("userId не может быть empty.");
                    throw new ArgumentNullException(nameof(userId), "userId не может быть empty.");
                }

                try
                {
                    using (var connection = new SqlConnection(_connectionString))
                    {
                        connection.Open();

                        using (var command = new SqlCommand("upCreate_new_comment", connection))
                        {
                            command.CommandType = CommandType.StoredProcedure;

                            var commentId = Guid.NewGuid();
                            var createdDate = DateTimeOffset.Now;

                            command.Parameters.AddWithValue("@id", commentId);
                            command.Parameters.AddWithValue("@userId", userId);
                            command.Parameters.AddWithValue("@fileId", fileId);
                            command.Parameters.AddWithValue("@text", comment.Text);
                            command.Parameters.AddWithValue("@postDate", createdDate);

                            command.ExecuteNonQuery();

                            comment.Id = commentId;
                            comment.PostDate = createdDate;

                            return comment;
                        }
                    }
                }
                catch (Exception e)
                {
                    logger.Error(e.Message);
                    return null;
                }
            }
        }

        public IEnumerable<Comment> GetFileComments(Guid fileId)
        {
            using (LogWrapper logger = new LogWrapper())
            {
                if (fileId == Guid.Empty)
                {
                    logger.Error("Id не может быть empty.");
                    throw new ArgumentNullException(nameof(fileId), "Id не может быть empty.");
                }

                try
                {
                    var comments = new List<Comment>();

                    using (var connection = new SqlConnection(_connectionString))
                    {
                        connection.Open();
                        using (var command = connection.CreateCommand())
                        {
                            command.CommandText = "SELECT " +
                                                  "Id, " +
                                                  "userId, " +
                                                  "fileId, " +
                                                  "Text, " +
                                                  "PostDate " +
                                                  "FROM ufSelect_comments_by_fileId(@fileId)";

                            command.Parameters.AddWithValue("@fileId", fileId);

                            using (var reader = command.ExecuteReader())
                            {
                                while (reader.Read())
                                {
                                    comments.Add(GetCommentInfo(reader.GetGuid(reader.GetOrdinal("Id"))));
                                }

                                return comments;
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    logger.Error(e.Message);
                    return null;
                }
            }
        }

        public void DeleteComment(Guid fileId, Guid userId, Comment comment)
        {
            using (LogWrapper logger = new LogWrapper())
            {
                if (comment == null)
                {
                    logger.Error("Comment не может быть null.");
                    throw new ArgumentNullException(nameof(comment), "Comment не может быть null.");
                }
                try
                {
                    using (var connection = new SqlConnection(_connectionString))
                    {
                        connection.Open();

                        using (var command = new SqlCommand("upDelete_comment", connection))
                        {
                            command.CommandType = CommandType.StoredProcedure;

                            command.Parameters.AddWithValue("@id", comment.Id);
                            command.Parameters.AddWithValue("@userId", userId);
                            command.Parameters.AddWithValue("@fileId", fileId);

                            command.ExecuteNonQuery();
                        }
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    throw;
                }
            }
        }
    }
}