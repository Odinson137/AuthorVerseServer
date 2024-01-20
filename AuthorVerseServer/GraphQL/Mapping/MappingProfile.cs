using AuthorVerseServer.DTO;
using AuthorVerseServer.GraphQL.Types;
using AuthorVerseServer.Models;
using AutoMapper;

namespace AuthorVerseServer.GraphQL.Mapping;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<Book, QueryBookDTO>();
        CreateMap<User, QueryUserDTO>();
        
        CreateMap<Book, QueryBookDTOJust>()
            .ForPath(dest => dest.Author, 
                opt => opt.MapFrom(src => src.Author));
    }
}