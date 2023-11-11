using AuthorVerseServer.Interfaces;
using AuthorVerseServer.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;

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
        public DbSet<Friendship> Friendships { get; set; }
        public DbSet<Genre> Genres { get; set; }
        public DbSet<Tag> Tags { get; set; }
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
                .HasMany(b => b.Genres)
                .WithMany(g => g.Books)
                .UsingEntity<BookGenre>(
                    j => j
                        .HasOne(bg => bg.Genre)
                        .WithMany()
                        .HasForeignKey(bg => bg.GenreId),
                    j => j
                        .HasOne(bg => bg.Book)
                        .WithMany()
                        .HasForeignKey(bg => bg.BookId),
                    j =>
                    {
                        j.HasKey(t => new { t.BookId, t.GenreId });
                        j.ToTable("BookGenre"); 
                    });

            modelBuilder.Entity<Book>()
                .HasMany(b => b.Tags)
                .WithMany(g => g.Books)
                .UsingEntity<BookTag>(
                    j => j
                        .HasOne(bg => bg.Tag)
                        .WithMany()
                        .HasForeignKey(bg => bg.TagId),
                    j => j
                        .HasOne(bg => bg.Book)
                        .WithMany()
                        .HasForeignKey(bg => bg.BookId),
                    j =>
                    {
                        j.HasKey(t => new { t.BookId, t.TagId });
                        j.ToTable("BookTag");
                    });


            modelBuilder.Entity<Book>()
                .HasMany(c => c.Comments)
                .WithOne(c => c.Book)
                .HasForeignKey(c => c.BookId)
                .IsRequired(false);

            modelBuilder.Entity<User>()
                .HasMany(c => c.Comments)
                .WithOne(c => c.Commentator)
                .HasForeignKey(c => c.CommentatorId)
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

            modelBuilder.Entity<Book>()
                .HasMany(b => b.Characters)
                .WithOne(b => b.Book)
                .HasForeignKey(b => b.BookId)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Comment>()
                .HasMany(b => b.commentRatings)
                .WithOne()
                .HasForeignKey(b => b.CommentId)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.Restrict);




            modelBuilder.Entity<Book>()
                .HasIndex(g => g.BookId);
            modelBuilder.Entity<Book>()
                .HasIndex(g => g.Title);
            modelBuilder.Entity<Book>()
                .HasIndex(g => g.PublicationData);
            modelBuilder.Entity<Book>()
                .HasIndex(g => g.Permission);
            modelBuilder.Entity<Book>()
                .HasIndex(g => g.AuthorId);

            modelBuilder.Entity<Genre>()
                .HasIndex(g => g.GenreId);

            modelBuilder.Entity<Tag>()
                .HasIndex(g => g.TagId);

            modelBuilder.Entity<BookChapter>()
                .HasIndex(g => g.BookChapterId);
            modelBuilder.Entity<BookChapter>()
                .HasIndex(g => g.BookId);
            modelBuilder.Entity<BookChapter>()
                .HasIndex(g => g.PublicationData);

            modelBuilder.Entity<BookTag>()
                .HasIndex(g => g.BookId);
            modelBuilder.Entity<BookTag>()
                .HasIndex(g => g.TagId);

            modelBuilder.Entity<BookGenre>()
                .HasIndex(g => g.BookId);
            modelBuilder.Entity<BookGenre>()
                .HasIndex(g => g.GenreId);

            modelBuilder.Entity<ChapterSection>()
                .HasIndex(g => g.SectionId);
            modelBuilder.Entity<ChapterSection>()
                .HasIndex(g => g.BookChapterId);
            modelBuilder.Entity<ChapterSection>()
                .HasIndex(g => g.Number);

            modelBuilder.Entity<Character>()
                .HasIndex(g => g.CharacterId);
            modelBuilder.Entity<Character>()
                .HasIndex(g => g.BookId);
            modelBuilder.Entity<Character>()
                .HasIndex(g => g.BookChapterId);

            modelBuilder.Entity<Comment>()
                .HasIndex(g => g.CommentId);
            modelBuilder.Entity<Comment>()
                .HasIndex(g => g.CommentatorId);
            modelBuilder.Entity<Comment>()
                .HasIndex(g => g.BookId);
            modelBuilder.Entity<Comment>()
                .HasIndex(g => g.Permission);

            modelBuilder.Entity<Friendship>()
                .HasIndex(g => g.User1Id);
            modelBuilder.Entity<Friendship>()
                .HasIndex(g => g.User2Id);
            modelBuilder.Entity<Friendship>()
                .HasIndex(g => g.Status);

            modelBuilder.Entity<MicrosoftUser>()
                .HasIndex(g => g.Id);
            modelBuilder.Entity<MicrosoftUser>()
                .HasIndex(g => g.AzureName);

            modelBuilder.Entity<Note>()
                .HasIndex(g => g.NoteId);
            modelBuilder.Entity<Note>()
                .HasIndex(g => g.UserId);
            modelBuilder.Entity<Note>()
                .HasIndex(g => g.BookChapterid);
            modelBuilder.Entity<Note>()
                .HasIndex(g => g.NoteCreatedDateTime);
            modelBuilder.Entity<Note>()
                .HasIndex(g => g.Permission);

            modelBuilder.Entity<SectionChoice>()
                .HasIndex(g => g.SectionChoiceId);
            modelBuilder.Entity<SectionChoice>()
                .HasIndex(g => g.ChapterSectionId);

            modelBuilder.Entity<UserSelectedBook>()
                .HasIndex(g => g.UserBookId);
            modelBuilder.Entity<UserSelectedBook>()
                .HasIndex(g => g.UserId);
            modelBuilder.Entity<UserSelectedBook>()
                .HasIndex(g => g.BookId);
        }
    }
}
