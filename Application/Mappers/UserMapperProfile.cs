using Application.DTOs.Users;
using AutoMapper;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Mappers
{
    public class UserMapperProfile : Profile
    {
        public UserMapperProfile()
        {
            CreateMap<CreateUserDTO, ApplicationUser>();
            CreateMap<ApplicationUser,GetUserDTO>();
            CreateMap<UpdateUserDTO, ApplicationUser>();
        }
    }
}
