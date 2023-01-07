using Microsoft.EntityFrameworkCore;

namespace QuestRoom.Models

{
    public class ApplicationContext:DbContext
    {
        public DbSet<Room> Rooms { get; set; }

        public DbSet<Image> Images { get; set; }

        public DbSet<Phone> Phones { get; set; }

        public ApplicationContext(DbContextOptions<ApplicationContext> options) : base(options)
        {
            //Database.EnsureDeleted();
            Database.EnsureCreated();
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            Console.WriteLine(Directory.GetCurrentDirectory());
            optionsBuilder.UseSqlServer(@$"Server=(localdb)\mssqllocaldb;AttachDBFileName={Directory.GetCurrentDirectory()}\QuestRoomDB.mdf;Trusted_Connection=True;");
        }
    }
}
