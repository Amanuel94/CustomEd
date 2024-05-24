using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using CustomEd.User.Dto;

public class UserApiClient
{
    private readonly HttpClient _httpClient;

    public UserApiClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<SchoolResonseDto?> CheckEmailExistsAsync(string email)
    {
        try
        {
            var response = await _httpClient.GetAsync($"schooldb/emails/{email}");
            response.EnsureSuccessStatusCode();
            
            var schoolResponse = await response.Content.ReadFromJsonAsync<SchoolResonseDto>();
            return schoolResponse;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred: {ex.Message}");
            return null;
        }
    }
}
