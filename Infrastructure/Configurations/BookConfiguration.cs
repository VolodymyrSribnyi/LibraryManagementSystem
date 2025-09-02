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
    public class BookConfiguration : IEntityTypeConfiguration<Book>
    {
        public void Configure(EntityTypeBuilder<Book> builder)
        {
            builder.ToTable(nameof(Book));

            builder.HasKey(b => b.Id);

            builder.Property(b => b.Id)
                .ValueGeneratedOnAdd()
                .IsRequired();

            builder.Property(b => b.Title)
                .HasMaxLength(200)
                .IsRequired();

            builder.Property(b => b.Publisher)
                .HasMaxLength(100)
                .IsRequired();

            builder.Property(b => b.Author)
                .IsRequired();

            builder.Property(b => b.Genre)
                .IsRequired();

            builder.Property(b => b.PublishingYear)
                .IsRequired();

            builder.Property(b => b.CreatedAt)
                .ValueGeneratedOnAdd();

            builder.Property(b => b.LastUpdatedAt)
                .ValueGeneratedOnAddOrUpdate();

            builder.Property(b => b.IsAvailable)
                .HasDefaultValue(true);

            builder.Property(b => b.Rating)
                .HasDefaultValue(0.0)
                .IsRequired();

            builder.Property(b => b.IsDeleted)
                .HasDefaultValue(false)
                .IsRequired();


        }
    }
}


