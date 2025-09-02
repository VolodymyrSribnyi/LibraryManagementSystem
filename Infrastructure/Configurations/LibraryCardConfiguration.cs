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
    public class LibraryCardConfiguration : IEntityTypeConfiguration<LibraryCard>
    {
        public void Configure(EntityTypeBuilder<LibraryCard> builder)
        {
            builder.ToTable(nameof(LibraryCard));

            builder.HasKey(lc => lc.Id);

            builder.Property(lc => lc.Id)
                .ValueGeneratedOnAdd()
                .IsRequired();

            builder.Property(lc => lc.UserId)
                .IsRequired();

            builder.Property(lc => lc.IsValid)
                .HasDefaultValue(true)
                .IsRequired();

            builder.Property(lc => lc.ValidTo)
                .IsRequired()
                .HasDefaultValue(DateTime.UtcNow.AddYears(1)); 
        }
    }
}
