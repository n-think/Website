using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Security.Claims;
using System.Threading.Tasks;
using Castle.Core.Internal;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Website.Core.Enums;
using Website.Core.Infrastructure;
using Website.Core.Interfaces.Services;
using Website.Core.Models.Domain;

namespace Website.Services.Services
{
    public class UserManager : AspNetUserManager<User>, IUserManager
    {
        public UserManager(
            IUserStore<User> store,
            IOptions<IdentityOptions> optionsAccessor,
            IPasswordHasher<User> passwordHasher,
            IEnumerable<IUserValidator<User>> userValidators,
            IEnumerable<IPasswordValidator<User>> passwordValidators,
            ILookupNormalizer keyNormalizer,
            IdentityErrorDescriber errors,
            IServiceProvider services,
            ILogger<UserManager<User>> logger
        )
            : base(store,
                optionsAccessor,
                passwordHasher,
                userValidators,
                passwordValidators,
                keyNormalizer,
                errors,
                services,
                logger)
        {
        }

        public async Task<SortPageResult<User>> GetSortFilterPageAsync(RoleSelector roleSelector, string searchString,
            string sortPropName, int page, int pageCount)
        {
            ThrowIfDisposed();
            if (sortPropName == null) throw new ArgumentNullException(nameof(sortPropName));
            if (pageCount < 0) throw new ArgumentOutOfRangeException(nameof(pageCount));
            if (page < 0) throw new ArgumentOutOfRangeException(nameof(page));
            if (!Enum.IsDefined(typeof(RoleSelector), roleSelector))
                throw new InvalidEnumArgumentException(nameof(roleSelector), (int) roleSelector, typeof(RoleSelector));

            IQueryable<User> query = Users;
            
            // filter roles
            FilterUserQuery(roleSelector, ref query);

            // searching
            SearchUsersQuery(searchString, ref query);

            // ordering
            OrderUserQuery(sortPropName, ref query);

            // paginating
            var totalUsers = await query.CountAsync();
            query = query.Skip((page - 1) * pageCount).Take(pageCount);

            var users = await query.ToListAsync(CancellationToken);

            return new SortPageResult<User> {FilteredData = users, TotalN = totalUsers};
        }

        private void FilterUserQuery(RoleSelector rolePick, ref IQueryable<User> query)
        {
            this.ThrowIfDisposed();

            switch (rolePick)
            {
                case RoleSelector.Administrators:
                    query = query
                        .Where(x => x.UserRoles.Any(z => z.Role.Name == "admin"));
                    break;

                case RoleSelector.Users:
                    query = query
                            .Where(x => x.UserRoles.Any(z => z.Role.Name == "user"));
                    break;

                default:
                    query = query
                            .Where(x => x.UserRoles.Any(z => z.Role.Name == "admin" || z.Role.Name == "user"));
                    break;
            }
        }

        private void SearchUsersQuery(string searchString, ref IQueryable<User> query)
        {
            this.ThrowIfDisposed();

            if (!String.IsNullOrEmpty(searchString))
            {
                query = query.Where(x => x.UserName.Contains(searchString) ||
                                         x.Email.Contains(searchString) ||
                                         x.PhoneNumber != null && x.PhoneNumber.Contains(searchString) ||
                                         x.FirstName != null &&
                                         x.FirstName.Contains(searchString) ||
                                         x.LastName != null &&
                                         x.LastName.Contains(searchString) ||
                                         x.PatrName != null && x.PatrName.Contains(searchString)
                );
            }
        }

        private void OrderUserQuery(string sortPropName, ref IQueryable<User> query)
        {
            this.ThrowIfDisposed();
            if (sortPropName.IsNullOrEmpty())
                throw new ArgumentNullException(nameof(sortPropName));

            bool descending = false;
            if (sortPropName.EndsWith("_desc"))
            {
                sortPropName = sortPropName.Substring(0, sortPropName.Length - 5);
                descending = true;
            }

            // checking property name
            var check = ServiceHelpers.CheckIfPropertyExists(sortPropName, typeof(User));
            if (!check.Result)
            {
                query = query.OrderBy(x => x.UserName);
            }
            else
            {
                Expression<Func<User, object>> orderExpr = u => EF.Property<object>(u, sortPropName);
                query = descending ? query.OrderByDescending(orderExpr) : query.OrderBy(orderExpr);
            }
        }

        public async Task UpdateUserLastActivityDate(int userId)
        {
            this.ThrowIfDisposed();
            var user = await Users.FirstOrDefaultAsync(x => x.Id == userId);
            if (user == null)
            {
                return;
            }
            user.LastActivityDate = DateTimeOffset.Now;
            await Store.UpdateAsync(user, CancellationToken);
        }

        /// <summary>
        /// Updates user with new password and claims, ignores (!) claims values.
        /// </summary>
        public async Task<IdentityResult> UpdateUserPasswordClaims(User user, string newPassword,
            IEnumerable<Claim> newClaims)
        {
            //TODO tests?
            if (user == null) throw new ArgumentNullException(nameof(user));
            ThrowIfDisposed();
            
            //update password
            if (!newPassword.IsNullOrEmpty())
            {
                var passwordResult = await SetPasswordAsync(user, newPassword);
                if (!passwordResult.Succeeded)
                {
                    return passwordResult;
                }
            }

            var claims = newClaims.ToList();
            if (!claims.IsNullOrEmpty())
            {
                await UpdateUserRoles(user, claims);
                await UpdateUserClaims(user, claims);
            }

            return await UpdateAsync(user);
        }

        private async Task UpdateUserRoles(User user, IEnumerable<Claim> newClaims)
        {
            if (!(Store is IUserRoleStore<User> roleStore))
            {
                throw new NotSupportedException("Current UserStore doesn't implement IUserRoleStore");
            }

            var roles = newClaims.Where(x => x.Type == ClaimTypes.Role).Select(x => x.Value);
            var userRoleList = await roleStore.GetRolesAsync(user, CancellationToken);
            var compResult = ServiceHelpers.ChangeCompare(userRoleList, roles, x => x);
            var rolesToRemove = compResult.Deleted;
            var rolesToAdd = compResult.Inserted.Distinct();

            foreach (var role in rolesToRemove)
            {
                var normalizedRole = NormalizeKey(role);
                await roleStore.RemoveFromRoleAsync(user, normalizedRole, CancellationToken);
            }

            foreach (var role in rolesToAdd)
            {
                var normalizedRole = NormalizeKey(role);
                if (!await roleStore.IsInRoleAsync(user, normalizedRole, CancellationToken))
                {
                    await roleStore.AddToRoleAsync(user, normalizedRole, CancellationToken);
                }
            }
        }

        private async Task UpdateUserClaims(User user, IEnumerable<Claim> newClaims)
        {
            if (!(Store is IUserClaimStore<User> claimStore))
            {
                throw new NotSupportedException("Current UserStore doesn't implement IUserClaimStore");
            }

            var claims = newClaims.Where(x => x.Type != ClaimTypes.Role).ToList();
            var userClaims = await GetClaimsAsync(user);
            var claimCompResult = ServiceHelpers.ChangeCompare(userClaims, claims, x => x.Type);
            var claimsToRemove = claimCompResult.Deleted;
            var claimsToAdd = claimCompResult.Inserted.Distinct();

            await claimStore.RemoveClaimsAsync(user, claimsToRemove, CancellationToken);
            await claimStore.AddClaimsAsync(user, claimsToAdd, CancellationToken);
        }

        #region helpers

        private async Task<IdentityResult> SetPasswordAsync(User user, string newPassword)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            var passwordStore = Store as IUserPasswordStore<User>;
            if (passwordStore == null)
            {
                var error = new IdentityError
                {
                    Description = "Current UserStore doesn't implement IUserPasswordStore"
                };
                return IdentityResult.Failed(error);
            }

            var passwordValidateResult = await ValidatePasswordAsync(user, newPassword);
            if (!passwordValidateResult.Succeeded)
            {
                return passwordValidateResult;
            }

            var newPasswordHash = this.PasswordHasher.HashPassword(user, newPassword);
            await passwordStore.SetPasswordHashAsync(user, newPasswordHash, CancellationToken);
            await UpdateSecurityStampAsync(user);
            return IdentityResult.Success;
        }

        #endregion


        private bool _disposed = false;

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            if (!_disposed)
            {
                _disposed = true;
            }
        }
    }
}