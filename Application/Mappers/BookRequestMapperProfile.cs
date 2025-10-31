using Application.DTOs.BookNotificationRequests;
using AutoMapper;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Mappers
{
    public class BookNotificationRequestMapperProfile : Profile
    {
        public BookNotificationRequestMapperProfile()
        {
            CreateMap<BookNotificationRequest, BookNotificationRequestDTO>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.UserId))
                .ForMember(dest => dest.BookId, opt => opt.MapFrom(src => src.BookId));

            CreateMap<BookNotificationRequestDTO, BookNotificationRequest>()
                .ForMember(dest => dest.User, opt => opt.Ignore())
                .ForMember(dest => dest.Book, opt => opt.Ignore())
                .ForMember(dest => dest.IsNotified, opt => opt.Ignore());
        }

    }
}
