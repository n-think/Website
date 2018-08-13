using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Website.Service.DTO
{
    public class ClientDTO
    {
        [Display(Name = "Логин")]
        public virtual string UserLogin { get; set; }
        
        public virtual string Email { get; set; }
        [Display(Name = "Email подтвержден")]
        public virtual bool EmailConfirmed { get; set; }
        [Display(Name = "Телефон")]
        public virtual string PhoneNumber { get; set; }
        [Display(Name = "Телефон подтвержден")]
        public virtual bool PhoneNumberConfirmed { get; set; }
        [Display(Name = "Двухфакторный вход")]
        public virtual bool TwoFactorEnabled { get; set; }

        /// <summary>
        /// Gets or sets the date and time, in UTC, when any user lockout ends.
        /// </summary>
        /// <remarks>A value in the past means the user is not locked out.</remarks>
        [Display(Name = "Дата конца блокировки")]
        public virtual DateTimeOffset? LockoutEnd { get; set; }

        /// <summary>
        /// Gets or sets a flag indicating if the user could be locked out.
        /// </summary>
        /// <value>True if the user could be locked out, otherwise false.</value>
        [Display(Name = "Возможна ли блокировка")]
        public virtual bool LockoutEnabled { get; set; }

        /// <summary>
        /// Gets or sets the number of failed login attempts for the current user.
        /// </summary>
        [Display(Name = "Неудачных попыток входа")]
        public virtual int AccessFailedCount { get; set; }
        [Display(Name = "Дата последней активности")]
        public virtual DateTimeOffset? LastActivityDate { get; set; }

        public ClientProfileDTO ProfileDto;
    }
}
