
using JobApplicationTracker.Data.DataModels;
using JobApplicationTracker.Data.Dto;
using JobApplicationTracker.Data.Dto.Responses;
using JobApplicationTracker.Data.Dtos.Responses;

namespace JobApplicationTracker.Data.Interface;

public interface IUserRepository
{
    Task<IEnumerable<UsersDto>> GetAllUsersAsync(int companyId);
    Task<UsersDto> GetUserByEmail(string email);
    Task<UsersDto> GetUsersByIdAsync(int userId);
    Task<ResponseDto> SubmitUsersAsync(UsersDataModel userDto);
    Task<int> CreateUserAsync(UsersDataModel userDto);
    Task<ResponseDto> DeleteUsersAsync(int userId);
    Task<bool> DoesEmailExists(string email);
    Task<UsersDto> GetUserByPhone(string phone);
    Task<UsersDataModel?> GetUserForLoginAsync(string email);
    Task<UserProfileDto> GetUserProfileAsync(int userId);
}