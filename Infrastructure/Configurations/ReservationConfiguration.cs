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
    public class ReservationConfiguration : IEntityTypeConfiguration<Reservation>
    {
        public void Configure(EntityTypeBuilder<Reservation> builder)
        {
            builder.ToTable(nameof(Reservation));

            builder.HasKey(r => r.Id);

            builder.Property(r => r.Id)
                .ValueGeneratedOnAdd()
                .IsRequired();

            builder.Property(r => r.UserId)
                .IsRequired();

            builder.Property(r => r.BookId)
                .IsRequired();

            builder.Property(r => r.ReservedAt)
                .ValueGeneratedOnAdd();

            builder.Property(r => r.EndsAt)
                .IsRequired()
                .HasDefaultValueSql("DATEADD(DAY, 14, GETUTCDATE())");

            builder.Property(r => r.IsReturned)
                .HasDefaultValue(false)
                .IsRequired();
        }
    }
}
