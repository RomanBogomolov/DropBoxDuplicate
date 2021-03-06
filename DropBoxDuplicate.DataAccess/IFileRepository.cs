﻿using System;
using System.Collections.Generic;
using DropBoxDuplicate.Model;

namespace DropBoxDuplicate.DataAccess
{
    public interface IFileRepository
    {
        /// <summary>
        /// Добавить информацию о файл в репозиторий.
        /// </summary>
        /// <param name="file"><see cref="Files"/></param>
        /// <returns></returns>
        Files Add(Files file);
        
        /// <summary>
        /// Получить файл.
        /// </summary>
        /// <param name="id">Id файла</param>
        /// <returns></returns>
        byte[] GetContent(Guid id);
        
        /// <summary>
        /// Получить информацию о файле.
        /// </summary>
        /// <param name="fileId"></param>
        /// <returns></returns>
        Files GetInfo(Guid fileId);
        
        /// <summary>
        /// Добавить файл в репозиторий.
        /// </summary>
        /// <param name="fileId">Id файла</param>
        /// <param name="content">Base64</param>
        void UpdateContent(Guid fileId, byte[] content);
        
        /// <summary>
        /// Обновить наименование файла
        /// </summary>
        /// <param name="file"></param>
        void UpdateUserFileName(Files file);
       
        /// <summary>
        /// Получить список файлов в репозитории пользователя
        /// </summary>
        /// <param name="id">Id файла</param>
        /// <returns></returns>
        IEnumerable<Files> GetUsersFiles(Guid id);
        
        /// <summary>
        /// Удлаить файл
        /// </summary>
        /// <param name="id">Id файла</param>
        void Delete(Guid id);

        /// <summary>
        /// Список расшаренных файлов для пользователя
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        IDictionary<Files, AccessType> GetShareFiles(Guid userId);
        
        /// <summary>
        /// Расшарить файл для пользователей
        /// </summary>
        /// <param name="fileId"></param>
        /// <param name="userId"></param>
        void AddfileToShareForUser(Guid fileId, Guid[] userId);
        
        /// <summary>
        /// Проверить, не расшарен ли уже файл для пользователя.
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="fileId"></param>
        /// <returns></returns>
        bool IsFileShare(Guid userId, Guid fileId);

        /// <summary>
        /// Исключить доступ пользователей к расшаренному файлу.
        /// </summary>
        /// <param name="fileId"></param>
        /// <param name="userId"></param>
        void DeleteUserFromShare(Guid fileId, Guid[] userId);

        /// <summary>
        /// Изменить уровень доступа к файлу
        /// </summary>
        /// <param name="share">Параметры для расшаривания файла</param>
        void UpdateAccessToFile(Share share);

        /// <summary>
        /// Получить информацию о комментарии
        /// </summary>
        /// <param name="id">Идентификатор комментария</param>
        /// <returns></returns>
        Comment GetCommentInfo(Guid id);

        /// <summary>
        /// Добавить комментарий к файлу
        /// </summary>
        /// <param name="userId">Id пользователя</param>
        /// <param name="comment"><see cref="Comment"/></param>
        /// <param name="fileId">Id файла</param>
        Comment AddCommentToFile(Guid fileId, Guid userId, Comment comment);

        /// <summary>
        /// Получить все комментарии для файла
        /// </summary>
        /// <param name="fileId">Идентификатор файла</param>
        /// <returns></returns>
        IEnumerable<Comment> GetFileComments(Guid fileId);

        /// <summary>
        /// Удалить комментарий
        /// </summary>
        /// <param name="userId">Id пользователя</param>
        /// <param name="comment"><see cref="Comment"/></param>
        /// <param name="fileId">Id файла</param>
        void DeleteComment(Guid fileId, Guid userId, Comment comment);
    }
}