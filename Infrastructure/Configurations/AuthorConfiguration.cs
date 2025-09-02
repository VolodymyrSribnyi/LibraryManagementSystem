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
    public class AuthorConfiguration : IEntityTypeConfiguration<Author>
    {
        public void Configure(EntityTypeBuilder<Author> builder)
        {
            builder.ToTable(nameof(Author));
            builder.HasKey(a => a.Id);
            builder.Property(a => a.Id)
                .ValueGeneratedOnAdd()
                .IsRequired();
            builder.Property(a => a.FirstName)
                .HasMaxLength(50)
                .IsRequired();
            builder.Property(a => a.Surname)
                .HasMaxLength(50)
                .IsRequired();
            builder.Property(a => a.CreatedAt)
                .ValueGeneratedOnAdd();

            builder.Property(a => a.MiddleName)
                .HasMaxLength(50)
                .IsRequired(false);

            builder.HasMany(a => a.Books)
                .WithOne(b => b.Author)
                .HasForeignKey(b => b.AuthorId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
