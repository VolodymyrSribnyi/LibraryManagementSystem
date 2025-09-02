using Application.DTOs.LibraryCards;
using AutoMapper;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Mappers
{
    public class LibraryCardMapperProfile : Profile
    {
        public LibraryCardMapperProfile()
        {
            CreateMap<LibraryCard, GetLibraryCardDTO>();
            CreateMap<CreateLibraryCardDTO, LibraryCard>();
            CreateMap<UpdateLibraryCardDTO, LibraryCard>();
            CreateMap<LibraryCard, UpdateLibraryCardDTO>();
        }
    }
}
