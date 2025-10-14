
using FinalCapstone.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace FinalCapstone.Data
{
    public class FinalCapstoneDbContext : IdentityDbContext<User>
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Entry> Entries { get; set; }
        public DbSet<EntryType> EntryTypes { get; set; }
        public DbSet<Emotion> Emotions { get; set; }
        public DbSet<EntryEmotion> EntryEmotions { get; set; }

        public FinalCapstoneDbContext(DbContextOptions<FinalCapstoneDbContext> options) : base(options)
        { }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<EntryType>()
            .HasIndex(et => et.TypeName)
            .IsUnique();


            modelBuilder.Entity<Emotion>()
            .HasIndex(e => e.EmotionName)
            .IsUnique();

            modelBuilder.Entity<EntryEmotion>()
            .HasKey(ee => new { ee.EntryId, ee.EmotionId });

            modelBuilder.Entity<Entry>()
            .HasOne(e => e.User)
            .WithMany(u => u.Entries)
            .HasForeignKey(e => e.UserId)
            .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Entry>()
            .HasOne(e => e.EntryType)
            .WithMany(et => et.Entries)
            .HasForeignKey(e => e.EntryTypeId)
            .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<EntryEmotion>()
            .HasOne(ee => ee.Entry)
            .WithMany(e => e.EntryEmotions)
            .HasForeignKey(ee => ee.EntryId)
            .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<EntryEmotion>()
            .HasOne(ee => ee.Emotion)
            .WithMany(e => e.EntryEmotions)
            .HasForeignKey(ee => ee.EmotionId)
            .OnDelete(DeleteBehavior.Cascade);
        }
    }
}