using Application.DTOs.Authors;
using Application.Services.Interfaces;
using AutoMapper;
using Domain.Entities;
using Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services.Implementations
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

            var auhtor = await _authorRepository.AddAsync(authorToCreate);

            return _mapper.Map<GetAuthorDTO>(auhtor);
        }
        public async Task<IEnumerable<GetAuthorDTO>> GetAllAsync()
        {
            var authors = await _authorRepository.GetAllAsync();

            return _mapper.Map<IEnumerable<GetAuthorDTO>>(authors);
        }
        public async Task<GetAuthorDTO> GetByIdAsync(Guid id)
        {
            var author = await _authorRepository.GetByIdAsync(id);
            //if (author == null) 
            //    throw new NotFoundException("Author not found");

            return _mapper.Map<GetAuthorDTO>(author);
        }
        public async Task<GetAuthorDTO> UpdateAsync(UpdateAuthorDTO updateAuthorDTO)
        {
            var authorExists = await _authorRepository.GetByIdAsync(updateAuthorDTO.Id);

            if(authorExists == null)
                throw new Exception("Author not found");

            _mapper.Map(updateAuthorDTO, authorExists);

            var author = await _authorRepository.UpdateAsync(authorExists);

            //if (author == null) throw new NotFoundException("Author not found");
            return _mapper.Map<GetAuthorDTO>(authorExists);
        }
        public async Task<bool> DeleteAsync(Guid id)
        {
            var author = await _authorRepository.GetByIdAsync(id);
            //if (author == null) throw new NotFoundException("Author not found");

            await _authorRepository.DeleteAsync(author.Id);

            return true;
        }


        public async Task<GetAuthorDTO> GetByFullNameAsync(string surname)
        {
            var author = await _authorRepository.GetByFullNameAsync(surname);

            //if (author == null) throw new NotFoundException("Author not found");

            return _mapper.Map<GetAuthorDTO>(author);
        }
    }
}
