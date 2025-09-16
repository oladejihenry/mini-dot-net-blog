using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using mini_blog.Entities;


namespace mini_blog.Data;

public class BlogDbContext(DbContextOptions<BlogDbContext> options): DbContext(options)
{
    public DbSet<Post> Posts => Set<Post>();
    public DbSet<Comment> Comments  => Set<Comment>();
    
    public DbSet<User> Users => Set<User>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Post>()
            .HasMany(p => p.Comments)
            .WithOne(c => c.Post)
            .HasForeignKey(c => c.PostId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasIndex(u => u.Email).IsUnique();
            entity.HasIndex(u => u.Username).IsUnique();
            entity.Property(u => u.Email).IsRequired();
            entity.Property(u => u.passwordHash).IsRequired();
            entity.Property(u => u.Username).IsRequired();
        });

    }
}