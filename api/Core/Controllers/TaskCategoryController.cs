using Core.DataAccess;
using Core.Model;
using Core.Model.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Core.Controllers;

[ApiController]
[Route("[controller]")]
public class TaskCategoryController: ControllerBase
{
    private readonly DataContext _dataContext;

    public TaskCategoryController(DataContext dataContext)
    {
        _dataContext = dataContext;
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<TaskCategory>> GetById(int id)
    {
        var result = await _dataContext.TaskCategories.FirstOrDefaultAsync(tc => tc.Id == id);
        return result == null ? NotFound() : Ok(result);
    }

    [HttpGet]
    public async Task<ActionResult<IList<TaskCategory>>> GetAll()
    {
        return await _dataContext.TaskCategories.ToListAsync();
    }

    [HttpPost]
    [Authorize]
    public async Task<ActionResult> Add([FromBody] TaskCategoryAddDto taskCategory)
    {
        var newTaskCategory = new TaskCategory()
        {
            Name = taskCategory.CategoryName,
            StartTime = taskCategory.StartTime,
            EndTime = taskCategory.EndTime,
            OwnerId = 1
        };
        
        await _dataContext.TaskCategories.AddAsync(newTaskCategory);

        await _dataContext.SaveChangesAsync();
        
        return Ok(newTaskCategory);
    }
}