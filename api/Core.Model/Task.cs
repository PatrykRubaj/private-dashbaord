namespace Core.Model;

public class Task
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public WeekDay DaysOfTheWeek { get; set; }
    public int TaskCategoryId { get; set; }
    public int Position { get; set; }
    public required string OwnerId { get; set; }
    
    public required TaskCategory TaskCategory { get; set; }
}