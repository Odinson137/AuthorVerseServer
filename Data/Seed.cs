using AuthorVerseServer.Enums;
using AuthorVerseServer.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace AuthorVerseServer.Data
{
    public class Seed
    {

        public static async Task SeedData(IApplicationBuilder applicationBuilder)
        {
            using (var serviceScope = applicationBuilder.ApplicationServices.CreateScope())
            {
                var context = serviceScope.ServiceProvider.GetService<DataContext>();

                var pendingMigrations = context.Database.GetPendingMigrations();
                if (pendingMigrations.Any())
                {
                    await context.Database.EnsureDeletedAsync();
                    await context.Database.MigrateAsync();
                    await context.Database.EnsureCreatedAsync();
                }

                if (!context.Roles.Any())
                {

                    User admin = new User()
                    {
                        UserName = "Admin",
                        Description = "Люблю жизнь, она моя, она нагнула меня, но я не отчаиваюсь, живу",
                        Name = "Юри",
                        LastName = "Brown",
                        LogoUrl = "hashtag.png",
                        Method = RegistrationMethod.Email,
                        Email = "buryy132@gmail.com",
                    };

                    var roleManager = serviceScope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
                    var userManager = serviceScope.ServiceProvider.GetRequiredService<UserManager<User>>();

                    await userManager.CreateAsync(admin, "Password@123");

                    //await context.Users.AddAsync(admin);

                    string folderPath = @"./wwwroot/api/images/";

                    string[] fileNames = Directory.GetFiles(folderPath);
                    List<string> files = new List<string>(10);

                    foreach (string fileName in fileNames)
                    {
                        string name = Path.GetFileName(fileName);
                        files.Add(name);
                    }

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
                    var tagNames = new List<string>()
                    {
                        "Book",
                        "AudioBook",
                        "Manga",
                        "Stories",
                        "Comics",
                        "Children's literature",
                        "Dramas",
                        "Poetry",
                        "Documental literature",
                    };

                    var genres = new List<Genre>();

                    foreach (var genreName in genreNames)
                    {
                        var genre = new Genre { Name = genreName };
                        genres.Add(genre);
                        context.Add(genre);
                    }

                    var tags = new List<Tag>();

                    foreach (var tagName in tagNames)
                    {
                        var tag = new Tag { Name = tagName };
                        tags.Add(tag);
                        context.Add(tag);
                    }

                    if (!await roleManager.RoleExistsAsync("Admin"))
                    {
                        var role = new IdentityRole("Admin");
                        await roleManager.CreateAsync(role);
                    }

                    await userManager.AddToRoleAsync(admin, "Admin");

                    Dictionary<string, string> bookDescriptions = new Dictionary<string, string>
                    {
                        {"Красный и чёрный", "История о молодом и амбициозном Жюльене Сореле, который стремится взобраться на социальную лестницу Франции."},
                        {"Война и мир", "Величественная эпопея о Российской империи во времена войн против Наполеона."},
                        {"Унесенные ветром", "Рассказ о жизни Скарлетт О'Хара во время Гражданской войны в США."},
                        {"Мастер и Маргарита", "Завораживающая сатира и магическая история о Дьяволе и его визите в Москву."},
                        {"Маленький принц", "Сказочная история о маленьком принце и его путешествии по разными планетам."},
                        {"Гарри Поттер и философский камень", "Первая книга о Гарри Поттере, его приключениях и магии."},
                        {"Преступление и наказание", "Роман о Родионе Раскольникове, который совершает ужасное преступление и сталкивается с собственной совестью."},
                        {"Лолита", "Контроверзный роман о Гумберте Хамберте и его страсти к молодой девочке по имени Лолита."},
                        {"Великий Гэтсби", "История о Джей Гэтсби и его стремлении к американской мечте в 1920-х годах."},
                        {"1984", "Дистопический роман о тоталитарном обществе, где правительство контролирует каждый аспект жизни граждан."}
                    };

                    List<string> usersName = new List<string>()
                    {
                        "", "Лев", "Маргарет", "Антуан", "Михаил", "Джоан", "Фёдор",
                        "Владимир", "Фрэнсис Скотт", "Джордж"
                    };

                    List<string> usersLastName = new List<string>()
                    {
                        "Стендаль", "Толстой", "Митчелл", "де Сент-Экзюпери", "Булгаков", "Роулинг", "Достоевский",
                        "Набоков", "Фицджеральд", "Оруэлл"
                    };

                    Random random = new Random();

                    int n = 0;
                    for (int b = 0; b < 100; b++)
                    {
                        User user = new User()
                        {
                            UserName = $"JS_User_{b}",
                            Description = "Люблю Ярика",
                            Email = "Kekus132@gmail.com",
                            Method = RegistrationMethod.Email,
                            EmailConfirmed = true,
                            LogoUrl = "java-script.png",
                            Name = usersName[n],
                            LastName = usersLastName[n],
                        };

                        n = (n + 1) % 10;

                        await userManager.CreateAsync(user, "ЮрикИзМножества_ЯрикИзСкриптеров_СаняИзНарода123");
                        await context.Users.AddAsync(user);

                        int num = 0;

                        foreach (var book in bookDescriptions)
                        {
                            var Book = new Book()
                            {
                                Title = book.Key + b.ToString(),
                                Author = admin,
                                Description = book.Value + b.ToString(),
                                PublicationData = DateTime.Now,
                                AgeRating = Enums.AgeRating.All,
                                Permission = Enums.PublicationPermission.Approved,
                                BookCover = files[num]
                            };
                            num++;

                            for (int i = 0; i < 3; i++)
                            {
                                var genre = genres[random.Next(0, 16)];
                                Book.Genres.Add(genre);
                            }

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

                            await context.AddAsync(Book);
                        }
                    }

                    await context.SaveChangesAsync();
                }
            }

        }
    }
}
