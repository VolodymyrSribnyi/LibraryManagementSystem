using Application.DTOs.Reservations;
using AutoMapper;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Mappers
{
    public class ReservingBookMapperProfile : Profile
    {
        public ReservingBookMapperProfile()
        {
            CreateMap<CreateReservationDTO, Reservation>()
                .ForMember(dest => dest.Book, opt => opt.Ignore())
                .ForMember(dest => dest.User, opt => opt.Ignore());

            CreateMap<Reservation, GetReservationDTO>()
                .ForMember(dest => dest.BookTitle, opt => opt.MapFrom(src => src.Book.Title));

            CreateMap<UpdateReservationStatusDTO, Reservation>();
                
        }
    }
}
