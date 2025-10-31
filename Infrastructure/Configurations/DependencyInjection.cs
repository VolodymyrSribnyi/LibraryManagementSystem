using Abstractions.Repositories;
using Application.EventHadlers;
using Application.Mappers;

using Application.Services.Interfaces;
using AutoMapper;
using Domain.Abstractions.Repositories;
using Domain.Entities;
using Domain.Events;
using Infrastructure.Events;
using Infrastructure.Repositories;
using Infrastructure.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.Configurations
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<LibraryContext>(options =>
            {
                options.UseSqlServer(configuration.GetConnectionString("DefaultConnection"));
            });

            services.AddIdentity<ApplicationUser, IdentityRole<Guid>>()
                .AddEntityFrameworkStores<LibraryContext>()
                .AddDefaultTokenProviders();

             
            services.AddAutoMapper(cfg =>
            {
                cfg.AddProfile<AuthorMapperProfile>();
                cfg.AddProfile<BookMapperProfile>();
                cfg.AddProfile<LibraryCardMapperProfile>();
                cfg.AddProfile<ReservingBookMapperProfile>();
                cfg.AddProfile<NotificationMapperProfile>();
                cfg.AddProfile<UserMapperProfile>();
                cfg.AddProfile<BookNotificationRequestMapperProfile>();
            });

            services.AddScoped<IAuthorRepository, AuthorRepository>();
            services.AddScoped<IBookRepository, BookRepository>();
            services.AddScoped<ILibraryCardRepository, LibraryCardRepository>();
            services.AddScoped<IReservingBookRepository, ReservingBookRepository>();
            services.AddScoped<INotificationRepository, NotificationRepository>();
            services.AddScoped<IBookNotificationRequestRepository, BookNotificationRequestRepository>();

            services.AddScoped<IAuthorService, AuthorService>();
            services.AddScoped<IBookService, BookService>();
            services.AddScoped<ILibraryCardService, LibraryCardService>();
            services.AddScoped<IReservingBookService, ReservingBookService>();
            services.AddScoped<INotificationService, NotificationService>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IBookNotificationRequestService, BookNotificationRequestService>();

            services.AddScoped<IDomainEventHandler<BookBecameAvailableEvent>, BookAvailabilityNotificationHandler>();
            services.AddScoped<IDomainEventPublisher, DomainEventPublisher>();

            return services;
        }
    }
}
