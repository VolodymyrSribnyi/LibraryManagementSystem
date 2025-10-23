using Domain.Entities;
using Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;


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

            builder.Property(b => b.AuthorId)
                .IsRequired();

            builder.Property(b => b.Genre)
                .IsRequired();

            builder.Property(b => b.PublishingYear)
                .IsRequired();

            builder.Property(b => b.CreatedAt)
                .ValueGeneratedOnAdd();

            builder.Property(b => b.IsAvailable)
                .HasDefaultValue(true);

            builder.Property(b => b.Rating)
                .HasDefaultValue(Rating.NotRated)
                .IsRequired();

            builder.Property(builder => builder.Description)
                .HasMaxLength(2000)
                .IsRequired();

            builder.Property(b => b.IsDeleted)
                .HasDefaultValue(false)
                .IsRequired();


        }
    }
}


