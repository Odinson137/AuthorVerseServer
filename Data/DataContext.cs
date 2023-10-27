using AuthorVerseServer.Interfaces;
using AuthorVerseServer.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace AuthorVerseServer.Data
{
    public class DataContext : IdentityDbContext<User>
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {
            //ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
        }

        public DbSet<Book> Books { get; set; }
        public DbSet<BookChapter> BookChapters { get; set; }
        public DbSet<ChapterSection> ChapterSections { get; set; }
        public DbSet<UserSelectedBook> UserSelectedBooks { get; set; }
        public DbSet<SectionChoice> SectionChoices { get; set; }
        public DbSet<Friendship> Friendships{ get; set; }
        public DbSet<Genre> Genres { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<Note> Notes { get; set; }
        public DbSet<Character> Characters { get; set; }
        public DbSet<MicrosoftUser> MicrosoftUsers { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Friendship>().Ignore(u => u.User1);

            modelBuilder.Entity<User>().Ignore(u => u.Friendships);
            modelBuilder.Entity<BookChapter>().Ignore(u => u.Characters);

            modelBuilder.Entity<Friendship>().HasNoKey();

            modelBuilder.Entity<Book>()
                .HasMany(c => c.Comments)
                .WithOne()
                .HasForeignKey(c => c.BookId)
                .IsRequired(false);

            modelBuilder.Entity<BookChapter>()
                .HasMany(c => c.Notes)
                .WithOne()
                .HasForeignKey(c => c.BookChapterid)
                .IsRequired(false);

            modelBuilder.Entity<User>()
                .HasMany(u => u.UserSelectedBooks)
                .WithOne(u => u.User)
                .HasForeignKey(c => c.UserId)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Book>()
                .HasMany(b => b.UserSelectedBooks)
                .WithOne(b => b.Book)
                .HasForeignKey(b => b.BookId)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.Restrict);

        }
    }
}
