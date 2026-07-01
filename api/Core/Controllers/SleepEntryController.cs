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
public class SleepEntryController : ControllerBase
{
    private readonly DataContext _dataContext;
    private readonly UserManager<IdentityUser> _userManager;

    public SleepEntryController(DataContext dataContext, UserManager<IdentityUser> userManager)
    {
        _dataContext = dataContext;
        _userManager = userManager;
    }

    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<SleepEntry>> GetById(int id)
    {
        var userId = _userManager.GetUserId(User) ?? throw new NoNullAllowedException();

        var result = await _dataContext.SleepEntries
            .FirstOrDefaultAsync(s => s.Id == id && s.OwnerId == userId && !s.IsDeleted);

        return result == null ? NotFound() : Ok(result);
    }

    [HttpGet("range")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<IList<SleepEntry>>> GetRange([FromQuery] SleepEntryRangeDto range)
    {
        var userId = _userManager.GetUserId(User) ?? throw new NoNullAllowedException();

        var result = await _dataContext.SleepEntries
            .Where(s => s.OwnerId == userId && !s.IsDeleted && s.Date >= range.From && s.Date <= range.Until)
            .OrderByDescending(s => s.Date)
            .ToListAsync();

        return Ok(result);
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<SleepEntry>> Add(SleepEntryAddDto sleepEntry)
    {
        var userId = _userManager.GetUserId(User) ?? throw new NoNullAllowedException();

        var dateAlreadyExists = await _dataContext.SleepEntries
            .AnyAsync(s => s.OwnerId == userId && !s.IsDeleted && s.Date == sleepEntry.Date);

        if (dateAlreadyExists)
        {
            return Conflict();
        }

        var newSleepEntry = new SleepEntry
        {
            Date = sleepEntry.Date,
            Start = sleepEntry.Start.UtcDateTime,
            Until = sleepEntry.Until.UtcDateTime,
            SleepHours = sleepEntry.SleepHours,
            RechargePercent = sleepEntry.RechargePercent,
            CreditPercent = sleepEntry.CreditPercent,
            DebtPercent = sleepEntry.DebtPercent,
            SleepPercent = sleepEntry.SleepPercent,
            RemHours = sleepEntry.RemHours,
            RemPercent = sleepEntry.RemPercent,
            DeepHours = sleepEntry.DeepHours,
            DeepPercent = sleepEntry.DeepPercent,
            Bpm = sleepEntry.Bpm,
            BpmPercent = sleepEntry.BpmPercent,
            SleepRating = sleepEntry.SleepRating,
            OwnerId = userId
        };

        await _dataContext.SleepEntries.AddAsync(newSleepEntry);
        await _dataContext.SaveChangesAsync();

        return Ok(newSleepEntry);
    }

    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(int id)
    {
        var userId = _userManager.GetUserId(User) ?? throw new NoNullAllowedException();

        var entry = await _dataContext.SleepEntries
            .FirstOrDefaultAsync(s => s.Id == id && s.OwnerId == userId && !s.IsDeleted);

        if (entry == null)
        {
            return NotFound();
        }

        entry.IsDeleted = true;
        entry.DeletedAt = DateTimeOffset.UtcNow;

        await _dataContext.SaveChangesAsync();

        return NoContent();
    }
}
