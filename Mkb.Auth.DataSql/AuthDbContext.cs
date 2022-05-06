using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Mkb.Auth.Contracts.Dtos;

namespace Mkb.Auth.DataSql
{
    public class AuthDbContext : DbContext
    {
        public AuthDbContext()
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https: //go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
                optionsBuilder.UseSqlServer("Data Source=127.0.0.1;Database=Auth;User Id=sa;Password=A1234567a;");
            }
        }

        public AuthDbContext(DbContextOptions<AuthDbContext> options)
            : base(options)
        {
        }

        internal virtual DbSet<User> Users { get; set; }
        internal virtual DbSet<Role> Roles { get; set; }
        internal virtual DbSet<UserRoles> UserRoles { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>(user =>
            {
                user.Property(x => x.Email)
                    .HasMaxLength(200)
                    .IsRequired()
                    .IsUnicode(false);

                user.Property(x => x.Salt).HasMaxLength(20).IsUnicode(false).IsRequired(true);

                user.Property(f => f.UserId).IsRequired().HasDefaultValueSql("NEWID()");
                user.Property(f => f.FirstName).HasMaxLength(100).IsRequired();
                user.Property(f => f.LastName).HasMaxLength(100).IsRequired();
                user.Property(f => f.Password).IsUnicode(false).HasMaxLength(64).IsRequired();
                user.HasMany(f => f.UserRoles).WithOne(x => x.User)
                    .HasForeignKey(x => x.UserId)
                    .HasConstraintName("FK_Users_UserRoles");
            });

            modelBuilder.Entity<Role>(Role =>
            {
                Role.Property(x => x.Description)
                    .HasMaxLength(55)
                    .IsRequired()
                    .IsUnicode(false);

                Role.HasMany(f => f.UserRoles).WithOne(f => f.Role)
                    .HasForeignKey(x => x.RoleId)
                    .HasConstraintName("FK_Roles_UserRoles");
            });

            modelBuilder.Entity<UserRoles>(UserRoles =>
            {
                UserRoles.HasOne(f => f.User).WithMany(x => x.UserRoles)
                    .HasForeignKey(x => x.UserId)
                    .HasConstraintName("FK_Users_UserRoles");
                UserRoles.HasOne(f => f.Role).WithMany(x => x.UserRoles)
                    .HasForeignKey(x => x.RoleId)
                    .HasConstraintName("FK_Roles_UserRoles");
            });
        }
    }

    public class Role : RoleDto
    {
        public int Id { get; set; }
        public virtual ICollection<UserRoles> UserRoles { get; set; }
    }

    public class UserRoles
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int RoleId { get; set; }
        public virtual Role Role { get; set; }
        public virtual User User { get; set; }
    }

    public class User : UserDto
    {
        public int Id { get; set; }
        public virtual ICollection<UserRoles> UserRoles { get; set; }
    }
}