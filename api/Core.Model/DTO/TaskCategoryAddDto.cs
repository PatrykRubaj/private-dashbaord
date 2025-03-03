using System.ComponentModel.DataAnnotations;

namespace Core.Model.DTO;

public class TaskCategoryAddDto
{
    public required string CategoryName { get; set; }
    public TimeOnly? StartTime { get; set; }
    public TimeOnly? EndTime { get; set; }
}