using AuthorVerseServer.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;
using System.Data;

namespace AuthorVerseServer.Data
{
    public class DataContext : IdentityDbContext<User>
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {
            var databaseCreator = Database.GetService<IDatabaseCreator>() as RelationalDatabaseCreator;

            if (databaseCreator != null)
            {
                if (!databaseCreator.CanConnect()) databaseCreator.Create();
                if (!databaseCreator.HasTables()) databaseCreator.CreateTables();
            }
        }


        public DbSet<Book> Books { get; set; }
        public DbSet<BookChapter> BookChapters { get; set; }
        public DbSet<ChapterSection> ChapterSections { get; set; }
        public DbSet<UserSelectedBook> UserSelectedBooks { get; set; }
        public DbSet<SectionChoice> SectionChoices { get; set; }
        public DbSet<Friendship> Friendships { get; set; }
        public DbSet<Genre> Genres { get; set; }
        public DbSet<Tag> Tags { get; set; }
        public DbSet<CommentRating> CommentRatings { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<Note> Notes { get; set; }
        public DbSet<CommentBase> CommentBases { get; set; }
        public DbSet<Character> Characters { get; set; }
        public DbSet<MicrosoftUser> MicrosoftUsers { get; set; }
        public DbSet<BookQuote> BookQuotes { get; set; }
        public DbSet<ForumMessage> ForumMessages { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<User>(
            entityBuilder =>
            {
                entityBuilder
                    .ToTable("AspNetUsers")
                    .SplitToTable(
                        "AspNetDetailedInfoUsers",
                        tableBuilder =>
                        {
                            tableBuilder.Property(user => user.Id).HasColumnName("Id");
                            tableBuilder.Property(user => user.Method);
                            tableBuilder.Property(user => user.EmailConfirmed);
                            tableBuilder.Property(user => user.PasswordHash);
                            tableBuilder.Property(user => user.SecurityStamp);
                            tableBuilder.Property(user => user.ConcurrencyStamp);
                            tableBuilder.Property(user => user.PhoneNumber);
                            tableBuilder.Property(user => user.PhoneNumberConfirmed);
                            tableBuilder.Property(user => user.TwoFactorEnabled);
                            tableBuilder.Property(user => user.LockoutEnd);
                            tableBuilder.Property(user => user.LockoutEnabled);
                            tableBuilder.Property(user => user.AccessFailedCount);
                        });
            });


            modelBuilder.Entity<Friendship>().HasKey(fs => new { fs.User1Id, fs.User2Id, fs.Status });

            modelBuilder.Entity<BookChapter>().Ignore(u => u.Characters);

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


            modelBuilder.Entity<Friendship>()
                .HasOne(fs => fs.User1)
                .WithMany(u => u.InitiatorFriendships)
                .HasForeignKey(fs => fs.User1Id)
                .OnDelete(DeleteBehavior.NoAction);
            modelBuilder.Entity<Friendship>()
                .HasOne(fs => fs.User2)
                .WithMany(u => u.TargetFriendships)
                .HasForeignKey(fs => fs.User2Id)
                .OnDelete(DeleteBehavior.NoAction);


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
                .WithOne(c => c.User)
                .HasForeignKey(c => c.UserId)
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
                .HasMany(b => b.CommentRatings)
                .WithOne()
                .HasForeignKey(b => b.CommentId)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.Restrict);


            modelBuilder.Entity<Book>()
                .HasMany(c => c.BookQuotes)
                .WithOne(c => c.Book)
                .HasForeignKey(c => c.BookId)
                .IsRequired(false);

            modelBuilder.Entity<User>()
                .HasMany(c => c.BookQuotes)
                .WithOne(c => c.Quoter)
                .HasForeignKey(c => c.QuoterId)
                .IsRequired(false);

            modelBuilder.Entity<BookChapter>()
                .HasMany(c => c.Notes)
                .WithOne(c => c.BookChapter)
                .HasForeignKey(c => c.BookChapterid)
                .IsRequired(true)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<User>()
                .HasMany(c => c.Notes)
                .WithOne(c => c.User)
                .HasForeignKey(c => c.UserId)
                .IsRequired(true)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Note>()
                .HasMany(c => c.Replies)
                .WithOne(c => c.ReplyNote)
                .HasForeignKey(c => c.ReplyToBaseId)
                .OnDelete(DeleteBehavior.Restrict);


            modelBuilder.Entity<Book>()
                .HasMany(m => m.ForumMessages)
                .WithOne(m => m.Book)
                .HasForeignKey(m => m.BookId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<User>()
                .HasMany(m => m.ForumMessages)
                .WithOne(m => m.User)
                .HasForeignKey(m => m.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<ForumMessage>()
                .HasMany(m => m.AnswerMessages)
                .WithOne(m => m.ParrentMessage)
                .HasForeignKey(m => m.ParrentMessageId)
                .OnDelete(DeleteBehavior.NoAction);



            modelBuilder.Entity<Book>()
                .HasIndex(g => g.Title);
            modelBuilder.Entity<Book>()
                .HasIndex(g => g.PublicationData);
            modelBuilder.Entity<Book>()
                .HasIndex(g => g.Permission);
            modelBuilder.Entity<Book>()
                .HasIndex(g => g.AuthorId);

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
                .HasIndex(g => g.BookChapterId);
            modelBuilder.Entity<ChapterSection>()
                .HasIndex(g => g.Number);

            modelBuilder.Entity<Character>()
                .HasIndex(g => g.BookId);
            modelBuilder.Entity<Character>()
                .HasIndex(g => g.BookChapterId);


            modelBuilder.Entity<Comment>()
                .HasIndex(g => g.UserId);            
            modelBuilder.Entity<Note>()
                .HasIndex(g => g.UserId);
            modelBuilder.Entity<CommentBase>()
                .HasIndex(g => g.Permission);
            modelBuilder.Entity<Comment>()
                .HasIndex(g => g.BookId);            
            modelBuilder.Entity<Note>()
                .HasIndex(g => g.BookChapterid);


            modelBuilder.Entity<Friendship>()
                .HasIndex(g => g.User1Id);
            modelBuilder.Entity<Friendship>()
                .HasIndex(g => g.User2Id);
            modelBuilder.Entity<Friendship>()
                .HasIndex(g => g.Status);

            modelBuilder.Entity<MicrosoftUser>()
                .HasIndex(g => g.AzureName)
                .IsUnique();

            modelBuilder.Entity<SectionChoice>()
                .HasIndex(g => g.ChapterSectionId);

            modelBuilder.Entity<UserSelectedBook>()
                .HasIndex(g => g.UserId);
            modelBuilder.Entity<UserSelectedBook>()
                .HasIndex(g => g.BookId);
        }

        public virtual async Task<int> AddForumMessageAsync(int bookId, string userId, int? parentMessageId, string text)
        {
            var messageIdParameter = new SqlParameter("@MessageId", SqlDbType.Int)
            {
                Direction = ParameterDirection.Output
            };

            await Database.ExecuteSqlRawAsync(
                "EXEC AddForumMessage " +
                "@BookId, @UserId, @ParrentMessageId, @Text, @SendTime, @MessageId OUTPUT",
                new SqlParameter("@BookId", bookId),
                new SqlParameter("@UserId", userId),
                new SqlParameter("@ParrentMessageId", parentMessageId ?? (object)DBNull.Value),
                new SqlParameter("@Text", text),
                new SqlParameter("@SendTime", DateTime.Now),
                messageIdParameter);

            return (int)messageIdParameter.Value;
        }
    }
}
