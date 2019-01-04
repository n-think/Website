﻿using System;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;

namespace Website.Core.DTO
{
    /// <summary>
    /// Represents a user in the identity system
    /// </summary>
    public class UserDto : ClaimsIdentity
    {
        /// <summary>
        /// Initializes a new instance of <see cref="UserDto" />.
        /// </summary>
        /// <remarks>
        /// The Id property is initialized to form a new GUID string value.
        /// </remarks>
        public UserDto()
        {
            this.Id = Guid.NewGuid().ToString();
        }

        /// <summary>
        /// Initializes a new instance of <see cref="UserDto" />.
        /// </summary>
        /// <param name="userName">The user name.</param>
        /// <remarks>
        /// The Id property is initialized to form a new GUID string value.
        /// </remarks>
        public UserDto(string userName)
            : this()
        {
            this.UserName = userName;
        }

        public string Id { get; set; }
        [Display(Name = "Логин")]
        public string UserName { get; set; }
        public string NormalizedUserName { get; set; }
        public string Email { get; set; }
        public string NormalizedEmail { get; set; }
        [Display(Name = "Email подтвержден")]
        public bool EmailConfirmed { get; set; }
        public string PasswordHash { get; set; }
        [Display(Name = "Телефон")]
        public string PhoneNumber { get; set; }
        [Display(Name = "Телефон подтвержден")]
        public bool PhoneNumberConfirmed { get; set; }
        public string ConcurrencyStamp { get; set; }
        public string SecurityStamp { get; set; }
        [Display(Name = "Возможна ли блокировка")]
        public bool LockoutEnabled { get; set; }
        [Display(Name = "Дата конца блокировки")]
        public DateTimeOffset? LockoutEnd { get; set; }
        [Display(Name = "Двухфакторный вход")]
        public bool TwoFactorEnabled { get; set; }
        [Display(Name = "Неудачных попыток входа")]
        public int AccessFailedCount { get; set; }
        public UserProfileDto UserProfile { get; set; }
        [Display(Name = "Дата последней активности")]
        public DateTimeOffset? LastActivityDate { get; set; }

        public override string ToString()
        {
            return this.UserName;
        }
    }
}