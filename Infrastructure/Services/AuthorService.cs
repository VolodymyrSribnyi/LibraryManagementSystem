using Application.ErrorHandling;
using Abstractions.Repositories;
using Application.DTOs.Authors;
using Application.Services.Interfaces;
using AutoMapper;
using Domain.Entities;
using Microsoft.Extensions.Logging;


namespace Infrastructure.Services
{
    public class AuthorService : IAuthorService
    {
        private readonly IAuthorRepository _authorRepository;
        private readonly ILogger<AuthorService> _logger;
        private readonly IMapper _mapper;
        public AuthorService(IAuthorRepository authorRepository, IMapper mapper, ILogger<AuthorService> logger)
        {
            _authorRepository = authorRepository;
            _mapper = mapper;
            _logger = logger;
        }
        public async Task<Result<GetAuthorDTO>> AddAsync(CreateAuthorDTO createAuthorDTO)
        {
            if (createAuthorDTO == null)
            {
                _logger.LogWarning($"{Errors.NullData.Code} AddAsync called with null CreateAuthorDTO");
                return Result<GetAuthorDTO>.Failure(Errors.NullData);
            }

            if (string.IsNullOrWhiteSpace(createAuthorDTO.FirstName) ||
                string.IsNullOrWhiteSpace(createAuthorDTO.Surname) ||
                string.IsNullOrEmpty(createAuthorDTO.Description) ||
                createAuthorDTO.Age <= 0)
            {
                _logger.LogWarning($"{Errors.InvalidData.Code} CreateAuthorDTO contains invalid data");
                return Result<GetAuthorDTO>.Failure(Errors.InvalidData);
            }

            var authorToCreate = _mapper.Map<Author>(createAuthorDTO);
            var existingAuthor = await _authorRepository.GetByIdAsync(authorToCreate.Id);

            if (existingAuthor != null)
            {
                _logger.LogInformation($"{Errors.AuthorExists.Code} Author with id {authorToCreate.Id} already exists");
                return Result<GetAuthorDTO>.Failure(Errors.AuthorExists);
            }

            var author = await _authorRepository.AddAsync(authorToCreate);

            if (author == null)
            {
                _logger.LogWarning($"Failed to create author with name {createAuthorDTO.FirstName} {createAuthorDTO.Surname}");
                throw new InvalidOperationException($"Failed to create author with name {createAuthorDTO.FirstName} {createAuthorDTO.Surname}");
            }

            _logger.LogInformation($"Successfully added author with id {author.Id}");
            var authorDTO = _mapper.Map<GetAuthorDTO>(author);

            return Result<GetAuthorDTO>.Success(authorDTO);
        }
        public async Task<Result<IEnumerable<GetAuthorDTO>>> GetAllAsync()
        {
            var authors = await _authorRepository.GetAllAsync();

            if (authors == null || authors.Any() == false)
            {
                _logger.LogInformation("No authors found in repository");
                return Result<IEnumerable<GetAuthorDTO>>.Success(Enumerable.Empty<GetAuthorDTO>());
            }

            return Result<IEnumerable<GetAuthorDTO>>.Success(_mapper.Map<IEnumerable<GetAuthorDTO>>(authors));
        }
        public async Task<Result<GetAuthorDTO>> GetByIdAsync(Guid id)
        {
            if (id == Guid.Empty)
            {
                _logger.LogWarning("GetByIdAsync called with empty GUID");
                return Result<GetAuthorDTO>.Failure(Errors.NullData);
            }

            var author = await _authorRepository.GetByIdAsync(id);

            if (author == null)
            {
                _logger.LogInformation($"Author with id {id} not found");
                return Result<GetAuthorDTO>.Failure(Errors.AuthorNotFound);
            }

            return Result<GetAuthorDTO>.Success(_mapper.Map<GetAuthorDTO>(author));
        }
        public async Task<Result<GetAuthorDTO>> UpdateAsync(UpdateAuthorDTO updateAuthorDTO)
        {
            if (updateAuthorDTO == null)
            {
                _logger.LogWarning("UpdateAsync called with null UpdateAuthorDTO");
                return Result<GetAuthorDTO>.Failure(Errors.NullData);
            }

            var authorExists = await _authorRepository.GetByIdAsync(updateAuthorDTO.Id);

            if (authorExists == null)
            {
                _logger.LogInformation($"Author with id {updateAuthorDTO.Id} not found");
                return Result<GetAuthorDTO>.Failure(Errors.AuthorNotFound);
            }

            _mapper.Map(updateAuthorDTO, authorExists);
            var author = await _authorRepository.UpdateAsync(authorExists);

            if (author == null)
            {
                _logger.LogWarning($"Failed to update author with id {authorExists.Id}");
                throw new InvalidOperationException($"Failed to update author with id {authorExists.Id}");
            }

            return Result<GetAuthorDTO>.Success(_mapper.Map<GetAuthorDTO>(authorExists));
        }
        public async Task<Result<bool>> DeleteAsync(Guid id)
        {
            if (id == Guid.Empty)
            {
                _logger.LogWarning("DeleteAsync called with empty GUID");
                return Result<bool>.Failure(Errors.NullData);
            }

            var author = await _authorRepository.GetByIdAsync(id);

            if (author == null)
            {
                _logger.LogInformation($"Author with id {id} not found");
                return Result<bool>.Failure(Errors.AuthorNotFound);
            }

            var result = await _authorRepository.DeleteAsync(author.Id);

            if (!result)
            {
                _logger.LogWarning($"Failed to delete author with id {id}");
                throw new InvalidOperationException($"Failed to delete author with id {id}");
            }

            _logger.LogInformation("Successfully deleted author with ID: {authorId}", id);
            return Result<bool>.Success(true);
        }

        public async Task<Result<GetAuthorDTO>> GetBySurnameAsync(string surname)
        {
            if (string.IsNullOrWhiteSpace(surname))
            {
                _logger.LogWarning("GetBySurnameAsync called with null or empty surname");
                return Result<GetAuthorDTO>.Failure(Errors.NullData);
            }

            var author = await _authorRepository.GetByFullNameAsync(surname);

            if (author == null)
            {
                _logger.LogInformation($"Author with surname {surname} not found");
                return Result<GetAuthorDTO>.Failure(Errors.AuthorNotFound);
            }

            return Result<GetAuthorDTO>.Success(_mapper.Map<GetAuthorDTO>(author));
        }
        public UpdateAuthorDTO MapToUpdateAuthorDTO(GetAuthorDTO getAuthorDTO)
        {
            return _mapper.Map<UpdateAuthorDTO>(getAuthorDTO);
        }
    }
}
