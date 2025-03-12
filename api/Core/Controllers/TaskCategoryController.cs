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
public class TaskCategoryController : ControllerBase
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

        return await _dataContext.TaskCategories.Where(tc => tc.OwnerId == userId).OrderBy(tc => tc.Position)
            .ToListAsync();
    }

    [HttpPost]
    public async Task<ActionResult<TaskCategory>> Add([FromBody] TaskCategoryAddDto taskCategory)
    {
        var userId = _userManager.GetUserId(User) ?? throw new NoNullAllowedException();

        var newTaskCategory = new TaskCategory()
        {
            Name = taskCategory.CategoryName,
            StartTime = taskCategory.StartTime,
            EndTime = taskCategory.EndTime,
            OwnerId = userId
        };

        if (taskCategory.Position != null)
        {
            var taskCategoriesAfterTheNewOne =
                _dataContext.TaskCategories.Where(tc => tc.Position >= taskCategory.Position && tc.OwnerId == userId);

            foreach (var resortedTaskCategory in taskCategoriesAfterTheNewOne)
            {
                resortedTaskCategory.Position += 1;
            }

            newTaskCategory.Position = (int)taskCategory.Position;
        }
        else
        {
            if (await _dataContext.TaskCategories.AnyAsync(x => x.OwnerId == userId))
            {
                newTaskCategory.Position =
                    await _dataContext.TaskCategories.Where(tc => tc.OwnerId == userId).MaxAsync(tc => tc.Position) + 1;
            }
        }


        await _dataContext.TaskCategories.AddAsync(newTaskCategory);

        await _dataContext.SaveChangesAsync();

        return Ok(newTaskCategory);
    }

    [HttpPut("{taskCategoryId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<TaskCategory>> Update(int taskCategoryId,
        [FromBody] TaskCategoryUpdateDto taskCategoryChanges)
    {
        var userId = _userManager.GetUserId(User) ?? throw new NoNullAllowedException();

        var originalTaskCategory =
            await _dataContext.TaskCategories.FirstOrDefaultAsync(x => x.OwnerId == userId && x.Id == taskCategoryId);

        if (originalTaskCategory == null)
        {
            return NotFound();
        }

        //Currently can't clear StartTime and EndTime, because when they're null, the original value is used.
        originalTaskCategory.Name = taskCategoryChanges.CategoryName ?? originalTaskCategory.Name;
        originalTaskCategory.Position = taskCategoryChanges.Position ?? originalTaskCategory.Position;
        originalTaskCategory.StartTime = taskCategoryChanges.StartTime ?? originalTaskCategory.StartTime;
        originalTaskCategory.EndTime = taskCategoryChanges.EndTime ?? originalTaskCategory.EndTime;

        await _dataContext.SaveChangesAsync();

        return Ok(originalTaskCategory);
    }
}