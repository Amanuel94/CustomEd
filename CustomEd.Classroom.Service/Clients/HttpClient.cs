using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace CusotmEd.Classroom.Service.Clients;
public class ClassroomApiClient
{
    private readonly HttpClient _httpClient;

    public ClassroomApiClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }
    public async Task<bool> CheckCourseTaughtByTeacher(string email, string courseNumber)
    {
        var response = await _httpClient.GetAsync($"schooldb/teachers/courses/{email}");
        if (response.IsSuccessStatusCode)
        {
            bool isTaught = await response.Content.ReadFromJsonAsync<bool>();
            return isTaught;
        }

        return false;
    }

        


    
}
