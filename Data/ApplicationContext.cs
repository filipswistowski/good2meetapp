using Microsoft.EntityFrameworkCore;

namespace activitiesapp.Models
{
    public class ApplicationContext : DbContext
    {
        public ApplicationContext(DbContextOptions<ApplicationContext> options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Participants> Participants { get; set; }
        public DbSet<Event> Events { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<User>()
                .ToTable("users");
            builder.Entity<User>()
                .HasKey(s => s.UserId);
            //-----------------UserType-------------------
            builder.Entity<UserType>()
                .ToTable("user_type");
            builder.Entity<UserType>().Property(s => s.UserTypeId)
                .HasColumnName("user_type_id");
            builder.Entity<UserType>().Property(s => s.Type)
                .HasColumnName("type");

            //-----------User-----------
            builder.Entity<User>()
                .HasOne(s => s.UserTypes)
                .WithMany(s => s.Users)
                .HasForeignKey(s => s.UserTypeId);

            //-----------Participants-------------
            builder.Entity<Participants>()
                .HasKey(pt => new { pt.UserId, pt.EventId });

            builder.Entity<Participants>()
                .HasOne(bc => bc.User)
                .WithMany(b => b.Participants)
                .HasForeignKey(s => s.UserId)
                .OnDelete(DeleteBehavior.ClientCascade);

            builder.Entity<Participants>()
                .HasOne(bc => bc.Event)
                .WithMany(c => c.Participants)
                .HasForeignKey(s => s.EventId);

            //-----------Event---------------
            builder.Entity<Event>()
                .HasMany(s => s.Participants)
                .WithOne(s => s.Event)
                .HasForeignKey(s => s.EventId);

            builder.Entity<Event>()
                .HasOne(s => s.Category)
                .WithMany(c => c.Events)
                .HasForeignKey(x => x.CategoryId);

            //---------Category-------------
            builder.Entity<Category>()
                .HasMany(s => s.Events)
                .WithOne(s => s.Category)
                .HasForeignKey(s => s.CategoryId);
        }

        public DbSet<activitiesapp.Models.Category> Category { get; set; }
    }
}