namespace Core.Model;

public class TaskCategory
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public int Position { get; set; }
    public TimeOnly? StartTime { get; set; }
    public TimeOnly? EndTime { get; set; }
    
    public int OwnerId { get; set; }
}