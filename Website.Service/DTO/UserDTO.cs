using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Website.Data.EF.Models;

namespace Website.Service.DTO
{
    public class UserDTO : ClaimsIdentity
    {
        public string Id { get; set; }
        [Display(Name = "Логин")]
        public string UserName { get; set; }
        public string NormalizedUserName { get; set; }
        public string Email { get; set; }
        public string NormalizedEmail { get; set; }
        public bool EmailConfirmed { get; set; }
        public string PasswordHash { get; set; }
        public string PhoneNumber { get; set; }
        public bool PhoneNumberConfirmed { get; set; }
        public string ConcurrencyStamp { get; set; }
        public string SecurityStamp { get; set; }
        public DateTimeOffset? RegistrationDate { get; set; }
        public DateTimeOffset? LastLoginDate { get; set; }
        public bool LockoutEnabled { get; set; }
        public DateTimeOffset? LockoutEnd { get; set; }
        public bool TwoFactorEnabled { get; set; }
        public int AccessFailedCount { get; set; }
        public ClientProfileDTO ClientProfile { get; set; }
        public ICollection<UserRoleDTO> Roles { get; set; }
        //public ICollection<IdentityUserClaim<string>> Claims { get; set; }
        public DateTimeOffset? LastActivityDate { get; set; }
        public override string ToString()
        {
            return this.UserName;
        }
    }
}
