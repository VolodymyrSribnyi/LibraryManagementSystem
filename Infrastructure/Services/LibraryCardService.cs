using Abstractions.Repositories;
using Application.DTOs.LibraryCards;
using Application.ErrorHandling;
using Application.Services.Interfaces;
using AutoMapper;
using Domain.Entities;
using Domain.Exceptions;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;


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

        public async Task<Result<GetLibraryCardDTO>> CreateAsync(Guid userId)
        {
            if (userId == Guid.Empty)
            {
                _logger.LogError("UserId is empty");
                return Result<GetLibraryCardDTO>.Failure(Errors.NullData);
            }

            var libraryCardToCreate = new LibraryCard
            {
                UserId = userId,
                IsValid = true,
                ValidTo = DateTime.UtcNow.AddYears(1)
            };

            if (await _libraryCardRepository.IsExistsAsync(userId))
            {
                _logger.LogWarning($"User with id {userId} has library card");
                return Result<GetLibraryCardDTO>.Failure(Errors.LibraryCardExists);
            }

            var createdLibraryCard = await _libraryCardRepository.CreateAsync(libraryCardToCreate);

            if (createdLibraryCard == null)
            {
                throw new InvalidOperationException("Failed to create library card.");
            }

            _logger.LogInformation("Library card successfully created");
            return Result<GetLibraryCardDTO>.Success(_mapper.Map<GetLibraryCardDTO>(createdLibraryCard));
        }


        public async Task<Result> DeleteAsync(Guid userId)
        {
            if (userId == Guid.Empty)
            {
                _logger.LogError("UserId is empty");
                return Result<GetLibraryCardDTO>.Failure(Errors.NullData);
            }

            var libraryCardToDelete = await _libraryCardRepository.GetByUserIdAsync(userId);

            if (libraryCardToDelete == null)
            {
                _logger.LogWarning($"Library card with userId {userId} not found.");
                return Result.Failure(Errors.LibraryCardNotFound);
            }

            var result = await _libraryCardRepository.DeleteAsync(libraryCardToDelete.Id);

            if (!result)
            {
                throw new Exception("Failed to delete library card.");
            }

            _logger.LogInformation("Library card successfully deleted");
            return Result.Success();
        }

        public async Task<Result<IEnumerable<GetLibraryCardDTO>>> GetAllAsync()
        {
            var libraryCards = await _libraryCardRepository.GetAllAsync();

            if (libraryCards == null || !libraryCards.Any())
            {
                _logger.LogWarning("No library cards found.");
                return Result<IEnumerable<GetLibraryCardDTO>>.Success(Enumerable.Empty<GetLibraryCardDTO>());
            }

            return Result<IEnumerable<GetLibraryCardDTO>>.Success(_mapper.Map<IEnumerable<GetLibraryCardDTO>>(libraryCards));
        }

        public async Task<Result<DateTime?>> GetExpirationDateAsync(Guid userId)
        {
            if (userId == Guid.Empty)
            {
                _logger.LogError("UserId is empty");
                return Result<DateTime?>.Failure(Errors.NullData);
            }

            var libraryCard = await _libraryCardRepository.GetByUserIdAsync(userId);

            if (libraryCard == null)
            {
                _logger.LogWarning($"Library card with userId {userId} not found.");
                return Result<DateTime?>.Failure(Errors.LibraryCardNotFound);
            }

            return Result<DateTime?>.Success(libraryCard.ValidTo);
        }

        public async Task<Result> IsActiveAsync(Guid userId)
        {
            if (userId == Guid.Empty)
            {
                _logger.LogError("UserId is empty");
                return Result<GetLibraryCardDTO>.Failure(Errors.NullData);
            }

            var libraryCard = await _libraryCardRepository.GetByUserIdAsync(userId);

            if (libraryCard == null)
            {
                _logger.LogWarning($"Library card with userId {userId} not found.");
                return Result.Failure(Errors.LibraryCardNotFound);
            }

            if (!libraryCard.IsValid || libraryCard.ValidTo <= DateTime.UtcNow)
            {
                _logger.LogInformation($"Library card with userId {userId} is not active.");
                return Result.Failure(Errors.LibraryCardExpired);
            }

            return Result.Success();
        }
        public async Task<Result<GetLibraryCardDTO>> UpdateAsync(UpdateLibraryCardDTO updateLibraryCardDTO)
        {
            if (updateLibraryCardDTO == null)
            {
                _logger.LogError("UpdateLibraryCardDTO is null");
                return Result<GetLibraryCardDTO>.Failure(Errors.NullData);
            }

            var libraryCard = await _libraryCardRepository.GetByUserIdAsync(updateLibraryCardDTO.UserId);

            if (libraryCard == null)
            {
                _logger.LogWarning($"Library card with userId {updateLibraryCardDTO.UserId} not found.");
                return Result<GetLibraryCardDTO>.Failure(Errors.LibraryCardNotFound);
            }

            _mapper.Map(updateLibraryCardDTO, libraryCard);

            var updatedLibraryCard = await _libraryCardRepository.UpdateAsync(libraryCard);

            if (updatedLibraryCard == null)
            {
                _logger.LogWarning("Failed to update library card.");
                return Result<GetLibraryCardDTO>.Failure(Errors.InvalidData);
            }

            _logger.LogInformation("Successfully updated library card");
            return Result<GetLibraryCardDTO>.Success(_mapper.Map<GetLibraryCardDTO>(updatedLibraryCard));
        }
        public async Task<Result<bool>> IsExistsAsync(Guid userId)
        {
            if (userId == Guid.Empty)
            {
                _logger.LogError("UserId is empty");
                return Result<bool>.Failure(Errors.NullData);
            }

            var exists = await _libraryCardRepository.IsExistsAsync(userId);
            return Result<bool>.Success(exists);
        }
    }
}
