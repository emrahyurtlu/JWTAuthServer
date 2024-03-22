using Microsoft.AspNetCore.Identity;
using Shared.Entities;

namespace AuthServer.Core.Models
{
    public class UserApp: IdentityUser, IEntity
    {
        public string City { get; set; }
    }
}
