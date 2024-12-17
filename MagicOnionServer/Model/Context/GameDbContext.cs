using MagicOnionServer.Model.Entity;
using Microsoft.EntityFrameworkCore;

namespace MagicOnionServer.Model.Context
{
    public class GameDbContext : DbContext
    {
        public DbSet<User> Users { get; set; }

#if DEBUG
        readonly string connectionString = "server=localhost;database=realtime_game;user=jobi;password=jobi;";
#else
        readonly string connectionString = "server=db-ge-14.mysql.database.azure.com;database=realtime_game;user=student;password=Yoshidajobi2023;";
#endif
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseMySql(connectionString, new MySqlServerVersion(new Version (8,0)));
        }
    }
}
