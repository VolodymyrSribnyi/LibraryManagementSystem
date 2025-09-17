using Abstractions.Repositories;
using Application.DTOs.Authors;
using Application.DTOs.Books;
using Application.Services.Interfaces;
using AutoMapper;
using Domain.Entities;
using Domain.Exceptions;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Services
{
    public class AuthorService : IAuthorService
    {
        private readonly IAuthorRepository _authorRepository;
        private readonly ILogger<AuthorService> _logger;
        private readonly IMapper _mapper;
        public AuthorService(IAuthorRepository authorRepository, IMapper mapper,ILogger<AuthorService> logger)
        {
            _authorRepository = authorRepository;
            _mapper = mapper;
            _logger = logger;
        }
        public async Task<GetAuthorDTO> AddAsync(CreateAuthorDTO createAuthorDTO)
        {
            if (createAuthorDTO == null)
                throw new ArgumentNullException(nameof(createAuthorDTO));

            var authorToCreate = _mapper.Map<Author>(createAuthorDTO);
            var existingAuthor = await _authorRepository.GetByIdAsync(authorToCreate.Id);

            if (existingAuthor != null)
            {
                throw new AuthorExistsException($"Author {createAuthorDTO.FirstName} {createAuthorDTO.Surname} already exists");
            }

            var author = await _authorRepository.AddAsync(authorToCreate);

            if (author == null)
            {
                throw new InvalidOperationException($"Failed to create author with name {createAuthorDTO.FirstName} {createAuthorDTO.Surname}");
            }

            _logger.LogInformation($"Successfully added author with id {author.Id}");
            return _mapper.Map<GetAuthorDTO>(author);
        }
        public async Task<IEnumerable<GetAuthorDTO>> GetAllAsync()
        {
            var authors = await _authorRepository.GetAllAsync();

            if (authors == null || authors.Any() == false)
            {
                _logger.LogInformation("No authors found in repository");
                return Enumerable.Empty<GetAuthorDTO>();
            }

            return _mapper.Map<IEnumerable<GetAuthorDTO>>(authors);
        }
        public async Task<GetAuthorDTO> GetByIdAsync(Guid id)
        {
            if (id == Guid.Empty)
                throw new ArgumentNullException(nameof(id));

            var author = await _authorRepository.GetByIdAsync(id);

            if (author == null)
            {
                throw new AuthorNotFoundException($"Author with id {id} not found");
            }

            return _mapper.Map<GetAuthorDTO>(author);
        }
        public async Task<GetAuthorDTO> UpdateAsync(UpdateAuthorDTO updateAuthorDTO)
        {
            if (updateAuthorDTO == null)
                throw new ArgumentNullException(nameof(updateAuthorDTO));

            var authorExists = await _authorRepository.GetByIdAsync(updateAuthorDTO.Id);

            if (authorExists == null)
            {
                throw new AuthorNotFoundException($"Author with id {updateAuthorDTO.Id} not found");
            }

            _mapper.Map(updateAuthorDTO, authorExists);
            var author = await _authorRepository.UpdateAsync(authorExists);

            if (author == null)
            {
                throw new InvalidOperationException($"Failed to update author with id {authorExists.Id}");
            }

            return _mapper.Map<GetAuthorDTO>(authorExists);
        }
        public async Task<bool> DeleteAsync(Guid id)
        {
            if (id == Guid.Empty)
                throw new ArgumentNullException(nameof(id));

            var author = await _authorRepository.GetByIdAsync(id);

            if (author == null)
            {
                throw new AuthorNotFoundException($"Author with id {id} not found");
            }

            var result = await _authorRepository.DeleteAsync(author.Id);

            if (!result)
            {
                throw new InvalidOperationException($"Failed to delete author with id {id}");
            }

            _logger.LogInformation("Successfully deleted author with ID: {authorId}", id);
            return true;
        }

        public async Task<GetAuthorDTO> GetByFullNameAsync(string surname)
        {
            if (string.IsNullOrWhiteSpace(surname))
                throw new ArgumentNullException(nameof(surname));

            var author = await _authorRepository.GetByFullNameAsync(surname);

            if (author == null)
            {
                throw new AuthorNotFoundException($"Author with surname {surname} not found");
            }

            return _mapper.Map<GetAuthorDTO>(author);
        }
        public UpdateAuthorDTO MapToUpdateAuthorDTO(GetAuthorDTO getAuthorDTO)
        {
            return _mapper.Map<UpdateAuthorDTO>(getAuthorDTO);
        }
    }
}
