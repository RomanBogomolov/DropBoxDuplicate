using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Results;
using DropBoxDuplicate.Api.Models;
using DropBoxDuplicate.DataAccess;
using DropBoxDuplicate.Model;
using Microsoft.AspNet.Identity;
using Swashbuckle.Swagger.Annotations;

namespace DropBoxDuplicate.Api.Controllers
{
    [RoutePrefix("api/file")]
    public class FileController : ApiController
    {
        private readonly IFileRepository _fileRepository;
        private readonly IUserStore<IdentityUser, Guid> _userRepository;
        private readonly ModelFactory _modelFactory = new ModelFactory();

        public FileController(IFileRepository fileRepository, IUserStore<IdentityUser, Guid> userRepository)
        {
            _userRepository = userRepository;
            _fileRepository = fileRepository;
        }

        /// <summary>
        /// Добавление файла в репозиторий.
        /// </summary>
        /// <remarks>
        /// Запрос
        ///  
        ///     POST api/file/add
        ///     {
        ///        "filename":"testfile"
        ///        "user":{"id":"E6ECA3DC-5338-46D4-86D3-46AC9C60ACB9"}
        ///     }
        /// 
        /// </remarks>
        /// <param name="file">Информация о файле</param>
        /// <returns>Новую службу.</returns>
        [SwaggerResponse(200, "Успешно добавлен", typeof(Files))]
        [Authorize]
        [HttpPost]
        [Route("add")]
        public IHttpActionResult AddFile(Files file)
        {
            _fileRepository.Add(file);
            return Ok();
        }

        /// <summary>
        /// Получить информацию о файле.
        /// </summary>
        /// <remarks>
        /// Информация о файле
        ///  
        ///     GET api/file/E31C6286-81F4-4325-9C0E-74287D197261
        /// 
        /// </remarks>
        /// <param name="id">Идентификатор файла</param>
        /// <returns>Информация о файле.</returns>
        [SwaggerResponse(200, "Вернется объект", typeof(FileReturnModel))]
        [Authorize]
        [HttpGet]
        [Route("{id:guid}")]
        public IHttpActionResult GetFileInfo(Guid id)
        {
            var file = _fileRepository.GetInfo(id);

            if (file != null)
            {
                return Ok(_modelFactory.Create(file));
            }

            return BadRequest("Не удалось загрузить информацию о файле.");
        }

        /// <summary>
        /// Удалить файл из репозитория.
        /// </summary>
        /// <remarks>
        /// Удаление файла
        ///  
        ///     DELETE api/file/E31C6286-81F4-4325-9C0E-74287D197261
        /// 
        /// </remarks>
        /// <param name="id">Идентификатор файла</param>
        /// <response code="203">No content</response>
        /// <response code="404">Неверный запрос</response>
        [SwaggerResponse(203)]
        [Authorize]
        [HttpDelete]
        [Route("{id:guid}")]
        public IHttpActionResult DeleteFile(Guid id)
        {
            _fileRepository.Delete(id);
            return new StatusCodeResult(HttpStatusCode.Accepted, this);
        }

        /// <summary>
        /// Сохранить файл на диск.
        /// </summary>
        /// <remarks>
        /// Скачивание файла
        ///  
        ///     GET api/file/E31C6286-81F4-4325-9C0E-74287D197261/content
        /// 
        /// </remarks>
        /// <param name="id">Идентификатор файла</param>
        /// <response code="404">Неверный запрос</response>
        [SwaggerResponse(200, "Файл")]
        [Authorize]
        [HttpGet]
        [Route("{id:guid}/content")]
        public IHttpActionResult GetContent(Guid id)
        {
            return Ok(_fileRepository.GetContent(id));
        }

        /// <summary>
        /// Обновить файл.
        /// </summary>
        /// <remarks>
        /// Обновление
        ///  
        ///     PUT /api/file/E31C6286-81F4-4325-9C0E-74287D197261/update
        /// 
        /// </remarks>
        /// <param name="id">Идентификатор файла</param>
        /// <response code="404">Неверный запрос</response>
        [SwaggerResponse(200, "Успешно обновлен.")]
        [Authorize]
        [HttpPut]
        [Route("{id:guid}/update")]
        public async Task<IHttpActionResult> UpdateFileContent(Guid id)
        {
            var bytes = await Request.Content.ReadAsByteArrayAsync();
            _fileRepository.UpdateContent(id, bytes);

            return Ok();
        }

        /// <summary>
        /// Получить список файлов в репозитории пользователя
        /// </summary>
        /// <remarks>
        /// Запрос
        ///  
        ///     GET api/file/user/E6ECA3DC-5338-46D4-86D3-46AC9C60ACB9
        /// 
        /// </remarks>
        /// <param name="id">Идентификатор файла</param>
        /// <response code="404">Неверный запрос</response>
        [SwaggerResponse(200, "Вернется объект", typeof(UserFilesReturnModel))]
        [Authorize]
        [HttpGet]
        [Route("user/{id:guid}")]
        public IHttpActionResult GetFiles(Guid id)
        {
            var userFiles = _fileRepository.GetUsersFiles(id);

            if (userFiles != null)
            {
                var returnModel = new List<UserFilesReturnModel>();
                foreach (var file in userFiles)
                {
                    returnModel.Add(_modelFactory.CreateUserFiles(file));
                }
                return Ok(returnModel);
            }

            return BadRequest("Не удалось загрузить файлы.");
        }


        /// <summary>
        /// Получить список расшаренных файлов для пользователя
        /// </summary>
        /// <remarks>
        /// Запрос
        ///  
        ///     GET api/file/user/E6ECA3DC-5338-46D4-86D3-46AC9C60ACB9
        /// 
        /// </remarks>
        /// <param name="id">Идентификатор файла</param>
        /// <response code="404">Неверный запрос</response>
        [SwaggerResponse(200, "Вернется объект", typeof(UserFilesReturnModel))]
        [Authorize]
        [HttpGet]
        [Route("user/{id:guid}/share")]
        public IHttpActionResult GetShareFiles(Guid id)
        {
            var shareFiles = _fileRepository.GetShareFiles(id);

            if (shareFiles != null)
            {
                var returnValue = shareFiles.Select(file => _modelFactory.CreateShareFiles(file.Key, file.Value)).ToList();
                return Ok(returnValue);
            }

            return BadRequest("Не удалось найти расшаренные файлы.");
        }

        /// <summary>
        /// Расшарить файл для пользователей
        /// </summary>
        /// <remarks>
        /// Запрос
        ///  
        ///     POST api/file/BC345A1F-E173-47A7-9EE6-23B7BD7F71A9/share
        ///     {
        ///        ["E6ECA3DC-5338-46D4-86D3-46AC9C60ACB9","60809d21-bb77-4f0b-80a8-cc03826f6c91"]        
        ///     }
        /// 
        /// </remarks>
        /// <param name="id">Идентификатор файла</param>
        /// <param name="userId">Идентификаторы пользователей</param>
        /// <response code="404">Неверный запрос</response>
        [SwaggerResponse(200)]
        [Authorize]
        [HttpPost]
        [Route("{id:guid}/share/")]
        public IHttpActionResult AddShareToUsers(Guid id, Guid[] userId)
        {
            foreach (var idUser in userId)
            {
                if (_userRepository.FindByIdAsync(idUser) == null)
                {
                    ModelState.AddModelError("user", $"Пользователь c {idUser} не зарегистрирован.");
                    return BadRequest(ModelState);
                }

                if (_fileRepository.IsFileShare(idUser, id))
                {
                    ModelState.AddModelError("user", $"Для пользователя c {idUser} файл уже расшарен.");
                    return BadRequest(ModelState);
                }
            }

            _fileRepository.AddfileToShareForUser(id, userId);
            return Ok();
        }

        /// <summary>
        /// Удалить пользователей из шары
        /// </summary>
        /// <remarks>
        /// Запрос
        ///  
        ///     DELETE api/file/BC345A1F-E173-47A7-9EE6-23B7BD7F71A9/share
        ///     {
        ///        ["E6ECA3DC-5338-46D4-86D3-46AC9C60ACB9","60809d21-bb77-4f0b-80a8-cc03826f6c91"]      
        ///     }
        /// 
        /// </remarks>
        /// <param name="id">Идентификатор файла</param>
        /// <param name="userId">Идентификаторы пользователей</param>
        /// <response code="404">Неверный запрос</response>
        [SwaggerResponse(203)]
        [Authorize]
        [HttpDelete]
        [Route("{id:guid}/share/")]
        public async Task<IHttpActionResult> DeleteUsersFromShare(Guid id, Guid[] userId)
        {
            foreach (var idUser in userId)
            {
                if (await _userRepository.FindByIdAsync(idUser) == null)
                {
                    ModelState.AddModelError("user", $"Пользователь c {idUser} не зарегистрирован.");
                    return BadRequest(ModelState);
                }

                if (!_fileRepository.IsFileShare(idUser, id))
                {
                    ModelState.AddModelError("user", $"Для пользователя c {idUser} файл {id} не расшарен.");
                    return BadRequest(ModelState);
                }
            }

            _fileRepository.DeleteUserFromShare(id, userId);

            return new StatusCodeResult(HttpStatusCode.Accepted, this);
        }

        /// <summary>
        /// Обновить уровень доступа к файлу
        /// </summary>
        /// <remarks>
        /// Запрос
        ///  
        ///     PUT api/file/BC345A1F-E173-47A7-9EE6-23B7BD7F71A9/share/
        ///     {
        ///         "accessAtribute":"2",
        ///         "userId":"E6ECA3DC-5338-46D4-86D3-46AC9C60ACB9"
        ///     }
        /// 
        /// </remarks>
        /// <param name="id">Идентификатор файла</param>
        /// <param name="share">Информация о шаре</param>
        /// <response code="404">Неверный запрос</response>
        [SwaggerResponse(200, "Уровень доступа успешно обновлен.")]
        //[Authorize]
        [HttpPut]
        [Route("{id:guid}/share/")]
        public async Task<IHttpActionResult> UpdateAccessToFile(Guid id, Share share)
        {
            var user = await _userRepository.FindByIdAsync(share.UserId);

            if (user == null)
            {
                ModelState.AddModelError("user", $"Пользователь c {share.UserId} не зарегистрирован.");
                return BadRequest(ModelState);
            }

            var shareFile = _fileRepository.IsFileShare(share.UserId, id);

            if (shareFile)
            {
                share.FileId = id;
                 _fileRepository.UpdateAccessToFile(share);
                return Ok();
            }

            ModelState.AddModelError("user", $"Для пользователя c {share.UserId} файл {id} не расшарен.");
            return BadRequest(ModelState);
        }


        [HttpPost]
        [Route("{fileId:guid}/comment/user/{userId:guid}/")]
        public IHttpActionResult AddCommentToFile(Guid fileId, Guid userId, Comment comment)
        {
            var checkShareFile = _fileRepository.IsFileShare(userId, fileId);

            if (checkShareFile)
            {
                _fileRepository.AddCommentToFile(fileId, userId, comment);
                return Ok();
            }

            ModelState.AddModelError("file", $"Файл {fileId} для пользоваеля {userId} не расшарен.");
            return BadRequest(ModelState);
        }

        [HttpGet]
        [Route("{fileId:guid}/comment")]
        public IHttpActionResult GetFilecomments(Guid fileId)
        {
            var checkFile = _fileRepository.GetInfo(fileId);

            if (checkFile == null)
            {
                ModelState.AddModelError("file", "Файл не найден.");
                return BadRequest();
            }

            var comments = _fileRepository.GetFileComments(fileId);

            if (comments == null)
            {
                ModelState.AddModelError("file", "Комментариев не найдено.");
                return BadRequest();
            }

            return Ok(comments);
        }

        [HttpDelete]
        [Route("{fileId:guid}/comment/user/{userId:guid}/")]
        public IHttpActionResult DeleteCommentFromFile(Guid fileId, Guid userId, Comment comment)
        {
            _fileRepository.DeleteComment(fileId, userId, comment);
            return new StatusCodeResult(HttpStatusCode.Accepted, this);
        }
    }
}
