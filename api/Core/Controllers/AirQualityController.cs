using System.Data;
using Core.DataAccess;
using Core.Mapping;
using Core.Model;
using Core.Model.DTO;
using Core.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Core.Controllers;

[ApiController]
[Authorize]
[Route("[controller]")]
public class AirQualityController : ControllerBase
{
    private readonly DataContext _dataContext;
    private readonly IAirGradientClient _airGradientClient;

    public AirQualityController(DataContext dataContext, IAirGradientClient airGradientClient)
    {
        _dataContext = dataContext;
        _airGradientClient = airGradientClient;
    }

    [HttpGet("latest/{sensorId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<AirQualityReadingDto>> GetLatest(int sensorId)
    {
        var sensor = await _dataContext.Sensors.FindAsync(sensorId);
        if (sensor is null)
        {
            return NotFound();
        }

        var latest = await _dataContext.AirQualityLogs
            .Where(l => l.SensorId == sensorId)
            .OrderByDescending(l => l.Timestamp)
            .FirstOrDefaultAsync();

        if (latest is null)
        {
            return NotFound();
        }

        return Ok(AirQualityReadingMapper.ToDto(latest, sensor));
    }

    [HttpGet("history/{sensorId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<IList<AirQualityReadingDto>>> GetHistory(
        int sensorId,
        [FromQuery] DateTimeOffset? from,
        [FromQuery] DateTimeOffset? until)
    {
        var sensor = await _dataContext.Sensors.FindAsync(sensorId);
        if (sensor is null)
        {
            return NotFound();
        }

        var query = _dataContext.AirQualityLogs
            .Where(l => l.SensorId == sensorId);

        if (from is not null)
        {
            query = query.Where(l => l.Timestamp >= from);
        }
        if (until is not null)
        {
            query = query.Where(l => l.Timestamp <= until);
        }

        var logs = await query
            .OrderBy(l => l.Timestamp)
            .ToListAsync();

        var dtos = logs.Select(l => AirQualityReadingMapper.ToDto(l, sensor)).ToList();
        return Ok(dtos);
    }

    [HttpPost("fetch/{sensorId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status502BadGateway)]
    public async Task<ActionResult<AirQualityReadingDto>> FetchAndLog(int sensorId)
    {
        var sensor = await _dataContext.Sensors.FindAsync(sensorId);
        if (sensor is null)
        {
            return NotFound();
        }

        var log = await _airGradientClient.FetchCurrentAsync(sensor, HttpContext.RequestAborted);
        if (log is null)
        {
            return Problem(
                statusCode: StatusCodes.Status502BadGateway,
                title: "Could not reach the sensor.");
        }

        await _dataContext.AirQualityLogs.AddAsync(log);
        await _dataContext.SaveChangesAsync();

        return Ok(AirQualityReadingMapper.ToDto(log, sensor));
    }
}
