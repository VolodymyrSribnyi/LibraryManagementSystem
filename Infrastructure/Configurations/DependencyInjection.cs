using Abstractions.Repositories;
using Application.Mappers;
using Application.Services.Implementations;
using Application.Services.Interfaces;
using AutoMapper;
using Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Configurations
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services,IConfiguration configuration)
        {
            services.AddScoped<IAuthorRepository,AuthorRepository>();
            services.AddScoped<IBookRepository, BookRepository>();
            services.AddScoped<ILibraryCardRepository, LibraryCardRepository>();
            services.AddScoped<IReservingBookRepository,ReservingBookRepository>();
            services.AddScoped<INotificationRepository, NotificationRepository>();

            services.AddScoped<IAuthorService, AuthorService>();
            services.AddScoped<IBookService, BookService>();
            services.AddScoped<ILibraryCardService, LibraryCardService>();
            services.AddScoped<IReservingBookService, ReservingBookService>();
            services.AddScoped<INotificationService, NotificationService>();
            services.AddScoped<IUserService, UserService>();

            services.AddScoped<AuthorMapperProfile>();
            services.AddScoped<BookMapperProfile>();
            services.AddScoped<LibraryCardMapperProfile>();
            services.AddScoped<ReservingBookMapperProfile>();
            services.AddScoped<NotificationMapperProfile>();
            services.AddScoped<UserMapperProfile>();

            services.AddDbContext<LibraryContext>(options =>
            {
                options.UseSqlServer(configuration.GetConnectionString("DefaultConnection"));
            });
            return services;
        }
    }
}
