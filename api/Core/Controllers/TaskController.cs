using System.Data;
using Core.DataAccess;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Core.Controllers;

[ApiController]
[Authorize]
[Route("[controller]")]
public class TaskController : ControllerBase
{
    private readonly DataContext _dataContext;
    private readonly UserManager<IdentityUser> _userManager;

    public TaskController(DataContext dataContext, UserManager<IdentityUser> userManager)
    {
        _dataContext = dataContext;
        _userManager = userManager;
    }

    [Route("{id}")]
    public async Task<ActionResult<Task>> GetTask(int id)
    {
        var userId = _userManager.GetUserId(User) ?? throw new NoNullAllowedException();
        var result = await _dataContext.Tasks.FirstOrDefaultAsync(t => t.Id == id && t.OwnerId == userId);

        return result == null ? NotFound() : Ok(result);
    }
}