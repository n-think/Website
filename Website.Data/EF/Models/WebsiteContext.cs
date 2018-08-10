using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Website.Data.EF.Models
{
    public partial class WebsiteContext : IdentityDbContext<ApplicationUser>
    {
        public WebsiteContext()
        {
        }

        public WebsiteContext(DbContextOptions<WebsiteContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Category> Categories { get; set; }
        public virtual DbSet<ClientProfile> ClientProfiles { get; set; }
        public virtual DbSet<DescriptionGroup> DescriptionGroups { get; set; }
        public virtual DbSet<Description> Descriptions { get; set; }
        public virtual DbSet<ProductImage> ProductImages { get; set; }
        public virtual DbSet<Product> Products { get; set; }
        //public virtual DbSet<ProductToCategory> ProductToCategories { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            // Customize the ASP.NET Identity model and override the defaults if needed.
            // For example, you can rename the ASP.NET Identity table names and more.
            // Add your customizations after calling base.OnModelCreating(builder);

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

            modelBuilder.Entity<ProductImage>(entity =>
            {
                entity.Property(e => e.Description).HasMaxLength(50);

                entity.Property(e => e.Name).HasMaxLength(50);

                entity.HasOne(d => d.Product)
                    .WithMany(p => p.ProductImages)
                    .HasForeignKey(d => d.ProductId)
                    .OnDelete(DeleteBehavior.SetNull)
                    .HasConstraintName("FK_ProductImages_Products");
            });

            modelBuilder.Entity<Product>(entity =>
            {
                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(50);
            });

            modelBuilder.Entity<ApplicationUser>()
                .HasOne(x => x.ClientProfile)
                .WithOne(x => x.User)
                .HasForeignKey<ClientProfile>(x => x.Id);

            //many to many ef core woohoo
            modelBuilder.Entity<ProductToCategory>()
                .HasKey(pc => new {pc.ProductId, pc.CategoryId });

            modelBuilder.Entity<ProductToCategory>()
                .HasOne(x => x.Product)
                .WithMany(x => x.ProductCategory)
                .HasForeignKey(x => x.ProductId);

            modelBuilder.Entity<ProductToCategory>()
                .HasOne(x => x.Category)
                .WithMany(x => x.ProductCategory)
                .HasForeignKey(x=> x.CategoryId);

        }
    }
}
