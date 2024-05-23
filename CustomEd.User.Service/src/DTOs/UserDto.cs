using CustomEd.Shared.JWT.Contracts;
using CustomEd.Shared.Model;

namespace CustomEd.User.DTO.Dto
{
    public class SubUserDto: UserDto
    {
        public string? StudentId {get; set;}
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public DateOnly? DateOfBirth { get; set; }
        public Department? Department { get; set; }
        public string? PhoneNumber {get; set;}
        public DateOnly? JoinDate { get; set; }
        public int? Year { get; set; }
        public string? Section { get; set; }

    }
    
}