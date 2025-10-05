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
    public class ApplicationUserConfiguration : IEntityTypeConfiguration<ApplicationUser>
    {
        public void Configure(EntityTypeBuilder<ApplicationUser> builder)
        {
            builder.ToTable(nameof(ApplicationUser));

            builder.HasKey(u => u.Id);

            builder.Property(u => u.Id)
                .ValueGeneratedOnAdd()
                .IsRequired();

            builder.Property(u => u.FirstName).HasMaxLength(50).IsRequired(false);

            builder.Property(u => u.Surname).HasMaxLength(50).IsRequired(false);

            builder.Property(u => u.CreatedAt)
               .ValueGeneratedOnAdd();

            builder.Property(u => u.Email)
                .HasMaxLength(100)
                .IsRequired();

            builder.Property(u => u.MiddleName)
                .HasMaxLength(50)
                .IsRequired(false);

            builder.HasMany(u => u.ReservedBooks)
                .WithOne(r => r.User)
                .HasForeignKey(r => r.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(u => u.LibraryCard)
                .WithOne(lc => lc.User)
                .HasForeignKey<LibraryCard>(lc => lc.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(u => u.Notifications)
                .WithOne(n => n.User)
                .HasForeignKey(n => n.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(u => u.BookSubscriptions)
                .WithOne(bnr => bnr.User)
                .HasForeignKey(bnr => bnr.UserId)
                .OnDelete(DeleteBehavior.Cascade);



        }
    }
}
