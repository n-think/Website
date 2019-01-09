using System.Threading.Tasks.Sources;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Website.Core.Models;
using Website.Core.Models.Domain;

namespace Website.Data.EF
{
    public class WebsiteDbContext : IdentityDbContext<
        User, Role, int, UserClaim, UserRole, UserLogin, RoleClaim, UserToken>
    {
        public WebsiteDbContext()
        {
        }

        public WebsiteDbContext(DbContextOptions options)
            : base(options)
        {
            ChangeTracker.LazyLoadingEnabled = true;
        }

        public virtual DbSet<Category> Categories { get; set; }
        public virtual DbSet<DescriptionGroup> DescriptionGroups { get; set; }
        public virtual DbSet<Description> Descriptions { get; set; }
        public virtual DbSet<Product> Products { get; set; }
        public virtual DbSet<Image> Images { get; set; }
        public virtual DbSet<ImageBinData> ImageBinData { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            RenameIdentityAndFixNav(modelBuilder);

            modelBuilder.Entity<Product>(entity =>
            {
                entity.HasKey(x => x.Id);
                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(50);
                entity.Property(x => x.Timestamp)
                    .IsRowVersion();
                entity.HasIndex(e => e.Code)
                    .IsUnique();
                entity.Property(e => e.Price)
                    .HasColumnType("decimal(18,2)");
                entity.ToTable("Products", "Production");
            });

            modelBuilder.Entity<Image>(entity =>
            {
                entity.HasOne(e => e.Product)
                    .WithMany(e => e.Images)
                    .HasForeignKey(e => e.ProductId)
                    .OnDelete(DeleteBehavior.Cascade);
                entity.HasKey(x => x.Id);
                entity.Property(e => e.Format).HasMaxLength(10);
                entity.HasIndex(e => e.Name).IsUnique();
                entity.HasIndex(e => e.ThumbName).IsUnique();
                entity.ToTable("Images", "Production");
            });

            modelBuilder.Entity<ImageBinData>(entity =>
            {
                entity.HasKey(x => x.ImageId);
                entity.HasOne(x => x.ImageInfo)
                    .WithOne(x => x.BinData)
                    .HasForeignKey<ImageBinData>(x => x.ImageId)
                    .OnDelete(DeleteBehavior.Cascade);
                entity.ToTable("ImagesBinData", "Production");
            });

            modelBuilder.Entity<Category>(entity =>
            {
                entity.HasKey(x => x.Id);
                entity.Property(e => e.Name).HasMaxLength(50);
                entity.Property(e => e.Timestamp).IsRowVersion();
                entity.HasOne(e => e.Parent)
                    .WithMany(e => e.Children)
                    .HasForeignKey(e => e.ParentId);
                entity.ToTable("Categories", "Production");
            });
            
            modelBuilder.Entity<DescriptionGroup>(entity =>
            {
                entity.HasKey(x => x.Id);
                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(50);
                entity.Property(e => e.Timestamp)
                    .IsRowVersion();
                entity.HasOne(e => e.Parent)
                    .WithMany(e => e.Children)
                    .HasForeignKey(e=>e.ParentId)
                    .HasConstraintName("FK_DescriptionGroups_DescriptionGroups")
                    .OnDelete(DeleteBehavior.ClientSetNull);
                entity.ToTable("DescriptionGroups", "Production");
            });

            modelBuilder.Entity<Description>(entity =>
            {
                entity.HasKey(x => x.Id);
                entity.HasOne(d => d.DescriptionGroup)
                    .WithMany(p => p.Descriptions)
                    .HasForeignKey(d => d.DescriptionGroupId)
                    .HasConstraintName("FK_Descriptions_DescriptionGroups");
                entity.HasOne(d => d.Product)
                    .WithMany(p => p.Descriptions)
                    .HasForeignKey(d => d.ProductId)
                    .HasConstraintName("FK_Descriptions_Products");
                entity.ToTable("Descriptions", "Production");
            });

            //many to many ef core
            modelBuilder.Entity<ProductToCategory>(entity =>
            {
                entity.HasKey(pc => new {pc.ProductId, pc.CategoryId});
                entity.HasOne(x => x.Product)
                    .WithMany(x => x.ProductToCategory)
                    .HasForeignKey(x => x.ProductId);
                entity.HasOne(x => x.Category)
                    .WithMany(x => x.ProductCategory)
                    .HasForeignKey(x => x.CategoryId);
                entity.ToTable("ProductToCategory", "Production");
            });
        }

        private void RenameIdentityAndFixNav(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>(b =>
            {
                b.ToTable("Users", "Auth");
            });

            modelBuilder.Entity<UserClaim>(b =>
            {
                b.ToTable("UserClaims", "Auth");
            });

            modelBuilder.Entity<UserLogin>(b =>{
               
                b.ToTable("UserLogins", "Auth");
            });

            modelBuilder.Entity<UserToken>(b =>
            {
                b.ToTable("UserTokens", "Auth");
            });

            modelBuilder.Entity<Role>(b =>
            {
                b.ToTable("Roles", "Auth");
            });

            modelBuilder.Entity<RoleClaim>(b =>
            {
                b.ToTable("RoleClaims", "Auth");
            });
            
            modelBuilder.Entity<User>()
                .HasMany(e => e.Claims)
                .WithOne(x=>x.User)
                .HasForeignKey(e => e.UserId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<User>()
                .HasMany(e => e.Logins)
                .WithOne(x=>x.User)
                .HasForeignKey(e => e.UserId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<UserRole>(entity =>
            {
                entity.HasKey(x => new {x.UserId, x.RoleId});
                entity.HasOne(x => x.Role)
                    .WithMany(x => x.UserRoles)
                    .HasForeignKey(x => x.RoleId)
                    .OnDelete(DeleteBehavior.Cascade);
                entity.HasOne(x => x.User)
                    .WithMany(x => x.UserRoles)
                    .HasForeignKey(x => x.UserId)
                    .OnDelete(DeleteBehavior.Cascade);
                entity.ToTable("UserRoles", "Auth");
            });

            modelBuilder.Entity<User>()
                .HasMany(e => e.Tokens)
                .WithOne(x=>x.User)
                .HasForeignKey(e => e.UserId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}