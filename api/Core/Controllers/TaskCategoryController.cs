using Core.DataAccess;
using Core.Model;
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
}