using CustomEd.User.Service.Model;

namespace CustomEd.User.Service.DTOs;

public class StudentDto
{
    public Guid Id { get; set; }
    public string? StudentId { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string Email { get; set; } = null!;
    public DateOnly? DateOfBirth { get; set; }
    public string? PhoneNumber { get; set; }
    public DateOnly? JoinDate { get; set; }
    public int? Year { get; set; }
    public string? Section { get; set; }
    public Role Role { get; set; }
    public Department? Department { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public bool IsVerified { get; set; }
}
