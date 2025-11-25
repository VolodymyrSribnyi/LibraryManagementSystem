using Application.DTOs.Authors;
using AutoMapper;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Mappers
{
    public class AuthorMapperProfile : Profile
    {
        public AuthorMapperProfile() 
        {
            CreateMap<CreateAuthorDTO, Author>();
            CreateMap<Author,GetAuthorDTO>()
                .ForMember(dest => dest.Books, opt => opt.MapFrom(src => src.Books));
            CreateMap<UpdateAuthorDTO, Author>();
            CreateMap<GetAuthorDTO, UpdateAuthorDTO>();
            CreateMap<GetAuthorDTO, Author>()
                .ForMember(dest => dest.Books, opt => opt.Ignore());

        }
    }
}
