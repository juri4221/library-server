using BookManagement.Models;
using BookManagement.Models.Auth;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace BookManagement.Infrastructure
{
    public class BookDbContext : IdentityDbContext<User, Role, Guid>
    {
        public BookDbContext(DbContextOptions<BookDbContext> context) : base(context)
        {

        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<Role>().HasData(new Role
            {
                ConcurrencyStamp = Guid.Parse("148c9f02-840f-4a11-8f20-c0fa102d8f33").ToString(),
                Id = Guid.Parse("779fea26-3ddf-4d66-abc9-47d87fbbcc81"),
                Name = "Admin",
                NormalizedName = "Admin"
            });
            builder.Entity<User>().HasData(new User
            {
                ConcurrencyStamp = Guid.NewGuid().ToString(),
                Email = "admin@gmail.com",
                EmailConfirmed = true,
                Id = Guid.Parse("a813b798-93fd-4d83-ac71-2619773b0e37"),
                LockoutEnabled = false,
                LockoutEnd = null,
                NormalizedEmail = "admin@gmail.com",
                NormalizedUserName = "admin",
                PasswordHash = "AQAAAAIAAYagAAAAEJ8JngvgVf1DYgIIkFpTQiCCglVfpS1WJf4/iNiWyJy8ZoO476H7EyUi8LZwUMXO6g==",
                PhoneNumber = "0",
                PhoneNumberConfirmed = true,
                SecurityStamp = "",
                TwoFactorEnabled = false,
                UserName = "admin",
            });
            builder.Entity<UserRole>().HasData(new UserRole
            {
                RoleId = Guid.Parse("779fea26-3ddf-4d66-abc9-47d87fbbcc81"),
                UserId = Guid.Parse("a813b798-93fd-4d83-ac71-2619773b0e37"),
            });
            base.OnModelCreating(builder);
        }
        
        public DbSet<Author> Authors { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Book> Books { get; set; }
    }
}
