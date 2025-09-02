using Application.DTOs.Notitfications;
using AutoMapper;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Mappers
{
    public class NotificationMapperProfile : Profile
    {
        public NotificationMapperProfile()
        {
            CreateMap<CreateNotificationDTO, Notification>();
            CreateMap<Notification, GetNotificationDTO>();
            CreateMap<UpdateStatusNotificationDTO,Notification>();
        }
    }
}
