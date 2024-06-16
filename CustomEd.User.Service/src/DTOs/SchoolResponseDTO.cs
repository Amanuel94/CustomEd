using CustomEd.Shared.Model;
using System.Text.Json.Serialization;

namespace CustomEd.User.Dto;


public class SchoolResponseDto
{
    [JsonPropertyName("userExists")]
    public bool UserExists { get; set; }

    [JsonPropertyName("role")]
    public Role Role { get; set; }
}
