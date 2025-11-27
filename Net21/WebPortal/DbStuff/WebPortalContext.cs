using System.Reflection;
using Microsoft.EntityFrameworkCore;
using WebPortal.DbStuff.Models;
using WebPortal.DbStuff.Models.CompShop;
using WebPortal.DbStuff.Models.CompShop.Devices;
using WebPortal.DbStuff.Models.Notifications;

namespace WebPortal.DbStuff
{
    public class WebPortalContext : DbContext
    {
        public WebPortalContext(DbContextOptions<WebPortalContext> options)
            : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<Girl> Girls { get; set; }
        public DbSet<Anime> Animes { get; set; }


        /* CompShop */
        public DbSet<Device> Devices { get; set; }
        public DbSet<Computer> Computers { get; set; }
        public DbSet<Category> Categoryes { get; set; }
        public DbSet<TypeDevice> TypeDevices { get; set; }
        public DbSet<News> News { get; set; }


        public DbSet<Notification> Notifications { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder
                .Entity<User>()
                .HasMany(user => user.FavoriteGirls)
                .WithMany(girl => girl.UserWhoAddToFavorite);

            modelBuilder
                .Entity<User>()
                .HasMany(user => user.CreatedGirls)
                .WithOne(girl => girl.Author)
                .OnDelete(DeleteBehavior.NoAction);


            modelBuilder
                .Entity<Computer>()
                .HasOne(comp => comp.Device)
                .WithOne(device => device.Computer);

            modelBuilder.Entity<Girl>().HasMany(x => x.Animes).WithMany(x => x.Characters);

            modelBuilder.Entity<Girl>().HasMany(x => x.Festivals).WithMany(x => x.Girls);

            modelBuilder
                .Entity<Notification>()
                .HasOne(x => x.Author)
                .WithMany(x => x.NotificationCreatedByMe)
                .OnDelete(DeleteBehavior.NoAction);
            modelBuilder
                .Entity<Notification>()
                .HasMany(x => x.UserWhoViewedIt)
                .WithMany(x => x.ViewedNotification);

            base.OnModelCreating(modelBuilder);

            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        }
    }
}
