using System.ComponentModel.DataAnnotations;

namespace Core.Model.DTO;

public class TaskCategoryUpdateDto
{
    public string? CategoryName { get; set; }
    public TimeOnly? StartTime { get; set; }
    public TimeOnly? EndTime { get; set; }
    public int? Position { get; set; }
}