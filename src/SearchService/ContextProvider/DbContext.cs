using Microsoft.EntityFrameworkCore;
using MongoDB.Driver;
using MongoDB.EntityFrameworkCore.Extensions;
using SearchService.Models;

namespace SearchService.ContextProvider
{
    // this will return me a Db context for Search Db
    public class searchDbContext : DbContext
    {

        public searchDbContext(DbContextOptions options) : base(options) {}

        public DbSet<Item> Items { get; set; }

        public static searchDbContext Create(IMongoDatabase database) =>
            new(new DbContextOptionsBuilder<searchDbContext>()
                .UseMongoDB(database.Client, database.DatabaseNamespace.DatabaseName)
                .Options);
        
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Item>().ToCollection("Items");
        }
   

    }
}
