using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using NBlog.Api.Categories;
using NBlog.Api.Comments;
using NBlog.Api.Likes;
using NBlog.Api.Posts;
using NBlog.Api.Reviews;
using NBlog.Api.Users;

namespace NBlog.Api.Entities;

public class AppDbCtx(DbContextOptions<AppDbCtx> options) 
    : IdentityDbContext<AppUser, IdentityRole, string>(options)
{
#nullable disable
    public DbSet<Post> Posts { get; set; }
    public DbSet<Comment> Comments { get; set; }
    public DbSet<Review> Reviews { get; set; }
    public DbSet<Like> Likes { get; set; }
    public DbSet<Category> Categories { get; set; }
}