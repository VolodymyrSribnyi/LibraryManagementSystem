using Application.DTOs.LibraryCards;
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
    public class LibraryCardService : ILibraryCardService
    {
        private readonly ILibraryCardRepository _libraryCardRepository;
        private readonly IMapper _mapper;

        public LibraryCardService(ILibraryCardRepository libraryCardRepository,IMapper mapper)
        {
            _libraryCardRepository = libraryCardRepository;
            _mapper = mapper;
        }

        public async Task<GetLibraryCardDTO> CreateAsync(Guid userId)
        {
            var libraryCardToCreate = new LibraryCard
            {
                UserId = userId,
                IsValid = true,
                ValidTo = DateTime.UtcNow.AddYears(1)
            };

            var createdLibraryCard = await _libraryCardRepository.CreateAsync(libraryCardToCreate);

            if (createdLibraryCard == null)
            {
                throw new Exception("Failed to create library card.");
            }

            return _mapper.Map<GetLibraryCardDTO>(createdLibraryCard);
        }


        public async Task<bool> DeleteAsync(Guid userId)
        {
            var libraryCardToDelete = await _libraryCardRepository.GetByUserIdAsync(userId);

            if (libraryCardToDelete == null)
            {
                throw new Exception("Library card not found.");
            }

            var result = await _libraryCardRepository.DeleteAsync(libraryCardToDelete.Id);

            if (!result)
            {
                throw new Exception("Failed to delete library card.");
            }
            return true;
        }

        public async Task<IEnumerable<GetLibraryCardDTO>> GetAllAsync()
        {
            var libraryCards = await _libraryCardRepository.GetAllAsync();

            if (libraryCards == null || !libraryCards.Any())
            {
                throw new Exception("No library cards found.");
            }

            return _mapper.Map<IEnumerable<GetLibraryCardDTO>>(libraryCards);
        }

        public async Task<DateTime?> GetExpirationDateAsync(Guid userId)
        {
            var libraryCard = await _libraryCardRepository.GetByUserIdAsync(userId);

            if (libraryCard == null)
            {
                throw new Exception("Library card not found.");
            }

            return libraryCard.ValidTo;
        }

        public async Task<bool> IsActiveAsync(Guid userId)
        {
            var libraryCard = await _libraryCardRepository.GetByUserIdAsync(userId);

            if (libraryCard == null)
            {
                throw new Exception("Library card not found.");
            }

            return libraryCard.IsValid && libraryCard.ValidTo > DateTime.UtcNow;
        }
        public async Task<GetLibraryCardDTO> UpdateAsync(UpdateLibraryCardDTO updateLibraryCardDTO)
        {
            var libraryCard = await _libraryCardRepository.GetByUserIdAsync(updateLibraryCardDTO.UserId);

            if (libraryCard == null)
            {
                throw new Exception("Library card not found.");
            }

            _mapper.Map(updateLibraryCardDTO, libraryCard);

            var updatedLibraryCard = await _libraryCardRepository.UpdateAsync(libraryCard);

            if (updatedLibraryCard == null)
            {
                throw new Exception("Failed to update library card.");
            }

            return _mapper.Map<GetLibraryCardDTO>(updatedLibraryCard);
        }
    }
}
