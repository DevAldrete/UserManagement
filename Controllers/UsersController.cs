using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UserManagement.Contracts;
using UserManagement.Models;

namespace UserManagement.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private readonly UserManagementDbContext _dbContext;
    private readonly ILogger<UsersController> _logger;

    public UsersController(UserManagementDbContext dbContext, ILogger<UsersController> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<UserResponse>>> GetUsers(CancellationToken cancellationToken)
    {
        var users = await _dbContext.Users
            .OrderBy(user => user.LastName)
            .ThenBy(user => user.FirstName)
            .Select(user => user.ToResponse())
            .ToListAsync(cancellationToken);

        return Ok(users);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<UserResponse>> GetUser(Guid id, CancellationToken cancellationToken)
    {
        var user = await _dbContext.Users.FindAsync(new object?[] { id }, cancellationToken);

        if (user is null)
        {
            return NotFound();
        }

        return Ok(user.ToResponse());
    }

    [HttpPost]
    public async Task<ActionResult<UserResponse>> CreateUser(CreateUserRequest request, CancellationToken cancellationToken)
    {
        var emailInUse = await _dbContext.Users.AnyAsync(user => user.Email == request.Email, cancellationToken);
        if (emailInUse)
        {
            ModelState.AddModelError(nameof(request.Email), "Email is already in use.");
            return ValidationProblem(ModelState);
        }

        var user = new User
        {
            FirstName = request.FirstName,
            LastName = request.LastName,
            Email = request.Email,
            PhoneNumber = request.PhoneNumber,
            Notes = request.Notes
        };

        _dbContext.Users.Add(user);
        await _dbContext.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("User {UserId} created", user.Id);

        return CreatedAtAction(nameof(GetUser), new { id = user.Id }, user.ToResponse());
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<UserResponse>> UpdateUser(Guid id, UpdateUserRequest request, CancellationToken cancellationToken)
    {
        var user = await _dbContext.Users.FindAsync(new object?[] { id }, cancellationToken);
        if (user is null)
        {
            return NotFound();
        }

        var emailInUse = await _dbContext.Users
            .Where(existing => existing.Id != id)
            .AnyAsync(existing => existing.Email == request.Email, cancellationToken);

        if (emailInUse)
        {
            ModelState.AddModelError(nameof(request.Email), "Email is already in use.");
            return ValidationProblem(ModelState);
        }

        user.ApplyUpdates(request);
        await _dbContext.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("User {UserId} updated", user.Id);

        return Ok(user.ToResponse());
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteUser(Guid id, CancellationToken cancellationToken)
    {
        var user = await _dbContext.Users.FindAsync(new object?[] { id }, cancellationToken);
        if (user is null)
        {
            return NotFound();
        }

        _dbContext.Users.Remove(user);
        await _dbContext.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("User {UserId} deleted", user.Id);

        return NoContent();
    }
}
