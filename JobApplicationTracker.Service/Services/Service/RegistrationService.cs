using JobApplicationTracker.Api.Enums;
using JobApplicationTracker.Data.DataModels;
using JobApplicationTracker.Data.Dtos.Responses;
using JobApplicationTracker.Data.Interface;
using JobApplicationTracker.Service.DTO.Requests;
using JobApplicationTracker.Service.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using System;

namespace JobApplicationTracker.Service.Services.Service
{
    public class RegistrationService : IRegistrationService
    {
        private readonly IUserRepository _userRepository;
        private readonly ICompaniesRepository _companiesRepository;
        private readonly IPasswordHasherService _passwordHasherService; // Added for password hashing

        public RegistrationService(IUserRepository userRepository, ICompaniesRepository companiesRepository, IPasswordHasherService passwordHasherService)
        {
            _userRepository = userRepository;
            _companiesRepository = companiesRepository;
            _passwordHasherService = passwordHasherService; // Initialize password hasher
        }

        public async Task<ResponseDto> RegisterUserAsync(RegisterDto request)
        {
            // Check for duplicate email
            if (await _userRepository.GetUserByEmail(request.Email) != null)
            {
                return new ResponseDto
                {
                    IsSuccess = false,
                    StatusCode = StatusCodes.Status400BadRequest,
                    Message = "Email is already registered."
                };
            }

            // Check for duplicate phone number (optional field)
            if (!string.IsNullOrWhiteSpace(request.PhoneNumber))
            {
                if (await _userRepository.GetUserByPhone(request.PhoneNumber) != null)
                {
                    return new ResponseDto
                    {
                        IsSuccess = false,
                        StatusCode = StatusCodes.Status400BadRequest,
                        Message = "This phone number is already in use. Try logging in."
                    };
                }
            }

            // Hash the password before saving
            var hashedPassword = _passwordHasherService.HashPassword(request.Password);

            if (request.CompanyDto != null)
            {
                var createdCompanyId = await _companiesRepository.CreateCompanyAsync(request.CompanyDto);
                // Add user if company created
                var companyUser = new UsersDataModel
                {
                    FirstName = request.FirstName,
                    LastName = request.LastName,
                    PhoneNumber = request.PhoneNumber,
                    PasswordHash = hashedPassword, // Use hashed password
                    CompanyId = createdCompanyId,
                    Email = request.Email,
                    UserType = (int)UserTypes.Company,
                    CreatedAt = request.CreateDateTime ?? DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                int createdUserId = await _userRepository.CreateUserAsync(companyUser);
                if (createdUserId < 1)
                {
                    return new ResponseDto
                    {
                        IsSuccess = false,
                        StatusCode = StatusCodes.Status500InternalServerError,
                        Message = "Registration failed. Try again later."
                    };
                }

                return new ResponseDto
                {
                    IsSuccess = true,
                    StatusCode = StatusCodes.Status200OK,
                    Message = "Registered successfully."
                };
            }
            else
            {
                // Handle job seeker (no company)
                var newUser = new UsersDataModel
                {
                    Email = request.Email,
                    FirstName = request.FirstName,
                    LastName = request.LastName,
                    PhoneNumber = request.PhoneNumber,
                    PasswordHash = hashedPassword, // Use hashed password
                    Location = request.Location,
                    UserType = (int)UserTypes.JobSeeker,
                    CreatedAt = request.CreateDateTime ?? DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                int userId = await _userRepository.CreateUserAsync(newUser);
                if (userId < 1)
                {
                    return new ResponseDto
                    {
                        IsSuccess = false,
                        StatusCode = StatusCodes.Status500InternalServerError,
                        Message = "Registration failed. Try again later."
                    };
                }

                return new ResponseDto
                {
                    IsSuccess = true,
                    StatusCode = StatusCodes.Status200OK,
                    Message = "Registered successfully."
                };
            }
        }
    }
}