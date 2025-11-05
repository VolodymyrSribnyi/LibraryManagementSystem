using Abstractions.Repositories;
using Application.DTOs.Authors;
using Application.Services.Interfaces;
using AutoMapper;
using Domain.Entities;
using FluentAssertions;
using Infrastructure.Services;
using Microsoft.Extensions.Logging;
using Moq;


namespace InfrastructureTests.Services
{
    public class AuthorServiceTests
    {
        private readonly AuthorService _authorService;
        private readonly Mock<IAuthorRepository> _authorRepositoryMock;
        private readonly Mock<ILogger<AuthorService>> _loggerMock;
        private readonly Mock<IMapper> _mapperMock;

        public AuthorServiceTests()
        {
            _loggerMock = new Mock<ILogger<AuthorService>>();
            _mapperMock = new Mock<IMapper>();
            _authorRepositoryMock = new Mock<IAuthorRepository>();
            _authorService = new AuthorService(_authorRepositoryMock.Object, _mapperMock.Object, _loggerMock.Object);
        }

        [Fact]
        public async Task AddAsync_NullCreateAuthorDTO_ReturnsFailureResult()
        {
            // Arrange
            Author authorToCreate = null;
            CreateAuthorDTO createAuthorDTO = null;

            _authorRepositoryMock.Setup(ar => ar.AddAsync(authorToCreate)).ReturnsAsync((Author)null);

            // Act
            var result = await _authorService.AddAsync(createAuthorDTO);
            // Assert
            Assert.False(result.IsSuccess);
        }
        [Fact]
        public async Task AddAsync_InvalidCreateAuthorDTO_ReturnsFailureResult()
        {
            // Arrange
            var createAuthorDTO = new CreateAuthorDTO
            {
                FirstName = "", // Invalid FirstName
                Surname = "Doe",
                Description = "An accomplished author.",
                Age = 45
            };
            // Act
            var result = await _authorService.AddAsync(createAuthorDTO);
            // Assert
            Assert.False(result.IsSuccess);
        }
        [Fact]
        public async Task AddAsync_ValidCreateAuthorDTO_ReturnsSuccessResult()
        {
            // Arrange
            var createAuthorDTO = new CreateAuthorDTO
            {
                FirstName = "John",
                Surname = "Doe",
                Description = "An accomplished author.",
                Age = 45
            };
            var authorToCreate = new Author
            {
                Id = Guid.NewGuid(),
                FirstName = createAuthorDTO.FirstName,
                Surname = createAuthorDTO.Surname,
                Description = createAuthorDTO.Description,
                Age = createAuthorDTO.Age
            };
            var createdAuthor = new Author
            {
                Id = authorToCreate.Id,
                FirstName = authorToCreate.FirstName,
                Surname = authorToCreate.Surname,
                Description = authorToCreate.Description,
                Age = authorToCreate.Age
            };
            var expectedGetAuthorDTO = new GetAuthorDTO
            {
                Id = createdAuthor.Id,
                FirstName = createdAuthor.FirstName,
                Surname = createdAuthor.Surname,
                Description = createdAuthor.Description,
                Age = createdAuthor.Age
            };

            _mapperMock.Setup(m => m.Map<Author>(createAuthorDTO)).Returns(authorToCreate);
            _authorRepositoryMock.Setup(ar => ar.GetByIdAsync(authorToCreate.Id)).ReturnsAsync((Author)null);
            _authorRepositoryMock.Setup(ar => ar.AddAsync(authorToCreate)).ReturnsAsync(createdAuthor);
            _mapperMock.Setup(m => m.Map<GetAuthorDTO>(createdAuthor)).Returns(expectedGetAuthorDTO);
            // Act
            var result = await _authorService.AddAsync(createAuthorDTO);
            // Assert
            Assert.True(result.IsSuccess);
            Assert.Equal(expectedGetAuthorDTO.Id, result.Value.Id);
        }
        [Fact]
        public async Task AddAsync_AuthorExists_ReturnsFailureResult()
        {
            // Arrange
            var createAuthorDTO = new CreateAuthorDTO
            {
                FirstName = "Jane",
                Surname = "Smith",
                Description = "A prolific writer.",
                Age = 50
            };

            var authorToCreate = new Author
            {
                Id = Guid.NewGuid(),
                FirstName = createAuthorDTO.FirstName,
                Surname = createAuthorDTO.Surname,
                Description = createAuthorDTO.Description,
                Age = createAuthorDTO.Age
            };
            var existingAuthor = new Author
            {
                Id = authorToCreate.Id,
                FirstName = authorToCreate.FirstName,
                Surname = authorToCreate.Surname,
                Description = authorToCreate.Description,
                Age = authorToCreate.Age
            };
            _mapperMock.Setup(m => m.Map<Author>(createAuthorDTO)).Returns(authorToCreate);


            _authorRepositoryMock.Setup(ar => ar.GetByIdAsync(authorToCreate.Id)).ReturnsAsync(existingAuthor);
            _authorRepositoryMock.Setup(ar => ar.AddAsync(authorToCreate)).ReturnsAsync(authorToCreate);

            // Act
            var result = await _authorService.AddAsync(createAuthorDTO);

            // Assert
            Assert.False(result.IsSuccess);
        }
        [Fact]
        public async Task AddAsync_RepositoryReturnsNull_ThrowsInvalidOperationException()
        {
            // Arrange
            var createAuthorDTO = new CreateAuthorDTO
            {
                FirstName = "John",
                Surname = "Doe",
                Description = "An accomplished author.",
                Age = 45
            };
            var authorToCreate = new Author
            {
                Id = Guid.NewGuid(),
                FirstName = createAuthorDTO.FirstName,
                Surname = createAuthorDTO.Surname,
                Description = createAuthorDTO.Description,
                Age = createAuthorDTO.Age
            };
            _mapperMock.Setup(m => m.Map<Author>(createAuthorDTO)).Returns(authorToCreate);
            _authorRepositoryMock.Setup(ar => ar.GetByIdAsync(authorToCreate.Id)).ReturnsAsync((Author)null);
            _authorRepositoryMock.Setup(ar => ar.AddAsync(authorToCreate)).ReturnsAsync((Author)null);
            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(() => _authorService.AddAsync(createAuthorDTO));
        }
        [Fact]
        public async Task GetAllAsync_NoAuthors_ReturnsEmptyCollection()
        {
            // Arrange
            _authorRepositoryMock.Setup(ar => ar.GetAllAsync()).ReturnsAsync(Enumerable.Empty<Author>());
            // Act
            var result = await _authorService.GetAllAsync();
            // Assert
            Assert.True(result.IsSuccess);
            Assert.Empty(result.Value);
        }
        [Fact]
        public async Task GetAllAsync_AuthorsExist_ReturnsAuthorsCollection()
        {
            // Arrange
            var authors = new List<Author>
            {
                new Author { Id = Guid.NewGuid(), FirstName = "Author1", Surname = "Surname1", Description = "Desc1", Age = 40 },
                new Author { Id = Guid.NewGuid(), FirstName = "Author2", Surname = "Surname2", Description = "Desc2", Age = 50 }
            };
            var expectedGetAuthorDTOs = new List<GetAuthorDTO>
            {
                new GetAuthorDTO { Id = authors[0].Id, FirstName = authors[0].FirstName, Surname = authors[0].Surname, Description = authors[0].Description, Age = authors[0].Age },
                new GetAuthorDTO { Id = authors[1].Id, FirstName = authors[1].FirstName, Surname = authors[1].Surname, Description = authors[1].Description, Age = authors[1].Age }
            };
            _authorRepositoryMock.Setup(ar => ar.GetAllAsync()).ReturnsAsync(authors);
            _mapperMock.Setup(m => m.Map<IEnumerable<GetAuthorDTO>>(authors)).Returns(expectedGetAuthorDTOs);
            // Act
            var result = await _authorService.GetAllAsync();
            // Assert
            Assert.True(result.IsSuccess);
            Assert.Equal(2, result.Value.Count());
        }
        [Fact]
        public async Task GetByIdAsync_AuthorExists_ReturnsAuthor()
        {
            // Arrange

            var author = new Author
            {
                Id = Guid.NewGuid(),
                FirstName = "Existing",
                Surname = "Author",
                Description = "An existing author.",
                Age = 60
            };
            var expectedGetAuthorDTO = new GetAuthorDTO
            {
                Id = author.Id,
                FirstName = author.FirstName,
                Surname = author.Surname,
                Description = author.Description,
                Age = author.Age
            };
            _authorRepositoryMock.Setup(ar => ar.GetByIdAsync(author.Id)).ReturnsAsync(author);
            _mapperMock.Setup(m => m.Map<GetAuthorDTO>(author)).Returns(expectedGetAuthorDTO);
            // Act
            var result = await _authorService.GetByIdAsync(author.Id);
            // Assert
            Assert.True(result.IsSuccess);
            Assert.Equal(expectedGetAuthorDTO.Id, result.Value.Id);
        }
        [Fact]
        public async Task GetByIdAsync_AuthorDoesNotExist_ReturnsFailureResult()
        {
            // Arrange
            var nonExistentAuthorId = Guid.NewGuid();
            _authorRepositoryMock.Setup(ar => ar.GetByIdAsync(nonExistentAuthorId)).ReturnsAsync((Author)null);
            // Act
            var result = await _authorService.GetByIdAsync(nonExistentAuthorId);
            // Assert
            Assert.False(result.IsSuccess);
        }
        [Fact]
        public async Task GetByIdAsync_NullGuid_ReturnsFailureResult()
        {
            // Arrange
            var nullGuid = Guid.Empty;
            // Act
            var result = await _authorService.GetByIdAsync(nullGuid);
            // Assert
            Assert.False(result.IsSuccess);
        }
        [Fact]
        public async Task UpdateAsync_NullUpdateAuthorDTO_ReturnsFailureResult()
        {
            // Arrange
            UpdateAuthorDTO updateAuthorDTO = null;
            // Act
            var result = await _authorService.UpdateAsync(updateAuthorDTO);
            // Assert
            Assert.False(result.IsSuccess);
        }
        [Fact]
        public async Task UpdateAsync_InvalidUpdateAuthorDTO_ReturnsFailureResult()
        {
            // Arrange
            var updateAuthorDTO = new UpdateAuthorDTO
            {
                Id = Guid.NewGuid(),
                FirstName = "", // Invalid FirstName
                Surname = "Doe",
                Description = "An accomplished author.",
                Age = 45
            };
            // Act
            var result = await _authorService.UpdateAsync(updateAuthorDTO);
            // Assert
            Assert.False(result.IsSuccess);

        }
        [Fact]
        public async Task UpdateAsync_ValidUpdateAuthorDTO_ReturnsSuccessResult()
        {
            // Arrange
            var updateAuthorDTO = new UpdateAuthorDTO
            {
                Id = Guid.NewGuid(),
                FirstName = "John",
                Surname = "Doe",
                Description = "An accomplished author.",
                Age = 45
            };
            var authorToUpdate = new Author
            {
                Id = updateAuthorDTO.Id,
                FirstName = updateAuthorDTO.FirstName,
                Surname = updateAuthorDTO.Surname,
                Description = updateAuthorDTO.Description,
                Age = (int)updateAuthorDTO.Age
            };
            var updatedAuthor = new Author
            {
                Id = authorToUpdate.Id,
                FirstName = authorToUpdate.FirstName,
                Surname = authorToUpdate.Surname,
                Description = authorToUpdate.Description,
                Age = authorToUpdate.Age
            };
            var expectedGetAuthorDTO = new GetAuthorDTO
            {
                Id = updatedAuthor.Id,
                FirstName = updatedAuthor.FirstName,
                Surname = updatedAuthor.Surname,
                Description = updatedAuthor.Description,
                Age = updatedAuthor.Age
            };
            _mapperMock.Setup(m => m.Map<Author>(updateAuthorDTO)).Returns(authorToUpdate);
            _authorRepositoryMock.Setup(ar => ar.GetByIdAsync(updateAuthorDTO.Id)).ReturnsAsync(authorToUpdate);
            _authorRepositoryMock.Setup(ar => ar.UpdateAsync(authorToUpdate)).ReturnsAsync(updatedAuthor);
            _mapperMock.Setup(m => m.Map<GetAuthorDTO>(updatedAuthor)).Returns(expectedGetAuthorDTO);
            // Act
            var result = await _authorService.UpdateAsync(updateAuthorDTO);
            // Assert
            Assert.True(result.IsSuccess);
        }
        [Fact]
        public async Task UpdateAsync_RepositoryReturnsNull_ThrowsInvalidOperationException()
        {
            // Arrange
            var updateAuthorDTO = new UpdateAuthorDTO
            {
                Id = Guid.NewGuid(),
                FirstName = "John",
                Surname = "Doe",
                Description = "An accomplished author.",
                Age = 45
            };
            var authorToUpdate = new Author
            {
                Id = updateAuthorDTO.Id,
                FirstName = updateAuthorDTO.FirstName,
                Surname = updateAuthorDTO.Surname,
                Description = updateAuthorDTO.Description,
                Age = (int)updateAuthorDTO.Age
            };
            _mapperMock.Setup(m => m.Map<Author>(updateAuthorDTO)).Returns(authorToUpdate);
            _authorRepositoryMock.Setup(ar => ar.GetByIdAsync(updateAuthorDTO.Id)).ReturnsAsync(authorToUpdate);
            _authorRepositoryMock.Setup(ar => ar.UpdateAsync(authorToUpdate)).ReturnsAsync((Author)null);
            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(() => _authorService.UpdateAsync(updateAuthorDTO));
        }
        [Fact]
        public async Task DeleteAsync_GuidEmpty_ReturnsFailureResult()
        {
            // Arrange
            var emptyGuid = Guid.Empty;
            // Act
            var result = await _authorService.DeleteAsync(emptyGuid);
            // Assert
            Assert.False(result.IsSuccess);
        }
        [Fact]
        public async Task DeleteAsync_ValidGuid_ReturnsSuccessResult()
        {
            // Arrange
            var authorId = Guid.NewGuid();
            _authorRepositoryMock.Setup(ar => ar.GetByIdAsync(authorId)).ReturnsAsync(new Author { Id = authorId });
            _authorRepositoryMock.Setup(ar => ar.DeleteAsync(authorId)).ReturnsAsync(true);
            // Act
            var result = await _authorService.DeleteAsync(authorId);
            // Assert
            Assert.True(result.IsSuccess);
        }
        [Fact]
        public async Task DeleteAsync_RepositoryReturnsFalse_ReturnsFailureResult()
        {
            // Arrange
            var authorId = Guid.NewGuid();
            _authorRepositoryMock.Setup(ar => ar.DeleteAsync(authorId)).ReturnsAsync(false);
            // Act
            var result = await _authorService.DeleteAsync(authorId);
            // Assert
            Assert.False(result.IsSuccess);
        }
        [Fact]
        public async Task DeleteAsync_RepositoryThrowsException_ThrowsException()
        {
            // Arrange
            var authorId = Guid.NewGuid();
            _authorRepositoryMock.Setup(ar => ar.GetByIdAsync(authorId)).ReturnsAsync(new Author { Id = authorId });
            _authorRepositoryMock.Setup(ar => ar.DeleteAsync(authorId)).ThrowsAsync( new InvalidOperationException("Failed to delete author with id"));
            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(() => _authorService.DeleteAsync(authorId));
        }
        [Fact]
        public async Task GetByFullName_EmptyFullName_ReturnsFailureResult()
        {
            // Arrange
            var emptyFullName = "";
            // Act
            var result = await _authorService.GetBySurnameAsync(emptyFullName);
            // Assert
            Assert.False(result.IsSuccess);
        }
        [Fact]
        public async Task GetByFullName_AuthorExists_ReturnsSuccessResult()
        {
            // Arrange
            var author = new Author
            {
                Id = Guid.NewGuid(),
                FirstName = "John",
                Surname = "Doe",
                Description = "An accomplished author.",
                Age = 45
            };
            var expectedGetAuthorDTO = new GetAuthorDTO
            {
                Id = author.Id,
                FirstName = author.FirstName,
                Surname = author.Surname,
                Description = author.Description,
                Age = author.Age
            };
            _authorRepositoryMock.Setup(ar => ar.GetByFullNameAsync(author.Surname)).ReturnsAsync(author);
            _mapperMock.Setup(m => m.Map<GetAuthorDTO>(author)).Returns(expectedGetAuthorDTO);
            // Act
            var result = await _authorService.GetBySurnameAsync(author.Surname);
            // Assert
            Assert.True(result.IsSuccess);
            Assert.Equal(expectedGetAuthorDTO.Id, result.Value.Id);
        }
    }
}
