namespace Core.Model;

public class TaskCategory
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public int Position { get; set; }
    public TimeOnly? StartTime { get; set; }
    public TimeOnly? EndTime { get; set; }
    public required string OwnerId { get; set; }

    public ICollection<Task> Tasks { get; set; } = new List<Task>();
}