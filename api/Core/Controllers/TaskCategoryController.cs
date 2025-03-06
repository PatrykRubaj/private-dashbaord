using System.Data;
using Core.DataAccess;
using Core.Model;
using Core.Model.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Core.Controllers;

[ApiController]
[Authorize]
[Route("[controller]")]
public class TaskCategoryController: ControllerBase
{
    private readonly DataContext _dataContext;
    private readonly UserManager<IdentityUser> _userManager;

    public TaskCategoryController(DataContext dataContext, UserManager<IdentityUser> userManager)
    {
        _dataContext = dataContext;
        _userManager = userManager;
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<TaskCategory>> GetById(int id)
    {
        var userId = _userManager.GetUserId(User) ?? throw new NoNullAllowedException();
        
        var result = await _dataContext.TaskCategories.FirstOrDefaultAsync(tc => tc.Id == id && tc.OwnerId == userId);
        return result == null ? NotFound() : Ok(result);
    }

    [HttpGet]
    public async Task<ActionResult<IList<TaskCategory>>> GetAll()
    {
        var userId = _userManager.GetUserId(User) ?? throw new NoNullAllowedException();
        
        return await _dataContext.TaskCategories.Where(tc => tc.OwnerId == userId).ToListAsync();
    }

    [HttpPost]
    public async Task<ActionResult> Add([FromBody] TaskCategoryAddDto taskCategory)
    {
        var userId = _userManager.GetUserId(User) ?? throw new NoNullAllowedException();
        
        var newTaskCategory = new TaskCategory()
        {
            Name = taskCategory.CategoryName,
            StartTime = taskCategory.StartTime,
            EndTime = taskCategory.EndTime,
            OwnerId = userId
        };
        
        await _dataContext.TaskCategories.AddAsync(newTaskCategory);

        await _dataContext.SaveChangesAsync();
        
        return Ok(newTaskCategory);
    }
}