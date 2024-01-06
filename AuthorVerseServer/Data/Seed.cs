using AuthorVerseServer.Data.Enums;
using AuthorVerseServer.Interfaces;
using AuthorVerseServer.Models;
using AuthorVerseServer.Models.ContentModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace AuthorVerseServer.Data
{
    public class Seed
    {
        public static async Task SeedData(DataContext context, RoleManager<IdentityRole> roleManager, UserManager<User> userManager)
        {
            var pendingMigrations = await context.Database.GetPendingMigrationsAsync();
            if (pendingMigrations.Any())
            {
                await context.Database.MigrateAsync();
            }

            if (!context.Books.Any() || !context.Friendships.Any())
            {

                //await context.Database.EnsureDeletedAsync();
                //await context.Database.EnsureCreatedAsync();

                User admin = new User()
                {
                    Id = "admin",
                    UserName = "Admin",
                    Description = "Люблю жизнь, она моя, она нагнула меня, но я не отчаиваюсь, живу",
                    Name = "Юри",
                    LastName = "Brown",
                    LogoUrl = "hashtag.png",
                    Method = RegistrationMethod.Email,
                    Email = "buryy137@gmail.com",
                    EmailConfirmed = true,
                };

                await userManager.CreateAsync(admin, "Password@123");

                if (!await roleManager.RoleExistsAsync("Admin"))
                {
                    var role = new IdentityRole("Admin");
                    await roleManager.CreateAsync(role);
                }

                await userManager.AddToRoleAsync(admin, "Admin");

                for (int i = 0; i < 5; i++)
                {
                    var friend = new User
                    {
                        UserName = $"Friend-{i}",
                        Description = "Живу ради админа",
                        Name = "Friend",
                        LastName = "Brown",
                        LogoUrl = "hashtag.png",
                        Method = RegistrationMethod.Email,
                        Email = $"frined132{i}@gmail.com",
                    };

                    Friendship friendship = new Friendship
                    {
                        User1 = admin,
                        User2 = friend,
                        Status = FriendshipStatus.Accepted,
                    };

                    admin.InitiatorFriendships.Add(friendship);
                }

                for (int i = 0; i < 2; i++)
                {
                    var friend = new User
                    {
                        UserName = $"Friend-request-{i}",
                        Description = "Живу ради админа, чтоб быть у него в запросах",
                        Name = "Friend",
                        LastName = "Brown",
                        LogoUrl = "hashtag.png",
                        Method = RegistrationMethod.Email,
                        Email = $"frined1321{i}@gmail.com",
                    };

                    Friendship friendship = new Friendship
                    {
                        User1 = admin,
                        User2 = friend,
                        Status = FriendshipStatus.Pending,
                    };

                    admin.InitiatorFriendships.Add(friendship);
                }

                for (int i = 0; i < 2; i++)
                {
                    var friend = new User
                    {
                        UserName = $"Friend-banned-{i}",
                        Description = "Живу ради админа, чтоб быть у него в бане",
                        Name = "Friend",
                        LastName = "Brown",
                        LogoUrl = "hashtag.png",
                        Method = RegistrationMethod.Email,
                        Email = $"frined1322{i}@gmail.com",
                    };

                    Friendship friendship = new Friendship
                    {
                        User1 = admin,
                        User2 = friend,
                        Status = FriendshipStatus.Blocked,
                    };

                    admin.InitiatorFriendships.Add(friendship);
                }

                var friend1 = new User
                {
                    UserName = $"Friend-banned-{344444444}",
                    Description = "Живу ради админа, чтоб быть у него в бане",
                    Name = "Friend",
                    LastName = "White",
                    LogoUrl = "hashtag.png",
                    Method = RegistrationMethod.Email,
                    Email = $"frined1322{3444444444444}@gmail.com",
                };

                Friendship friendship1 = new Friendship
                {
                    User1 = friend1,
                    User2 = admin,
                    Status = FriendshipStatus.Accepted,
                };

                admin.TargetFriendships.Add(friendship1);


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
                        "Science Fiction",
                        "Detective",
                        "Fantasy",
                        "Adventure",
                        "Romance",
                        "Thriller",
                        "Horror",
                        "Science Fiction",
                        "Historical",
                        "Action",
                        "Mystery",
                        "Comedy",
                        "Drama",
                        "Film Noir",
                        "Biography",
                        "Fantasy"
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
                }

                await context.AddRangeAsync(genres);

                var tags = new List<Tag>();

                foreach (var tagName in tagNames)
                {
                    var tag = new Tag { Name = tagName };
                    tags.Add(tag);
                }

                await context.AddRangeAsync(tags);



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

                var firstBook = new Book
                {
                    Title = "FirstBook",
                    Author = admin,
                    Description = "первая книга для тестов",
                    AgeRating = AgeRating.All,
                    Permission = PublicationPermission.Approved,
                    BookCover = files[0],
                    Comments = new List<Comment>() 
                    { 
                        new Comment
                        {
                            //Likes = 1,
                            //DisLikes = 0,
                            Text = "Тестовый комментарий админа",
                            User = admin,
                            ReaderRating = 4,
                            Permission = PublicationPermission.Approved,
                            CommentRatings = new List<Rating>()
                            {
                                new Rating()
                                {
                                    LikeRating = LikeRating.Like,
                                    UserCommentedId = admin.Id,
                                    Discriminator = RatingEntityType.Comment,
                                }
                            }
                        },
                    },
                    BookChapters = new List<BookChapter>(),
                };

                await context.Books.AddAsync(firstBook);
                await context.SaveChangesAsync();

                var chapter1 = new BookChapter()
                {
                    Book = firstBook,
                    BookChapterNumber = 1,
                    Title = "Что-то",
                    ChapterSections = new List<ChapterSection>(),
                };

                firstBook.BookChapters.Add(chapter1);
                await context.BookChapters.AddAsync(chapter1);
                await context.SaveChangesAsync();

                var section1 = new ChapterSection()
                {
                    BookChapter = chapter1,
                    ContentType = ContentType.Text,
                    Number = 1,
                    ChoiceFlow = 1,

                    ContentBase = new TextContent()
                    {
                        Text = "Среди сотен тысяч звёздных систем, скрывающих в себе тайны далеких миров, начинается наше удивительное путешествие. Это история о смелых искателях приключений, готовых исследовать неведомые просторы космоса. Они столкнутся с загадочными цивилизациями, раскроют давно забытые тайны, и, возможно, найдут ответы на самые глубокие вопросы о природе вселенной.\r\n\r\nЭта книга приглашает вас отправиться в захватывающее космическое приключение, полное опасностей и открытий. Вас ждут неизведанные планеты, космические бури и встречи с разумными существами, о которых вы и не могли мечтать. Готовы ли вы покорить звёзды и найти свой след в бескрайних просторах галактики?\r\n\r\nОткройте первую страницу и погрузитесь в этот фантастический мир, где каждая глава — это новое открытие, а каждая строчка — шаг в неизведанные горизонты. Готовьтесь к невероятным приключениям и встречам, которые оставят вас в состоянии постоянного восхищения. Дерзайте, исследователи космоса, потому что неведомые миры ждут вас!",
                    }
                };

                chapter1.ChapterSections.Add(section1);

                var section2 = new ChapterSection()
                {
                    BookChapter = chapter1,
                    ContentType = ContentType.Text,
                    Number = 2,
                    ChoiceFlow = 1,

                    ContentBase = new TextContent()
                    {
                        Text = "Вы выбрали бежать наружу. Под открытым небом вас ждут новые приключения и неизведанные опасности.",
                    }
                };

                chapter1.ChapterSections.Add(section2);

                var section3 = new ChapterSection()
                {
                    BookChapter = chapter1,
                    ContentType = ContentType.Audio,
                    Number = 3,
                    ChoiceFlow = 1,

                    ContentBase = new AudioContent()
                    {
                        Url = "MiyaGi & Andy Panda - Буревестник.mp3",
                    }
                };

                chapter1.ChapterSections.Add(section3);

                var choiceSection1 = new SectionChoice()
                {
                    ChoiceText = "Run outside",
                    ChoiceNumber = 1,
                    TargetSection = new ChapterSection()
                    {
                        Number = 5,
                        ChoiceFlow = 1,
                        BookChapter = chapter1,
                        ContentType = ContentType.Image,
                        ContentBase = new FileContent
                        {
                            Url = "javascript-it-юмор-geek-5682739.jpeg",
                        }
                    }
                };

                var returnChoiceSection = new ChapterSection()
                {
                    Number = 5,
                    ChoiceFlow = 2,
                    BookChapter = chapter1,
                    ContentType = ContentType.Text,
                    ContentBase = new TextContent
                    {
                        Text = "Вы выбрали бежать вперед."
                    }
                };

                var choiceSection2 = new SectionChoice()
                {
                    ChoiceNumber = 2,
                    ChoiceText = "Run forward",
                    TargetSection = returnChoiceSection,
                };

                var section4 = new ChapterSection()
                {
                    BookChapter = chapter1,
                    ContentType = ContentType.Text,
                    Number = 4,
                    ChoiceFlow = 1,

                    ContentBase = new TextContent()
                    {
                        Text = "Здесь должен быть переход",
                    },
                    SectionChoices = new List<SectionChoice>()
                    {
                        choiceSection1, choiceSection2,
                    },
                };

                chapter1.ChapterSections.Add(section4);

                var section5 = new ChapterSection()
                {
                    BookChapter = chapter1,
                    ContentType = ContentType.Text,
                    Number = 6,
                    ChoiceFlow = 1,

                    ContentBase = new TextContent()
                    {
                        Text = "Привет, это продолжение первого потока без разделения",
                    },
                };

                chapter1.ChapterSections.Add(section5);

                var choiceSection3 = new SectionChoice()
                {
                    ChoiceText = "Идти направо",
                    ChoiceNumber = 1,

                    TargetSection = new ChapterSection()
                    {
                        Number = 8,
                        ChoiceFlow = 1,
                        BookChapter = chapter1,
                        ContentType = ContentType.Text,
                        ContentBase = new TextContent
                        {
                            Text = "Вы выбрали не на лево, а на право)"
                        }
                    }
                };

                var choiceSection4 = new SectionChoice()
                {
                    ChoiceNumber = 2,
                    ChoiceText = "Идти налево",
                    TargetSection = new ChapterSection()
                    {
                        Number = 8,
                        ChoiceFlow = 3,
                        BookChapter = chapter1,
                        ContentType = ContentType.Text,
                        ContentBase = new TextContent
                        {
                            Text = "Вы выбрали на лево, а не на право)"
                        }
                    }
                };

                var section6 = new ChapterSection()
                {
                    BookChapter = chapter1,
                    ContentType = ContentType.Text,
                    Number = 7,
                    ChoiceFlow = 1,

                    ContentBase = new TextContent()
                    {
                        Text = "Привет, это продолжение и 7 часть. Во мне есть пути идти направо либо налево написанные с ошибками)",
                    },
                    SectionChoices = new List<SectionChoice>()
                    {
                        choiceSection3, choiceSection4,
                    },
                };

                chapter1.ChapterSections.Add(section6);

                var section7 = new ChapterSection()
                {
                    BookChapter = chapter1,
                    ContentType = ContentType.Text,
                    Number = 9,
                    ChoiceFlow = 1,

                    ContentBase = new TextContent()
                    {
                        Text = "Сюда ты только что перешёл. И это просто текст. В следующем будет возврат во вторую ветвь",
                    },
                };

                chapter1.ChapterSections.Add(section7);

                var choiceSection5 = new SectionChoice()
                {
                    ChoiceNumber = 1,
                    ChoiceText = "Вернуться к самому началу. по факту это будет единственный путь. Так тоже можно, кто мешает",
                    TargetSection = returnChoiceSection,
                };

                var section8 = new ChapterSection()
                {
                    BookChapter = chapter1,
                    ContentType = ContentType.Text,
                    Number = 10,
                    ChoiceFlow = 1,

                    ContentBase = new TextContent()
                    {
                        Text = "А и из этой части ты должен будешь перекидываться во второй поток, который был ещё в самом начале либо всё",
                    },
                    SectionChoices = new List<SectionChoice>()
                    {
                        choiceSection5,
                    },
                };

                chapter1.ChapterSections.Add(section8);

                var section9 = new ChapterSection()
                {
                    BookChapter = chapter1,
                    ContentType = ContentType.Text,
                    Number = 6,
                    ChoiceFlow = 2,

                    ContentBase = new TextContent()
                    {
                        Text = "Конец этой мини истории",
                    },
                };

                chapter1.ChapterSections.Add(section9);



                for (int i = 0; i < 3; i++)
                {
                    var genre = genres[random.Next(0, 16)];
                    firstBook.Genres.Add(genre);
                }


                for (int i = 0; i < 3; i++)
                {
                    var tag = tags[random.Next(0, 9)];
                    firstBook.Tags.Add(tag);
                }

                var chapterTest = new BookChapter
                {
                    Book = firstBook,
                    Characters = new List<Character>(),
                    BookChapterNumber = 1,
                    Title = "Перерождение",
                    Description = "ЧТо я вижу, что я знаю"
                };

                var character1 = new Character()
                {
                    Name = "Борин",
                    Book = firstBook,
                    Description = "хохохох",
                    BookChapters = new List<BookChapter>() { chapterTest },
                };

                var character2 = new Character()
                {
                    Name = "Марес",
                    Book = firstBook,
                    Description = "хихихиххи",
                    BookChapters = new List<BookChapter>() { chapterTest },
                };

                var chapterTest2 = new BookChapter
                {
                    Book = firstBook,
                    Characters = new List<Character>(),
                    BookChapterNumber = 2,
                    Title = "Перерождение",
                    Description = "ЧТо я вижу, что я знаю"
                };

                var character3 = new Character
                {
                    Book = firstBook,
                    Description = "Тут такой борзый пацанчик",
                    Name = "Анджелина",
                    BookChapters = new List<BookChapter>() { chapterTest, chapterTest2 },
                };

                await context.Characters.AddRangeAsync(character1, character2, character3);

                await context.BookChapters.AddRangeAsync(chapterTest, chapterTest2);

                var selectedBook = new UserSelectedBook()
                {
                    BookState = BookState.Reading,
                    Book = firstBook,
                    User = admin,
                    LastBookChapterNumber = firstBook.BookChapters.First().BookChapterNumber,
                };

                User kol = new User
                {
                    Id = "kolic",
                    UserName = "Kolic",
                    NormalizedUserName = "Kolic".ToUpper(),
                    Email = "ppockorn@gmail.com",
                    NormalizedEmail = "ppockorn@gmail.com".ToUpper(),
                    EmailConfirmed = true,
                };

                await context.Users.AddAsync(kol);

                var selectedBook1 = new UserSelectedBook()
                {
                    BookState = BookState.Reading,
                    Book = firstBook,
                    User = kol,
                    LastBookChapterNumber = firstBook.BookChapters.First().BookChapterNumber,
                };

                await context.UserSelectedBooks.AddRangeAsync(selectedBook, selectedBook1);

                int numer = 0;
                foreach (var book in bookDescriptions)
                {
                    Book Book = new Book()
                    {
                        Title = book.Key,
                        Author = new User { UserName = $"CCharpProger_{numer}", 
                            Name = usersName[numer],
                            LastName = usersLastName[numer]
                        },
                        Description = book.Value,
                        AgeRating = AgeRating.All,
                        Permission = PublicationPermission.Approved,
                        BookCover = files[numer++]
                    };

                    for (int i = 0; i < 3; i++)
                    {
                        var genre = genres[random.Next(0, 16)];
                        Book.Genres.Add(genre);
                    }

                    for (int i = 0; i < 3; i++)
                    {
                        var tag = tags[random.Next(0, 9)];
                        Book.Tags.Add(tag);
                    }

                    var chapters = new List<BookChapter>() 
                    { 
                        new BookChapter()
                        {
                            Title = "Топовая уникальная глава",
                            BookChapterNumber = 1,
                            ChapterSections = new List<ChapterSection>()
                                {
                 
                                }
                        }, new BookChapter()
                        {
                            Title = "Топовая уникальная глава",
                            BookChapterNumber = 2,
                            ChapterSections = new List<ChapterSection>()
                                {
                                   
                                }
                        }, new BookChapter()
                        {
                            Title = "Топовая уникальная глава",
                            BookChapterNumber = 3,
                            ChapterSections = new List<ChapterSection>()
                                {
                                    
                                }
                        } };

                    Note note = new Note()
                    {
                        User = admin,
                        BookChapter = chapters[0],
                        Text = "Когда же выйдет новые главы магической битвы!",
                        Replies = new List<Note>
                        {
                            new Note() {
                                User = admin,
                                BookChapter = chapters[0],
                                Text = "В качестве продолжения к моему прошлому комментарию пишу, что новые главы магической битвы до сих по не вышли. Годжо, живи!",
                            },
                            new Note() {
                                User = admin,
                                BookChapter = chapters[0],
                                Text = "Ещё не вышло. Ладно, пойду Берсерка почитаю",
                                Replies = new List<Note>
                                {
                                    new Note() {
                                        User = admin,
                                        BookChapter = chapters[0],
                                        Text = "Бля, Берсерк тоже не вышел!!!",
                                    }
                                }
                            },
                        }
                    };

                    admin.Notes.Add(note);

                    var comments = new List<Comment>();
                    admin.Notes.Add(note);
                    for (int i = 0; i < random.Next(5, 10); i++)
                    {
                        comments.Add(new Comment()
                        {
                            User = admin,
                            Text = "Это мой первый тестовый коммент, так что не судите строго. Я правда стараюсь, Ярик, пожалуйста, не делай со мной то, что ты делал со мной в прошлый раз. Пожалуйста",
                            ReaderRating = random.Next(1, 6),
                            //Likes = 1,
                            //DisLikes = 1,
                        });
                        comments.Add(new Comment()
                        {
                            User = admin,
                            Text = "Это мой второй тестовый коммент, так что не судите строго. Эта книга — настоящий литературный шедевр! Автор с легкостью создает живописные образы и увлекательные сюжеты, погружая читателя в удивительный мир воображения. От момента первого взгляда до последней строки она дарит непередаваемые эмоции и заставляет задуматься над глубокими философскими вопросами. Страстные персонажи, захватывающие сюжеты и неожиданные повороты — все это делает книгу невероятно увлекательной и запоминающейся. Прочитав ее, вы погружаетесь в уникальный мир приключений и открываете для себя неизведанные грани литературы",
                            ReaderRating = random.Next(1, 6)
                        });
                        comments.Add(new Comment()
                        {
                            User = admin,
                            Text = "Это мой третий тестовый коммент, так что не судите строго. Увлекательная история, наполненная волнующими поворотами сюжета и захватывающими персонажами. Книга, которая поднимает важные вопросы о судьбе, предательстве и вечной борьбе добра со злом. Мастерски прописанные детали создают неповторимую атмосферу, погружая читателя в удивительный мир авторского воображения. Здесь найдется место как для приключений, так и для глубоких размышлений. Невозможно оторваться от страниц, пока герои ведут нас через запутанные лабиринты загадок и тайн. Великолепное произведение, которое оставит неизгладимый след в сердце каждого читателя",
                            ReaderRating = random.Next(1, 6)
                        });
                    }

                    Book.Comments = comments;

                    Book.Rating = comments.Average(x => x.ReaderRating);
                    Book.CountRating = comments.Count;

                    Book.BookChapters = chapters;

                    UserSelectedBook userSelectedBook = new UserSelectedBook()
                    {
                        Book = Book,
                        User = admin,
                        BookState = (BookState)random.Next(0, 5),
                        LastBookChapterNumber = random.Next(1, 4),
                    };

                    admin.UserSelectedBooks.Add(userSelectedBook);
                }

                int numeric = 0;
                foreach (var book in bookDescriptions)
                {
                    var Book = new Book()
                    {
                        Title = book.Key,
                        Author = admin,
                        Description = book.Value,
                        AgeRating = AgeRating.All,
                        Permission = PublicationPermission.Approved,
                        BookCover = files[numeric++],
                        BookQuotes = new List<BookQuote> { 
                            new BookQuote
                            {
                                User = admin,
                                Text = "Огонь не тот, кто ярко пляшет, а тот, кто всё уничтожает"
                            },
                            new BookQuote
                            {
                                User = admin,
                                Text = "Я не дорожу вашим мнением, оно вращается в туалете семенем"
                            },
                            new BookQuote
                            {
                                User = admin,
                                Text = "Огонь не тот, кто ярко пляшет, а тот, кто всё уничтожает"
                            },
                            new BookQuote
                            {
                                User = admin,
                                Text = "Я не дорожу вашим мнением, оно вращается в туалете семенем"
                            },
                            new BookQuote
                            {
                                User = admin,
                                Text = "Огонь не тот, кто ярко пляшет, а тот, кто всё уничтожает"
                            },
                            new BookQuote
                            {
                                User = admin,
                                Text = "Я не дорожу вашим мнением, оно вращается в туалете семенем"
                            },
                        }
                    };

                    for (int i = 0; i < 3; i++)
                    {
                        var genre = genres[random.Next(0, 16)];
                        Book.Genres.Add(genre);
                    }

                    for (int i = 0; i < 3; i++)
                    {
                        var tag = tags[random.Next(0, 9)];
                        Book.Tags.Add(tag);
                    }


                    BookChapter chapter = new BookChapter()
                    {
                        Title = "Топовая уникальная глава",
                        BookChapterNumber = 1,
                        ChapterSections = new List<ChapterSection>()
                        {
                           
                        }
                    };

                    BookChapter chapter2 = new BookChapter()
                    {
                        Title = "Топовая уникальная глава",
                        BookChapterNumber = 1,
                        ChapterSections = new List<ChapterSection>()
                                {

                                }
                    };

                    BookChapter chapter3 = new BookChapter()
                    {
                        Title = "Топовая уникальная глава",
                        BookChapterNumber = 1,
                        ChapterSections = new List<ChapterSection>()
                                {
                                    
                                }
                    };

                    Book.BookChapters = new List<BookChapter>() { chapter, chapter2, chapter3 };

                    var comments = new List<Comment>();
                    for (int i = 0; i < random.Next(3, 10); i++)
                    {
                        comments.Add(new Comment()
                        {
                            User = admin,
                            Text = "Это мой первый тестовый коммент, так что не судите строго. Я правда стараюсь, Ярик, пожалуйста, не делай со мной то, что ты делал со мной в прошлый раз. Пожалуйста",
                            ReaderRating = random.Next(1, 6)
                        });
                        comments.Add(new Comment()
                        {
                            User = admin,
                            Text = "Это мой второй тестовый коммент, так что не судите строго. Эта книга — настоящий литературный шедевр! Автор с легкостью создает живописные образы и увлекательные сюжеты, погружая читателя в удивительный мир воображения. От момента первого взгляда до последней строки она дарит непередаваемые эмоции и заставляет задуматься над глубокими философскими вопросами. Страстные персонажи, захватывающие сюжеты и неожиданные повороты — все это делает книгу невероятно увлекательной и запоминающейся. Прочитав ее, вы погружаетесь в уникальный мир приключений и открываете для себя неизведанные грани литературы",
                            ReaderRating = random.Next(1, 6)
                        });
                        comments.Add(new Comment()
                        {
                            User = admin,
                            Text = "Это мой третий тестовый коммент, так что не судите строго. Увлекательная история, наполненная волнующими поворотами сюжета и захватывающими персонажами. Книга, которая поднимает важные вопросы о судьбе, предательстве и вечной борьбе добра со злом. Мастерски прописанные детали создают неповторимую атмосферу, погружая читателя в удивительный мир авторского воображения. Здесь найдется место как для приключений, так и для глубоких размышлений. Невозможно оторваться от страниц, пока герои ведут нас через запутанные лабиринты загадок и тайн. Великолепное произведение, которое оставит неизгладимый след в сердце каждого читателя",
                            ReaderRating = random.Next(1, 6)
                        });
                    }

                    Book.Comments = comments;

                    Book.Rating = comments.Average(x => x.ReaderRating);
                    Book.CountRating = comments.Count;

                    admin.Books.Add(Book);
                }

                int n = 0;
                for (int b = 0; b < 1; b++)
                {
                    User user = new User()
                    {
                        UserName = $"JSUser{b}",
                        Description = "Люблю Ярика",
                        Email = "Kekus132@gmail.com",
                        Method = RegistrationMethod.Email,
                        EmailConfirmed = true,
                        LogoUrl = "java-script.png",
                        Name = usersName[n],
                        LastName = usersLastName[n],
                    };

                    n = (n + 1) % 10;

                    //await userManager.CreateAsync(user, "ЮрикИзМножества_ЯрикИзСкриптеров_СаняИзНарода123");
                    await context.Users.AddAsync(user);

                    int num = 0;

                    foreach (var book in bookDescriptions)
                    {
                        var Book = new Book()
                        {
                            Title = book.Key + b.ToString(),
                            Author = user,
                            Description = book.Value + b.ToString(),
                            AgeRating = AgeRating.All,
                            Permission = PublicationPermission.Approved,
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
                            var tag = tags[random.Next(0, 9)];
                            Book.Tags.Add(tag);
                        }


                        BookChapter chapter = new BookChapter()
                        {
                            ChapterSections = new List<ChapterSection>()
                                {
                                   
                                }
                        };

                        BookChapter chapter2 = new BookChapter()
                        {
                            ChapterSections = new List<ChapterSection>()
                                {
                                    
                                }
                        };

                        BookChapter chapter3 = new BookChapter()
                        {
                            ChapterSections = new List<ChapterSection>()
                                {
                                    
                                }
                        };

                        Book.BookChapters = new List<BookChapter>() { chapter, chapter2, chapter3 };

                        var comments = new List<Comment>();
                        for (int i = 0; i < random.Next(5, 10); i++)
                        {
                            comments.Add(new Comment()
                            {
                                User = user,
                                Text = "Это мой первый тестовый коммент, так что не судите строго. Я правда стараюсь, Ярик, пожалуйста, не делай со мной то, что ты делал со мной в прошлый раз. Пожалуйста",
                                ReaderRating = random.Next(1, 6)
                            });
                            comments.Add(new Comment()
                            {
                                User = user,
                                Text = "Это мой второй тестовый коммент, так что не судите строго. Эта книга — настоящий литературный шедевр! Автор с легкостью создает живописные образы и увлекательные сюжеты, погружая читателя в удивительный мир воображения. От момента первого взгляда до последней строки она дарит непередаваемые эмоции и заставляет задуматься над глубокими философскими вопросами. Страстные персонажи, захватывающие сюжеты и неожиданные повороты — все это делает книгу невероятно увлекательной и запоминающейся. Прочитав ее, вы погружаетесь в уникальный мир приключений и открываете для себя неизведанные грани литературы",
                                ReaderRating = random.Next(1, 6)
                            });
                            comments.Add(new Comment()
                            {
                                User = user,
                                Text = "Это мой третий тестовый коммент, так что не судите строго. Увлекательная история, наполненная волнующими поворотами сюжета и захватывающими персонажами. Книга, которая поднимает важные вопросы о судьбе, предательстве и вечной борьбе добра со злом. Мастерски прописанные детали создают неповторимую атмосферу, погружая читателя в удивительный мир авторского воображения. Здесь найдется место как для приключений, так и для глубоких размышлений. Невозможно оторваться от страниц, пока герои ведут нас через запутанные лабиринты загадок и тайн. Великолепное произведение, которое оставит неизгладимый след в сердце каждого читателя",
                                ReaderRating = random.Next(1, 6)
                            });
                        }

                        Book.Comments = comments;

                        Book.Rating = comments.Average(x => x.ReaderRating);
                        Book.CountRating = comments.Count;

                        user.Books.Add(Book);
                    }
                }

                n = 0;

                for (int b = 0; b < 40; b++)
                {
                    User user = new User()
                    {
                        UserName = $"PyUser{b}",
                        Description = "Люблю Ярика",
                        Email = "Putus132@gmail.com",
                        Method = RegistrationMethod.Email,
                        EmailConfirmed = true,
                        LogoUrl = "java-script.png",
                        Name = usersName[n],
                        LastName = usersLastName[n],
                    };

                    n = (n + 1) % 10;

                    //await userManager.CreateAsync(user, "ЮрикИзМножества_ЯрикИзСкриптеров_СаняИзНарода123");
                    await context.Users.AddAsync(user);

                    int num = 0;

                    foreach (var book in bookDescriptions)
                    {
                        var Book = new Book()
                        {
                            Title = book.Key + b.ToString(),
                            Author = user,
                            Description = book.Value + b.ToString(),
                            AgeRating = AgeRating.All,
                            Permission = PublicationPermission.Approved,
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
                            var tag = tags[random.Next(0, 9)];
                            Book.Tags.Add(tag);
                        }

                        user.Books.Add(Book);
                    }
                }

                ForumMessage message = new ForumMessage()
                {
                    BookId = 1,
                    UserId = "admin",
                    Text = "В Африке, если человек на 80% состоит из воды, то считается, что он из благополучной семьи",
                };

                ForumMessage message1 = new ForumMessage()
                {
                    BookId = 1,
                    UserId = "admin",
                    Text = "Умер Гулливер… и лилипуты тихо сказали «Еб@ть копать…»",
                    ParrentMessage = new ForumMessage
                    {
                        BookId = 1,
                        UserId = "admin",
                        Text = "Когда вы плачете — никто не видит ваши слезы. Когда вы счастливы — никто не заметит вашу улыбку. Но попробуйте только пукнуть",
                        ParrentMessage = new ForumMessage
                        {
                            BookId = 1,
                            UserId = "admin",
                            Text = "Черепашки—ниндзя нападали вчетвером на одного, потому что у них тренер был крыса",
                            ParrentMessage = new ForumMessage
                            {
                                BookId = 1,
                                UserId = "admin",
                                Text = "Девушка не вовремя сделала каменное лицо и утонула",
                                ParrentMessage = new ForumMessage
                                {
                                    BookId = 1,
                                    UserId = "admin",
                                    Text = "Митинг косоглазых состоялся на сорок метров левее здания городской администрации",
                                }
                            }
                        }
                    }
                };

                await context.ForumMessages.AddRangeAsync(message, message1);

                await context.SaveChangesAsync();
            }
        }

    }
}