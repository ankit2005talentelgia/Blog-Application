using Microsoft.EntityFrameworkCore;
using System.Data;
using Travel_Blogging.Models;

namespace Travel_Blogging.Data
{
    public class ApplicationDbContext:DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) 
        {

        }

        // add both the dbset i.e user and post
        public DbSet<UserModel> Users { get; set; }
        public DbSet<PostModel> Posts { get; set; }
        public DbSet<CommentModel> Comments { get; set; }
    }
}
