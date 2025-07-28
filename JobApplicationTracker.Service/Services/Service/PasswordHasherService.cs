using BCrypt.Net;
using JobApplicationTracker.Service.Services.Interfaces;

namespace JobApplicationTracker.Service.Services.Service
{
    public class PasswordHasherService : IPasswordHasherService
    {
        public string HashPassword(string password)
        {
            if (string.IsNullOrWhiteSpace(password))
            {
                throw new ArgumentException("Password cannot be null or empty.", nameof(password));
            }

            return BCrypt.Net.BCrypt.HashPassword(password);
        }

        public bool VerifyPassword(string password, string hashedPassword)
        {
            if (string.IsNullOrWhiteSpace(password) || string.IsNullOrWhiteSpace(hashedPassword))
            {
                throw new ArgumentException("Password and hashed password cannot be null or empty.");
            }

            return BCrypt.Net.BCrypt.Verify(password, hashedPassword);
        }
    }
}