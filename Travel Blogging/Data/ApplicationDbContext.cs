using Microsoft.EntityFrameworkCore;
using System.Data;
using Travel_Blogging.Models;

namespace Travel_Blogging.Data
{
    public class ApplicationDbContext:DbContext
    {
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<PostModel>()
                .HasOne(p => p.Author)
                .WithMany(u => u.Posts)
                .HasForeignKey(p => p.AuthorId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<ReviewModel>()
                .HasOne(c => c.User)
                .WithMany(u=>u.Comments)
                .HasForeignKey(c => c.UserId)
                .OnDelete(DeleteBehavior.NoAction);


            // ENUM → STRING conversion in post model
            modelBuilder.Entity<PostModel>()
                .Property(p => p.Status)
                .HasConversion<string>();

            // global soft delete filters
            modelBuilder.Entity<PostModel>()
                .HasQueryFilter(p => !p.IsDeleted);

            modelBuilder.Entity<UserModel>()
                .HasQueryFilter(u => !u.IsDeleted);

            modelBuilder.Entity<ReviewModel>()
                .HasQueryFilter(c => !c.IsDeleted);
        }
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) 
        {

        }

        // add both the dbset i.e user and post
        public DbSet<UserModel> Users { get; set; }
        public DbSet<PostModel> Posts { get; set; }
        public DbSet<ReviewModel> Comments { get; set; }
    }
}
