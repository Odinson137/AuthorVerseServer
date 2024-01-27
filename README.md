**`Description`**

The idea of the website is to create a unified platform where authors can create various types of books, including text-only books, books with images, manga, manhwa, and audiobooks. The main feature of the project is the ability to branch the plot. Authors can create branching not only within a single chapter but also throughout their entire book, and they can create links to other books. This feature allows authors to create extensive works where each writer develops the branching part they want.

The development was carried out by a team of 3 people. From the initial idea, I planned it to be a significant project, a gem in my portfolio. I suggested the project to a friend who works with React and another .Net developer. The first month was spent designing in Figma, and the link is provided below. The basic pages and all possible functions were outlined. Project implementations and functional clarifications were discussed. Then the development phase began, with separate teams working on the back end and front end. The back end was completed 100% ahead of the front end.

The project was conceived as educational, so instead of simple solutions, more complex ones or their analogs in different frameworks were used. Docker was used to learn microservices architecture, creating several pods, including two .Net applications: one for the main server and another for the forum for each book. The two services interact with each other using gRPC and Redis for data transfer and storage. Identity framework was used for registration and authorization. Redis was used instead of the built-in cache and for other important parts of the application to reduce database queries.

**`Figma images`**

![main page](https://github.com/Odinson137/AuthorVerseServer/assets/87028237/0fa797bb-c7b7-43f3-bfd8-e0a5ae85b1dd)


**`Build`**

To run the project, you need Docker and an internet connection to download the Git repository. Once you download the repository, navigate to the project folder and enter the following command in the console:

```
docker-compose up --build
```

This command installs all the necessary images for the project, followed by the creation of the containers. After that, you can access this address:
```
http://localhost:7069/swagger/index.html
```
where you can find all possible endpoints for the project.

The project also includes work with GraphQL, but not for the entire project. The project was developed for teamwork and purely for learning purposes. Access GraphQL requests through:
```
http://localhost:5288/graphql
```
which opens a sandbox page to experiment with sending requests to the server.

**`Links`**

Figma link for the project design and features:
```
https://www.figma.com/file/6iWHrnGmfTOCgTHi0ATXIL?type=design
```
React client repository:
```
https://github.com/Yarik76/AuthorVerseClient.git
```

**`Stack`**

Technologies used:
.Net 8, EF, Identity, Redis, Docker, GraphQL, gRPC, MS SQL, MVC, SignalR, Repository pattern, DI, Unit tests, Integration tests, Stress tests

**`End`**

That's it. Good luck with your work!
