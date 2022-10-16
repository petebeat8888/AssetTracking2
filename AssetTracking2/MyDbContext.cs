using Microsoft.EntityFrameworkCore;
using DbContext = Microsoft.EntityFrameworkCore.DbContext;



namespace AppEFDBClasses
{
    internal class MyDbContext : DbContext
    {
        
        string connectionString = "Data Source=DESKTOP-6HHA63N;DataBase=AssetTrackingDb;Trusted_Connection=True;";

        private DbContextOptions<MyDbContext>? Options;
        
        public Microsoft.EntityFrameworkCore.DbSet<ComputerDB>? Computers { get; set; }
        public Microsoft.EntityFrameworkCore.DbSet<PhoneDB>? Phones { get; set; }
        
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<ComputerDB>().ToTable("Computers");
            modelBuilder.Entity<PhoneDB>().ToTable("Phones");
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            // we tell app yo use connection string
            optionsBuilder.UseSqlServer(connectionString);
        }

    }

}
