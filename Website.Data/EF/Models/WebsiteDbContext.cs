using Microsoft.EntityFrameworkCore;

namespace Website.Data.EF.Models
{
    public partial class WebsiteDbContext : DbContext
    {
        public WebsiteDbContext()
        {
        }

        public WebsiteDbContext(DbContextOptions<WebsiteDbContext> options)
            : base(options)
        {
        }

        public virtual DbSet<User> Users { get; set; }
        public virtual DbSet<UserClaim> UserClaims { get; set; }
        public virtual DbSet<UserLogin> UserLogins { get; set; }
        public virtual DbSet<UserToken> UserTokens { get; set; }
        public virtual DbSet<UserRole> UserRoles { get; set; }
        public virtual DbSet<Role> Roles { get; set; }
        public virtual DbSet<RoleClaim> RoleClaims { get; set; }
        public virtual DbSet<Category> Categories { get; set; }
        public virtual DbSet<UserProfile> UserProfiles { get; set; }
        public virtual DbSet<DescriptionGroup> DescriptionGroups { get; set; }
        public virtual DbSet<Description> Descriptions { get; set; }
        public virtual DbSet<Product> Products { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            FromIdentityBuilder(modelBuilder);

            modelBuilder.Entity<Category>(entity =>
            {
                entity.Property(e => e.Name).HasMaxLength(50);
            });

            modelBuilder.Entity<DescriptionGroup>(entity =>
            {
                entity.Property(e => e.Description).HasMaxLength(50);

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(50);
            });

            modelBuilder.Entity<Description>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.Name).HasMaxLength(50);

                entity.HasOne(d => d.DescriptionGroupNavigation)
                    .WithMany(p => p.Descriptions)
                    .HasForeignKey(d => d.DescriptionGroup)
                    .OnDelete(DeleteBehavior.SetNull)
                    .HasConstraintName("FK_Descriptions_DescriptionGroups");

                entity.HasOne(d => d.Product)
                    .WithMany(p => p.Descriptions)
                    .HasForeignKey(d => d.ProductId)
                    .OnDelete(DeleteBehavior.SetNull)
                    .HasConstraintName("FK_Descriptions_Products");
            });

            modelBuilder.Entity<Product>(entity =>
            {
                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(x => x.Timestamp)
                    .IsRowVersion();
            });

            modelBuilder.Entity<User>(entity =>
            {
                entity.HasOne(x => x.UserProfile)
                    .WithOne(x => x.User)
                    .HasForeignKey<UserProfile>(x => x.Id);
            });

            modelBuilder.Entity<UserProfile>(entity =>
            {
                entity.Property(x => x.Timestamp)
                    .IsRowVersion();
            });

            //many to many ef core
            modelBuilder.Entity<ProductToCategory>(entity =>
            {
                entity.HasKey(pc => new { pc.ProductId, pc.CategoryId });

                entity.HasOne(x => x.Product)
                    .WithMany(x => x.ProductCategory)
                    .HasForeignKey(x => x.ProductId);

                entity.HasOne(x => x.Category)
                    .WithMany(x => x.ProductCategory)
                    .HasForeignKey(x => x.CategoryId);
            });
        }

        private void FromIdentityBuilder(ModelBuilder modelBuilder)
        {
            //gutted encryptPersonalData

            modelBuilder.Entity<User>(b =>
            {
                b.HasKey(u => u.Id);
                b.HasIndex(u => u.NormalizedUserName).HasName("UserNameIndex").IsUnique();
                b.HasIndex(u => u.NormalizedEmail).HasName("EmailIndex");
                b.ToTable("Users");
                b.Property(u => u.ConcurrencyStamp).IsConcurrencyToken();

                b.Property(u => u.UserName).HasMaxLength(256).IsRequired();
                b.Property(u => u.NormalizedUserName).HasMaxLength(256);
                b.Property(u => u.Email).HasMaxLength(256).IsRequired(); ;
                b.Property(u => u.NormalizedEmail).HasMaxLength(256);

                b.HasIndex(u => u.Email).IsUnique();
                b.HasIndex(u => u.UserName).IsUnique();

                b.HasMany<UserClaim>().WithOne().HasForeignKey(uc => uc.UserId).IsRequired();
                b.HasMany<UserLogin>().WithOne().HasForeignKey(ul => ul.UserId).IsRequired();
                b.HasMany<UserToken>().WithOne().HasForeignKey(ut => ut.UserId).IsRequired();
            });

            modelBuilder.Entity<UserClaim>(b =>
            {
                b.HasKey(uc => uc.Id);
                b.ToTable("UserClaims");
                b.HasOne(x => x.User).WithMany(x => x.Claims).HasForeignKey(x => x.UserId);
            });

            modelBuilder.Entity<UserLogin>(b =>
            {
                b.HasKey(l => new { l.LoginProvider, l.ProviderKey });
                b.ToTable("UserLogins");
                b.HasOne(x => x.User).WithMany(x => x.Logins).HasForeignKey(x => x.UserId);
            });

            modelBuilder.Entity<UserToken>(b =>
            {
                b.HasKey(t => new { t.UserId, t.LoginProvider, t.Name });
                b.ToTable("UserTokens");
                b.HasOne(x => x.User).WithMany(x => x.Tokens).HasForeignKey(x => x.UserId);
            });

            modelBuilder.Entity<User>(b =>
            {
                b.HasMany<UserRole>().WithOne().HasForeignKey(ur => ur.UserId).IsRequired();
            });

            modelBuilder.Entity<Role>(b =>
            {
                b.HasKey(r => r.Id);
                b.HasIndex(r => r.NormalizedName).HasName("RoleNameIndex").IsUnique();
                b.ToTable("Roles");
                b.Property(r => r.ConcurrencyStamp).IsConcurrencyToken();

                b.Property(u => u.Name).HasMaxLength(256);
                b.Property(u => u.NormalizedName).HasMaxLength(256);

                b.HasMany<UserRole>().WithOne().HasForeignKey(ur => ur.RoleId).IsRequired();
                b.HasMany<RoleClaim>().WithOne().HasForeignKey(rc => rc.RoleId).IsRequired();
            });

            modelBuilder.Entity<RoleClaim>(b =>
            {
                b.HasKey(rc => rc.Id);
                b.ToTable("RoleClaims");
            });

            modelBuilder.Entity<UserRole>(b =>
            {
                b.HasKey(r => new { r.UserId, r.RoleId });
                b.ToTable("UserRoles");

                b.HasOne(x => x.User)
                    .WithMany(x => x.UserRoles)
                    .HasForeignKey(x => x.UserId);

                b.HasOne(x => x.Role)
                    .WithMany(x => x.UserRoles)
                    .HasForeignKey(x => x.RoleId);
            });
        }
    }
}
