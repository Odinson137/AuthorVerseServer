using AuthorVerseServer.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Reflection.Metadata;

namespace AuthorVerseServer.Data
{
    public class DataContext : IdentityDbContext<User>
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {
            //ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
        }

        public DbSet<Book> Books { get; set; }
        public DbSet<UserSelectedBook> UserSelectedBooks { get; set; }
        public DbSet<Genre> Genres { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<Note> Notes { get; set; }
        public DbSet<Character> Characters { get; set; }

        //protected override void OnModelCreating(ModelBuilder modelBuilder)
        //{
        //    modelBuilder.Entity<User>()
        //        .HasMany(e => e.Books)
        //        .WithOne(e => e.Author)
        //        .HasForeignKey(e => e.AuthorId)
        //        .IsRequired();
        //}

        public static void Seed(DataContext context)
        {
            string folderPath = @"./bookImage";

            string[] fileNames = Directory.GetFiles(folderPath);
            List<string> files = new List<string>(10);
            
            foreach (string fileName in fileNames)
            {
                string name = Path.GetFileName(fileName);
                files.Add(name);
            }

            if (!context.Books.Any())
            {
                //context.ChangeTracker.AutoDetectChangesEnabled = false;

                var genreNames = new List<string>
                    {
                        "Фантастика",
                        "Детектив",
                        "Фэнтези",
                        "Приключения",
                        "Роман",
                        "Триллер",
                        "Ужасы",
                        "Научная фантастика",
                        "Исторический",
                        "Боевик",
                        "Мистика",
                        "Комедия",
                        "Драма",
                        "Фильм-нуар",
                        "Биография",
                        "Фэнтези"
                    };

                var genres = new List<Genre>();

                foreach (var genreName in genreNames)
                {
                    var genre = new Genre { Name = genreName };
                    genres.Add(genre);
                    context.Add(genre);
                }

                User admin = new User()
                {
                    UserName = "Admin",
                };

                context.Users.Add(admin);

                Dictionary<string, string> bookDescriptions = new Dictionary<string, string>
                {
                    {"Красный и чёрный - Стендаль", "История о молодом и амбициозном Жюльене Сореле, который стремится взобраться на социальную лестницу Франции."},
                    {"Война и мир - Лев Толстой", "Величественная эпопея о Российской империи во времена войн против Наполеона."},
                    {"Унесенные ветром - Маргарет Митчелл", "Рассказ о жизни Скарлетт О'Хара во время Гражданской войны в США."},
                    {"Мастер и Маргарита - Михаил Булгаков", "Завораживающая сатира и магическая история о Дьяволе и его визите в Москву."},
                    {"Маленький принц - Антуан де Сент-Экзюпери", "Сказочная история о маленьком принце и его путешествии по разными планетам."},
                    {"Гарри Поттер и философский камень - Джоан Роулинг", "Первая книга о Гарри Поттере, его приключениях и магии."},
                    {"Преступление и наказание - Фёдор Достоевский", "Роман о Родионе Раскольникове, который совершает ужасное преступление и сталкивается с собственной совестью."},
                    {"Лолита - Владимир Набоков", "Контроверзный роман о Гумберте Хамберте и его страсти к молодой девочке по имени Лолита."},
                    {"Великий Гэтсби - Фрэнсис Скотт Фицджеральд", "История о Джей Гэтсби и его стремлении к американской мечте в 1920-х годах."},
                    {"1984 - Джордж Оруэлл", "Дистопический роман о тоталитарном обществе, где правительство контролирует каждый аспект жизни граждан."}
                };

                Random random = new Random();

                int num = 0;
                foreach (var book in bookDescriptions)
                {
                    var Book = new Book()
                    {
                        Title = book.Key,
                        Author = admin,
                        Description = book.Value,
                        PublicationData = DateTime.Now,
                        AgeRating = Enums.AgeRating.All,
                        Permission = Enums.PublicationPermission.Approved,
                        Genres = new List<Genre>(),
                        BookCover = new Image() { Url = files[num] }
                    };
                    num++;

                    for (int i = 0; i < 3; i++)
                    {
                        var genre = genres[random.Next(0, 16)];
                        Book.Genres.Add(genre);
                    }


                    BookChapter chapter = new BookChapter()
                    {
                        ChapterSections = new List<ChapterSection>()
                        {
                            new ChapterSection()
                            {
                                Number = 1,
                                Text = "Среди сотен тысяч звёздных систем, скрывающих в себе тайны далеких миров, начинается наше удивительное путешествие. Это история о смелых искателях приключений, готовых исследовать неведомые просторы космоса. Они столкнутся с загадочными цивилизациями, раскроют давно забытые тайны, и, возможно, найдут ответы на самые глубокие вопросы о природе вселенной.\r\n\r\nЭта книга приглашает вас отправиться в захватывающее космическое приключение, полное опасностей и открытий. Вас ждут неизведанные планеты, космические бури и встречи с разумными существами, о которых вы и не могли мечтать. Готовы ли вы покорить звёзды и найти свой след в бескрайних просторах галактики?\r\n\r\nОткройте первую страницу и погрузитесь в этот фантастический мир, где каждая глава — это новое открытие, а каждая строчка — шаг в неизведанные горизонты. Готовьтесь к невероятным приключениям и встречам, которые оставят вас в состоянии постоянного восхищения. Дерзайте, исследователи космоса, потому что неведомые миры ждут вас!"
                            }
                        },
                        PublicationData = DateTime.Now
                    };

                    BookChapter chapter2 = new BookChapter()
                    {
                        ChapterSections = new List<ChapterSection>()
                        {
                            new ChapterSection()
                            {
                                Number = 2,
                                Text = "Среди сотен тысяч звёздных систем, скрывающих в себе тайны далеких миров, начинается наше удивительное путешествие. Это история о смелых искателях приключений, готовых исследовать неведомые просторы космоса. Они столкнутся с загадочными цивилизациями, раскроют давно забытые тайны, и, возможно, найдут ответы на самые глубокие вопросы о природе вселенной.\r\n\r\nЭта книга приглашает вас отправиться в захватывающее космическое приключение, полное опасностей и открытий. Вас ждут неизведанные планеты, космические бури и встречи с разумными существами, о которых вы и не могли мечтать. Готовы ли вы покорить звёзды и найти свой след в бескрайних просторах галактики?\r\n\r\nОткройте первую страницу и погрузитесь в этот фантастический мир, где каждая глава — это новое открытие, а каждая строчка — шаг в неизведанные горизонты. Готовьтесь к невероятным приключениям и встречам, которые оставят вас в состоянии постоянного восхищения. Дерзайте, исследователи космоса, потому что неведомые миры ждут вас!"
                            }
                        },
                        PublicationData = DateTime.Now
                    };

                    Book.BookChapters = new List<BookChapter>() { chapter, chapter2 };

                    context.Add(Book);
                }

                context.SaveChanges();
            }
        }

    }
}
