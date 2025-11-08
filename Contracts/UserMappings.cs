using UserManagement.Models;

namespace UserManagement.Contracts;

public static class UserMappings
{
    public static UserResponse ToResponse(this User user) => new(
        user.Id,
        user.FirstName,
        user.LastName,
        user.Email,
        user.PhoneNumber,
        user.Notes,
        user.IsActive,
        user.CreatedAt,
        user.UpdatedAt
    );

    public static void ApplyUpdates(this User user, UpdateUserRequest request)
    {
        user.FirstName = request.FirstName;
        user.LastName = request.LastName;
        user.Email = request.Email;
        user.PhoneNumber = request.PhoneNumber;
        user.Notes = request.Notes;
        user.IsActive = request.IsActive;
    }
}
