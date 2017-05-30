using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;

namespace DropBoxDuplicate.Model
{
    public sealed class IdentityUser : IUser<Guid>
    {
        public Guid Id { get; set; }
        public string UserName { get; set; }
        public string FirstName { get; set; }
        public string SecondName { get; set; }
        public string Email { get; set; }
        public bool EmailConfirmed { get; set; }
        public DateTimeOffset RegDate { get; set; }
        public string PasswordHash { get; set; }
        public string City { get; set; }
        public DateTimeOffset? BirthDate { get; set; }
        public Guid SecurityStamp { get; set; }


        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<IdentityUser, Guid> manager,
            string authenticationType)
        {
            var userIdentity = await manager.CreateIdentityAsync(this, authenticationType);
            return userIdentity;
        }
    }
}
