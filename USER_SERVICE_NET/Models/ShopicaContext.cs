using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace USER_SERVICE_NET.Models
{
    public partial class ShopicaContext : DbContext
    {
        public ShopicaContext()
        {
        }

        public ShopicaContext(DbContextOptions<ShopicaContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Account> Account { get; set; }
        public virtual DbSet<Customer> Customer { get; set; }
        public virtual DbSet<Seller> Seller { get; set; }

//        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
//        {
//            if (!optionsBuilder.IsConfigured)
//            {
//#warning To protect potentially sensitive information in your connection string, you should move it out of source code. See http://go.microsoft.com/fwlink/?LinkId=723263 for guidance on storing connection strings.
//                optionsBuilder.UseMySQL("Server=remotemysql.com;Database=gBfYRbGtXo;Uid=gBfYRbGtXo;Pwd=E9W73GQRXJ;");
//            }
//        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Account>(entity =>
            {
                entity.ToTable("account");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasColumnType("int(11)");

                entity.Property(e => e.ImageUrl)
                    .HasColumnName("image_url")
                    .HasMaxLength(100);

                entity.Property(e => e.IsActive)
                    .HasColumnName("is_active")
                    .HasColumnType("tinyint(4)");

                entity.Property(e => e.Password)
                    .HasColumnName("password")
                    .HasMaxLength(45);

                entity.Property(e => e.Type)
                    .HasColumnName("type")
                    .HasColumnType("int(11)")
                    .HasComment("'0-ADMIN','1-SELLER','2-CUSTOMER'");

                entity.Property(e => e.Username)
                    .HasColumnName("username")
                    .HasMaxLength(45);

                entity.Property(e => e.Created_at)
                    .HasColumnName("created_at")
                    .HasColumnType("datetime");

                entity.Property(e => e.Updated_at)
                      .HasColumnName("updated_at")
                      .HasColumnType("datetime");
            });

            modelBuilder.Entity<Customer>(entity =>
            {
                entity.HasKey(e => new { e.Id, e.AccountId })
                    .HasName("PRIMARY");

                entity.ToTable("customer");

                entity.HasIndex(e => e.AccountId)
                    .HasName("fk_customer_Account1_idx");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasColumnType("int(11)")
                    .ValueGeneratedOnAdd();

                entity.Property(e => e.AccountId)
                    .HasColumnName("account_id")
                    .HasColumnType("int(11)");

                entity.Property(e => e.Address)
                    .HasColumnName("address")
                    .HasMaxLength(100);

                entity.Property(e => e.CustomerName)
                    .HasColumnName("customer_name")
                    .HasMaxLength(100);

                entity.Property(e => e.Email)
                    .HasColumnName("email")
                    .HasMaxLength(45);

                entity.Property(e => e.Gender)
                    .HasColumnName("gender")
                    .HasColumnType("int(11)")
                    .HasComment("'0-MALE','1-FEMALE'");

                entity.Property(e => e.Phone)
                    .HasColumnName("phone")
                    .HasMaxLength(12);

                entity.Property(e => e.Created_at)
                    .HasColumnName("created_at")
                    .HasColumnType("datetime");

                entity.Property(e => e.Updated_at)
                      .HasColumnName("updated_at")
                      .HasColumnType("datetime");

                entity.HasOne(d => d.Account)
                    .WithMany(p => p.Customer)
                    .HasForeignKey(d => d.AccountId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_customer_Account1");
            });

            modelBuilder.Entity<Seller>(entity =>
            {
                entity.HasKey(e => new { e.Id, e.StoreId, e.AccountId })
                    .HasName("PRIMARY");

                entity.ToTable("seller");

                entity.HasIndex(e => e.AccountId)
                    .HasName("fk_staff_Account1_idx");

                entity.HasIndex(e => e.StoreId)
                    .HasName("fk_Staff_Store1_idx");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasColumnType("int(11)")
                    .ValueGeneratedOnAdd();

                entity.Property(e => e.StoreId)
                    .HasColumnName("store_id")
                    .HasColumnType("int(11)");

                entity.Property(e => e.AccountId)
                    .HasColumnName("account_id")
                    .HasColumnType("int(11)");

                entity.Property(e => e.Address)
                    .HasColumnName("address")
                    .HasMaxLength(100);

                entity.Property(e => e.Email)
                    .HasColumnName("email")
                    .HasMaxLength(45);

                entity.Property(e => e.Gender)
                    .HasColumnName("gender")
                    .HasColumnType("int(11)")
                    .HasComment("'0-MALE','1-FEMALE'");

                entity.Property(e => e.Phone)
                    .HasColumnName("phone")
                    .HasMaxLength(12);

                entity.Property(e => e.SellerName)
                    .HasColumnName("seller_name")
                    .HasMaxLength(45);

                entity.Property(e => e.Updated_at)
                      .HasColumnName("updated_at")
                      .HasColumnType("datetime");

                entity.Property(e => e.Created_at)
                      .HasColumnName("created_at")
                      .HasColumnType("datetime");

                entity.HasOne(d => d.Account)
                    .WithMany(p => p.Seller)
                    .HasForeignKey(d => d.AccountId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_staff_Account1");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
