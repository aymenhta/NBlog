using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace NBlog.Api.Entities;

public class AppDbCtx : IdentityDbContext<AppUser, IdentityRole, string>
{
    public AppDbCtx(DbContextOptions<AppDbCtx> options) : base(options)
    {
    }

    // protected override void OnModelCreating(ModelBuilder modelBuilder)
    // {
    //     modelBuilder.Entity<User>()
    //         .HasIndex(u => new { u.Username, u.Email })
    //         .IsUnique();
    // }
#nullable disable
    public DbSet<Post> Posts { get; set; }
    public DbSet<Comment> Comments { get; set; }
    public DbSet<Review> Reviews { get; set; }
    public DbSet<Like> Likes { get; set; }
    public DbSet<Category> Categories { get; set; }
}