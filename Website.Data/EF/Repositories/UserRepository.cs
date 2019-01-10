using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Website.Core.Models.Domain;

namespace Website.Data.EF.Repositories
{
    public class UserRepository : UserStore<User, Role, WebsiteDbContext, int, UserClaim, UserRole, UserLogin, UserToken, RoleClaim>
    {
        public UserRepository(WebsiteDbContext context, IdentityErrorDescriber describer = null) : base(context,
            describer)
        {
            if (context == null) throw new ArgumentNullException(nameof(context));
        }

        public override async Task<IdentityResult> UpdateAsync(User user,
            CancellationToken cancellationToken = new CancellationToken())
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            var localDbUser = Context.Set<User>()
                .Local
                .FirstOrDefault(e => e.Id == user.Id);

            if (localDbUser != null)
            {
                Context.Entry(localDbUser).State = EntityState.Detached;//
            }
                Context.Attach(user);
                user.ConcurrencyStamp = Guid.NewGuid().ToString();
                Context.Update(user);
            

            try

            {
                await SaveChanges(cancellationToken);
            }
            catch (DbUpdateConcurrencyException ex)
            {
                return IdentityResult.Failed(ErrorDescriber.ConcurrencyFailure());
            }

            return IdentityResult.Success;
        }
    }
}