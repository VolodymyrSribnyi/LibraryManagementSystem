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
            CreateMap<CreateReservationDTO, Reservation>();
            CreateMap<Reservation, GetReservationDTO>();
            CreateMap<UpdateReservationStatusDTO, Reservation>();
        }
    }
}
