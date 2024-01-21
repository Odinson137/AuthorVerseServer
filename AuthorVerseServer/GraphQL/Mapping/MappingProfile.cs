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
        
        CreateMap<Book, QueryBookDTO>()
            .ForMember(dest => dest.AuthorUserName, 
                opt => opt.MapFrom(src => src.Author.UserName))
            .ForMember(dest => dest.AuthorName, 
                opt => opt.MapFrom(src => src.Author.Name))
            .ForMember(dest => dest.AuthorLastName, 
                opt => opt.MapFrom(src => src.Author.LastName))
            .ForMember(dest => dest.AuthorUserName, 
                opt => opt.MapFrom(src => src.Author.UserName))
            .ForMember(dest => dest.AuthorDescription, 
                opt => opt.MapFrom(src => src.Author.Description))
            .ForMember(dest => dest.AuthorEmail, 
                opt => opt.MapFrom(src => src.Author.Email))
            .ForMember(dest => dest.AuthorLogoUrl, 
                opt => opt.MapFrom(src => src.Author.LogoUrl));
    }
}