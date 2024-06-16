using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using CustomEd.User.Dto;
using CustomEd.User.Service.DTOs;

public class UserApiClient
{
    private readonly HttpClient _httpClient;

    public UserApiClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<SchoolResponseDto?> CheckEmailExistsAsync(string email)
    {
        
        try
        {
            var response = await _httpClient.GetAsync($"schooldb/emails/{email}");
            response.EnsureSuccessStatusCode();
            Console.WriteLine(response.Content.ReadAsStringAsync().Result);
            var schoolResponse = await response.Content.ReadFromJsonAsync<SchoolResponseDto>();
            Console.WriteLine(schoolResponse!.UserExists);

            return schoolResponse;
            
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred: {ex.Message}");
            return null;
        }
    }

    public async Task<List<string>> FetchStudentProfile(string email)
    {
        try
        {
            var response = await _httpClient.GetAsync($"schooldb/students/profile/{email}");
            response.EnsureSuccessStatusCode();

            var schoolResponse = await response.Content.ReadFromJsonAsync<List<string>>();
            return schoolResponse;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred: {ex.Message}");
            return null;
        }
    }

    public async Task<List<string>> FetchTeacherProfile(string email)
    {
        try
        {
            var response = await _httpClient.GetAsync($"schooldb/teachers/profile/{email}");
            response.EnsureSuccessStatusCode();
            var schoolResponse = await response.Content.ReadFromJsonAsync<List<string>>();
            return schoolResponse;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred: {ex.Message}");
            return null;
        }
    }

    public async Task<List<StudentDto>> FetchStudentProfiles()
    {
        try
        {
            var response = await _httpClient.GetAsync($"schooldb/fetch-database/students");
            response.EnsureSuccessStatusCode();
            var schoolResponse = await response.Content.ReadFromJsonAsync<List<StudentDto>>();
            return schoolResponse;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred: {ex.Message}");
            return null;
        }
    }

    public async Task<List<TeacherDto>> FetchTeacherProfiles()
    {
        try
        {
            var response = await _httpClient.GetAsync($"schooldb/fetch-database/teachers");
            response.EnsureSuccessStatusCode();
            var schoolResponse = await response.Content.ReadFromJsonAsync<List<TeacherDto>>();
            return schoolResponse;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred: {ex.Message}");
            return null;
        }
    }
}
