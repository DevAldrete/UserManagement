using System.ComponentModel.DataAnnotations;

namespace UserManagement.Contracts;

public record CreateUserRequest(
    [Required][MaxLength(100)] string FirstName,
    [Required][MaxLength(100)] string LastName,
    [Required][EmailAddress][MaxLength(256)] string Email,
    [Phone][MaxLength(32)] string? PhoneNumber,
    [MaxLength(512)] string? Notes
);

public record UpdateUserRequest(
    [Required][MaxLength(100)] string FirstName,
    [Required][MaxLength(100)] string LastName,
    [Required][EmailAddress][MaxLength(256)] string Email,
    [Phone][MaxLength(32)] string? PhoneNumber,
    [MaxLength(512)] string? Notes,
    bool IsActive
);

public record UserResponse(
    Guid Id,
    string FirstName,
    string LastName,
    string Email,
    string? PhoneNumber,
    string? Notes,
    bool IsActive,
    DateTimeOffset CreatedAt,
    DateTimeOffset UpdatedAt
);
