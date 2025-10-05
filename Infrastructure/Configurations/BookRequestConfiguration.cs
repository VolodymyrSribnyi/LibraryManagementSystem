using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Configurations
{
    public class BookRequestConfiguration : IEntityTypeConfiguration<BookNotificationRequest>
    {
        public void Configure(EntityTypeBuilder<BookNotificationRequest> builder)
        {
            builder.ToTable("BookRequests");

            builder.HasKey(br => br.Id);
            builder.Property(br => br.Id)
                .ValueGeneratedOnAdd()
                .IsRequired();
            builder.Property(br => br.UserId)
                .IsRequired();
            builder.Property(br => br.BookId)
                .IsRequired();
            builder.Property(builder => builder.IsNotified)
                .HasDefaultValue(false)
                .IsRequired();
        }
    }
}
