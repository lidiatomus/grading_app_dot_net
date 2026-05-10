using System.Text.Json;
using Siemens.Internship2026.GradeBook.Interfaces;
using Siemens.Internship2026.GradeBook.Models;

namespace Siemens.Internship2026.GradeBook.Repositories;

public class GradeRepository : IGradeRepository
{
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _configuration;

    public GradeRepository(HttpClient httpClient, IConfiguration configuration)
    {
        _httpClient = httpClient;
        _configuration = configuration;
    }

    public async Task<Grade?> GetByIdAsync(int id)
    {
        var grades = await GetAllAsync();

        return grades.FirstOrDefault(g => g.Id == id && g.IsActive);
    }

    public async Task<IEnumerable<Grade>> GetAllAsync()
    {
        var url = _configuration["GradeSource:Url"];

        if (string.IsNullOrWhiteSpace(url))
        {
            return Enumerable.Empty<Grade>();
        }

        var response = await _httpClient.GetAsync(url);

        if (!response.IsSuccessStatusCode)
        {
            return Enumerable.Empty<Grade>();
        }

        var json = await response.Content.ReadAsStringAsync();

        var grades = JsonSerializer.Deserialize<List<Grade>>(
            json,
            new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

        return grades ?? Enumerable.Empty<Grade>();
    }
}