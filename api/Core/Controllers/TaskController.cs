using Core.DataAccess;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Core.Controllers;

[ApiController]
[Route("[controller]")]
public class TaskController : ControllerBase
{
    private readonly DataContext _dataContext;

    public TaskController(DataContext dataContext)
    {
        _dataContext = dataContext;
    }

    [Route("{id}")]
    public async Task<ActionResult<Task>> GetTask(int id)
    {
        var result = await _dataContext.Tasks.FirstOrDefaultAsync(t => t.Id == id);

        return result == null ? NotFound() : Ok(result);
    }
}