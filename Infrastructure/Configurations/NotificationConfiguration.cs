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
    public class NotificationConfiguration : IEntityTypeConfiguration<Notification>
    {
        public void Configure(EntityTypeBuilder<Notification> builder)
        {
            builder.ToTable(nameof(Notification));
            builder.HasKey(n => n.Id);
            builder.Property(n => n.Id)
                .ValueGeneratedOnAdd()
                .IsRequired();
            builder.Property(n => n.Message)
                .HasMaxLength(500)
                .IsRequired();
            builder.Property(n => n.CreatedAt)
                .ValueGeneratedOnAdd()
                .IsRequired();
            builder.Property(n => n.IsRead)
                .HasDefaultValue(false)
                .IsRequired();
            builder.Property(n => n.NotificationType)
                .IsRequired();
        }
    }
}
