using Application.DTOs.Books;
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
            CreateMap<CreateBookDTO, Book>();
            CreateMap<Book, GetBookDTO>();
            CreateMap<UpdateBookDTO, Book>();
            CreateMap<Book, UpdateBookDTO>();
        }
    }
}
