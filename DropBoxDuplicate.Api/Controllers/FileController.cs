using System;
using System.Net;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Results;
using DropBoxDuplicate.DataAccess;
using DropBoxDuplicate.Model;
using Microsoft.AspNet.Identity;

namespace DropBoxDuplicate.Api.Controllers
{
    [RoutePrefix("api/file")]
    public class FileController : ApiController
    {
        private readonly IFileRepository _fileRepository;
        private readonly IUserStore<IdentityUser, Guid> _userRepository;

        public FileController(IFileRepository fileRepository, IUserStore<IdentityUser, Guid> userRepository)
        {
            _userRepository = userRepository;
            _fileRepository = fileRepository;
        }

        [Authorize]
        [HttpPost]
        [Route("add")]
        public IHttpActionResult AddFile(Files file)
        {
            _fileRepository.Add(file);
            return Ok();
        }

        [Authorize]
        [HttpGet]
        [Route("{id:guid}")]
        public IHttpActionResult GetFileInfo(Guid id)
        {
            if (id == Guid.Empty)
            {
                ModelState.AddModelError("fileId", "Не может быть empty.");
                return BadRequest(ModelState);
            }

            return Ok(_fileRepository.GetInfo(id));
        }

        [Authorize]
        [HttpDelete]
        [Route("{id:guid}")]
        public IHttpActionResult DeleteFile(Guid id)
        {
            if (id == Guid.Empty)
            {
                ModelState.AddModelError("id", "Не может быть empty.");
                return BadRequest(ModelState);
            }

            _fileRepository.Delete(id);

            return new StatusCodeResult(HttpStatusCode.Accepted, this);
        }

        [Authorize]
        [HttpGet]
        [Route("{id:guid}/content")]
        public IHttpActionResult GetContent(Guid id)
        {
            if (id == Guid.Empty)
            {
                ModelState.AddModelError("id", "Не может быть empty.");
                return BadRequest(ModelState);
            }

            var content = _fileRepository.GetContent(id);

            return Ok(content);
        }

        [Authorize]
        [HttpPut]
        [Route("{id:guid}/update")]
        public async Task<IHttpActionResult> UpdateFileContent(Guid id)
        {
            if (id == Guid.Empty)
            {
                ModelState.AddModelError("id", "Не может быть empty.");
                return BadRequest(ModelState);
            }

            var bytes = await Request.Content.ReadAsByteArrayAsync();

            _fileRepository.UpdateContent(id, bytes);

            return Ok();
        }

        [Authorize]
        [HttpGet]
        [Route("user/{id:guid}")]
        public IHttpActionResult GetUserFiles(Guid id)
        {
            var userFiles = _fileRepository.GetUsersFiles(id);
            return Ok(userFiles);
        }

        [Authorize]
        [HttpGet]
        [Route("user/{id:guid}/share")]
        public IHttpActionResult GetShareFiles(Guid id)
        {
            var userFiles = _fileRepository.GetShareFiles(id);
            return Ok(userFiles);
        }

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

        [Authorize]
        [HttpDelete]
        [Route("{id:guid}/share/")]
        public IHttpActionResult DeleteUsersFromShare(Guid id, Guid[] userId)
        {
            foreach (var idUser in userId)
            {
                if (_userRepository.FindByIdAsync(idUser) == null)
                {
                    ModelState.AddModelError("user", $"Пользователь c {idUser} не зарегистрирован.");
                    return BadRequest(ModelState);
                }

                if (!_fileRepository.IsFileShare(idUser, id))
                {
                    ModelState.AddModelError("user", $"Для пользователя c {idUser} файл не расшарен.");
                    return BadRequest(ModelState);
                }
            }

            _fileRepository.DeleteUserFromShare(id, userId);

            return new StatusCodeResult(HttpStatusCode.Accepted, this);
        }

        [Authorize]
        [HttpPut]
        [Route("{id:guid}/share/user/{userId:guid}")]
        public IHttpActionResult UpdateAccessToFile(Guid id, Guid userId, AccessType type)
        {
            var user = _userRepository.FindByIdAsync(userId);

            if (user == null)
            {
                ModelState.AddModelError("user", $"Пользователь c {userId} не зарегистрирован.");
                return BadRequest();
            }

            var share = _fileRepository.IsFileShare(userId, id);

            if (share)
            {
                _fileRepository.UpdateAccessToFile(id, userId, type);
                return Ok();
            }

            ModelState.AddModelError("user", $"Для пользователя c {userId} файл {id} не расшарен.");
            return BadRequest();
        }
    }
}
