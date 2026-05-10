using Siemens.Internship2026.GradeBook.Interfaces;
using Siemens.Internship2026.GradeBook.Models;

namespace Siemens.Internship2026.GradeBook.Services;

public class GradeService : IGradeService
{
    private readonly IGradeRepository _gradeRepository;

    public GradeService(IGradeRepository gradeRepository)
    {
        _gradeRepository = gradeRepository;
    }

    public async Task<IEnumerable<Grade>> GetAllAsync()
    {
        return await _gradeRepository.GetAllAsync();
    }

    public async Task<Grade?> GetByIdAsync(int id)
    {
        if (id <= 0)
        {
            return null;
        }

        return await _gradeRepository.GetByIdAsync(id);
    }

    public async Task<IEnumerable<Grade>> GetFirstPassingActiveGradesAsync(int count)
    {
        if (count <= 0)
        {
            return Enumerable.Empty<Grade>();
        }

        var grades = await _gradeRepository.GetAllAsync();

        return grades
            .Where(g => g.Value >= 5 && g.IsActive)
            .Take(count);
    }
}