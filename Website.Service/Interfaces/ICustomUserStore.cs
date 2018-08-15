using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Website.Service.DTO;

namespace Website.Service.Interfaces
{
    public interface ICustomUserStore<TUser, TDbUser, TRole, TDbRole, TDbUserClaim, TDbUserRole, TDbUserLogin, TDbUserToken, TDbRoleClaim> :
    IUserStore<TUser>,
    IUserPasswordStore<TUser>,
    IUserEmailStore<TUser>,
    IUserLoginStore<TUser>,
    IUserRoleStore<TUser>,
    IUserSecurityStampStore<TUser>,
    IUserClaimStore<TUser>,
    IUserAuthenticationTokenStore<TUser>,
    IUserTwoFactorStore<TUser>,
    IUserPhoneNumberStore<TUser>,
    IUserLockoutStore<TUser>
    //,IQueryableUserStore<TUser> не получится через dto
    where TUser : class
    where TDbUser : class
    where TRole : class
    where TDbRole : class
    where TDbUserClaim : class, new()
    where TDbUserRole : class, new()
    where TDbUserLogin : class, new()
    where TDbUserToken : class, new()
    where TDbRoleClaim : class, new()
    {
        DbContext Context { get; }
    }
}
