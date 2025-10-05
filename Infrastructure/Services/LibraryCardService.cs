using Abstractions.Repositories;
using Application.DTOs.LibraryCards;
using Application.Services.Interfaces;
using AutoMapper;
using Domain.Entities;
using Domain.Exceptions;
using Microsoft.Extensions.Logging;


namespace Infrastructure.Services
{
    public class LibraryCardService : ILibraryCardService
    {
        private readonly ILibraryCardRepository _libraryCardRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<LibraryCardService> _logger;

        public LibraryCardService(ILibraryCardRepository libraryCardRepository, IMapper mapper, ILogger<LibraryCardService> logger)
        {
            _libraryCardRepository = libraryCardRepository;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<GetLibraryCardDTO> CreateAsync(Guid userId)
        {
            if (userId == Guid.Empty)
                throw new ArgumentNullException("userId");

            var libraryCardToCreate = new LibraryCard
            {
                UserId = userId,
                IsValid = true,
                ValidTo = DateTime.UtcNow.AddYears(1)
            };

            if (await _libraryCardRepository.IsExistsAsync(userId))
            {
                throw new LibraryCardExistsException($"User with id {userId} has library card");
            }

            var createdLibraryCard = await _libraryCardRepository.CreateAsync(libraryCardToCreate);

            if (createdLibraryCard == null)
            {
                throw new InvalidOperationException("Failed to create library card.");
            }

            _logger.LogInformation("Library card successfully created");
            return _mapper.Map<GetLibraryCardDTO>(createdLibraryCard);
        }


        public async Task<bool> DeleteAsync(Guid userId)
        {
            if (userId == Guid.Empty)
                throw new ArgumentNullException("userId");

            var libraryCardToDelete = await _libraryCardRepository.GetByUserIdAsync(userId);

            if (libraryCardToDelete == null)
            {
                throw new LibraryCardNotFoundException($"Library card with userId {userId} not found.");
            }

            var result = await _libraryCardRepository.DeleteAsync(libraryCardToDelete.Id);

            if (!result)
            {
                throw new Exception("Failed to delete library card.");
            }

            _logger.LogInformation("Library card successfully deleted");
            return true;
        }

        public async Task<IEnumerable<GetLibraryCardDTO>> GetAllAsync()
        {
            var libraryCards = await _libraryCardRepository.GetAllAsync();

            if (libraryCards == null || !libraryCards.Any())
            {
                _logger.LogWarning("No library cards found.");
                return Enumerable.Empty<GetLibraryCardDTO>();
            }

            return _mapper.Map<IEnumerable<GetLibraryCardDTO>>(libraryCards);
        }

        public async Task<DateTime?> GetExpirationDateAsync(Guid userId)
        {
            if (userId == Guid.Empty)
                throw new ArgumentNullException(nameof(userId));

            var libraryCard = await _libraryCardRepository.GetByUserIdAsync(userId);

            if (libraryCard == null)
            {
                throw new LibraryCardNotFoundException($"Library card with userId {userId} not found.");
            }

            return libraryCard.ValidTo;
        }

        public async Task<bool> IsActiveAsync(Guid userId)
        {
            if(userId == Guid.Empty)
                throw new ArgumentNullException(nameof(userId));

            var libraryCard = await _libraryCardRepository.GetByUserIdAsync(userId);

            if (libraryCard == null)
            {
                throw new LibraryCardNotFoundException($"Library card with userId {userId} not found.");
            }

            return libraryCard.IsValid && libraryCard.ValidTo > DateTime.UtcNow;
        }
        public async Task<GetLibraryCardDTO> UpdateAsync(UpdateLibraryCardDTO updateLibraryCardDTO)
        {
            if (updateLibraryCardDTO == null)
                throw new ArgumentNullException(nameof(updateLibraryCardDTO));

            var libraryCard = await _libraryCardRepository.GetByUserIdAsync(updateLibraryCardDTO.UserId);

            if (libraryCard == null)
            {
                throw new LibraryCardNotFoundException($"Library card with userId {updateLibraryCardDTO.UserId} not found.");
            }

            _mapper.Map(updateLibraryCardDTO, libraryCard);

            var updatedLibraryCard = await _libraryCardRepository.UpdateAsync(libraryCard);

            if (updatedLibraryCard == null)
            {
                throw new InvalidOperationException("Failed to update library card.");
            }

            _logger.LogInformation("Successfully updated library card");
            return _mapper.Map<GetLibraryCardDTO>(updatedLibraryCard);
        }
    }
}
