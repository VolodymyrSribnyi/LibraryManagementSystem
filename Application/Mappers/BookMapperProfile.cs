using Application.DTOs.Books;
using Application.Services.Interfaces;
using AutoMapper;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Mappers
{
    public class BookMapperProfile : Profile
    {
        public BookMapperProfile()
        {
            CreateMap<CreateBookDTO, Book>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.PictureBlobName, opt => opt.Ignore());
            CreateMap<Book, GetBookDTO>()
                .ForMember(dest => dest.PictureUrl, opt => opt.Ignore());
            CreateMap<GetBookDTO, Book>()
                /*.ForMember(dest => dest.Author).Ignore()*/;
            CreateMap<UpdateBookDTO, Book>();
            CreateMap<Book, UpdateBookDTO>();

        }
    }
}
