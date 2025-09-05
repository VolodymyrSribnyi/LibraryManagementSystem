using Abstractions.Repositories;
using Application.DTOs.Authors;
using Application.Services.Interfaces;
using AutoMapper;
using Domain.Entities;
using Domain.Exceptions;
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
        private readonly IMapper _mapper;
        public AuthorService(IAuthorRepository authorRepository,IMapper mapper)
        {
            _authorRepository = authorRepository;
            _mapper = mapper;
        }
        public async Task<GetAuthorDTO> AddAsync(CreateAuthorDTO createAuthorDTO)
        {
            var authorToCreate = _mapper.Map<Author>(createAuthorDTO);

            var author = await _authorRepository.AddAsync(authorToCreate);

            if(author == null)
                throw new InvalidOperationException("Failed to create author");

            return _mapper.Map<GetAuthorDTO>(author);
        }
        public async Task<IEnumerable<GetAuthorDTO>> GetAllAsync()
        {
            var authors = await _authorRepository.GetAllAsync();

            return _mapper.Map<IEnumerable<GetAuthorDTO>>(authors);
        }
        public async Task<GetAuthorDTO> GetByIdAsync(Guid id)
        {
            var author = await _authorRepository.GetByIdAsync(id);

            if (author == null)
                throw new AuthorNotFoundException(id);

            return _mapper.Map<GetAuthorDTO>(author);
        }
        public async Task<GetAuthorDTO> UpdateAsync(UpdateAuthorDTO updateAuthorDTO)
        {
            var authorExists = await _authorRepository.GetByIdAsync(updateAuthorDTO.Id);

            if (authorExists == null)
                throw new AuthorNotFoundException(updateAuthorDTO.Id);

            _mapper.Map(updateAuthorDTO, authorExists);

            var author = await _authorRepository.UpdateAsync(authorExists);

            return _mapper.Map<GetAuthorDTO>(authorExists);
        }
        public async Task<bool> DeleteAsync(Guid id)
        {
            var author = await _authorRepository.GetByIdAsync(id);

            if (author == null)
                throw new AuthorNotFoundException(id);

            await _authorRepository.DeleteAsync(author.Id);

            return true;
        }


        public async Task<GetAuthorDTO> GetByFullNameAsync(string surname)
        {
            var author = await _authorRepository.GetByFullNameAsync(surname);

            if (author == null)
                throw new AuthorNotFoundException(surname);

            return _mapper.Map<GetAuthorDTO>(author);
        }
    }
}
