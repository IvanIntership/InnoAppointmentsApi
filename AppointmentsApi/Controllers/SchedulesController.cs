using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using MediatR;
using Swashbuckle.AspNetCore.Annotations;
using InnoAppointmentsApi.Dtos;
using InnoAppointmentsApi.Features.Schedules;
using InnoAppointmentsApi.Constants;

namespace InnoAppointmentsApi.Controllers;

[ApiController]
[Route("[controller]")]
[Consumes("application/json")]
[Authorize(Policy = AuthPolicies.RequireStaff)]
public sealed class SchedulesController : ControllerBase
{
    private readonly IMediator _mediator;

    public SchedulesController(IMediator mediator)
    {
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
    }
    
    [HttpPost]
    [SwaggerOperation(
        Summary = "Adds a new schedule",
        Description = "Creates a new monthly schedule for a doctor",
        OperationId = "CreateSchedule"
    )]
    [SwaggerResponse(StatusCodes.Status201Created, "Schedule was created successfully", typeof(Guid))]
    [SwaggerResponse(StatusCodes.Status400BadRequest, "Invalid request body or parameters")]
    [SwaggerResponse(StatusCodes.Status401Unauthorized, "Unauthorized")]
    [SwaggerResponse(StatusCodes.Status403Forbidden, "Forbidden")]
    [SwaggerResponse(StatusCodes.Status500InternalServerError, "Internal service error")]
    public async Task<IActionResult> CreateSchedule([FromBody] CreateScheduleCommand command, CancellationToken ct = default)
    {
        var result = await _mediator.Send(command, ct);
        return Created($"/schedules/{result}", result);
    }

    [HttpDelete("{id:guid}")]
    [SwaggerOperation(
        Summary = "Deletes a schedule",
        Description = "Permanently removes a doctor's schedule by its unique identifier.",
        OperationId = "DeleteSchedule"
    )]
    [SwaggerResponse(StatusCodes.Status204NoContent, "Schedule was successfully deleted")]
    [SwaggerResponse(StatusCodes.Status401Unauthorized, "Unauthorized")]
    [SwaggerResponse(StatusCodes.Status403Forbidden, "Forbidden")]
    [SwaggerResponse(StatusCodes.Status500InternalServerError, "Internal service error")]
    public async Task<IActionResult> DeleteSchedule([FromRoute] Guid id, CancellationToken ct = default)
    {
        await _mediator.Send(new DeleteScheduleCommand(id), ct);
        return NoContent();
    }
    
    [HttpPut]
    [SwaggerOperation(
        Summary = "Edits a schedule",
        Description = "Edits doctor's schedule specified details",
        OperationId = "EditSchedule"
    )]
    [SwaggerResponse(StatusCodes.Status204NoContent, "Schedule was successfully edited")]
    [SwaggerResponse(StatusCodes.Status400BadRequest, "Invalid request body or parameters")]
    [SwaggerResponse(StatusCodes.Status401Unauthorized, "Unauthorized")]
    [SwaggerResponse(StatusCodes.Status403Forbidden, "Forbidden")]
    [SwaggerResponse(StatusCodes.Status500InternalServerError, "Internal service error")]
    public async Task<IActionResult> UpdateSchedule([FromBody] UpdateScheduleCommand command, CancellationToken ct = default)
    {
        await _mediator.Send(command, ct);
        return NoContent();
    }
    
    [HttpGet("{id:guid}")]
    [Authorize(Policy = AuthPolicies.RequireAllRoles)]
    [SwaggerOperation(
        Summary = "Gets a schedule by ID",
        Description = "Retrieves detailed information for a specific schedule using its unique identifier.",
        OperationId = "GetScheduleById"
    )]
    [SwaggerResponse(StatusCodes.Status200OK, "Schedule retrieved successfully", typeof(ScheduleDto))]
    [SwaggerResponse(StatusCodes.Status404NotFound, "Schedule with specified ID was not found")]
    [SwaggerResponse(StatusCodes.Status401Unauthorized, "Unauthorized")]
    [SwaggerResponse(StatusCodes.Status500InternalServerError, "Internal service error")]
    public async Task<IActionResult> GetSchedule([FromRoute] Guid id, CancellationToken ct = default)
    {
        var schedule = await _mediator.Send(new GetScheduleByIdQuery(id), ct);
        return schedule is not null ? Ok(schedule) : NotFound();
    }
    
    [HttpGet("doctor/{doctorId:guid}")]
    [Authorize(Policy = AuthPolicies.RequireAllRoles)]
    [SwaggerOperation(
        Summary = "Gets schedules by doctor ID",
        Description = "Retrieves a list of schedules associated with a specific doctor.",
        OperationId = "GetSchedulesByDoctorId"
    )]
    [SwaggerResponse(StatusCodes.Status200OK, "Schedules retrieved successfully", typeof(IEnumerable<ScheduleDto>))]
    [SwaggerResponse(StatusCodes.Status401Unauthorized, "Unauthorized")]
    [SwaggerResponse(StatusCodes.Status500InternalServerError, "Internal service error")]
    public async Task<IActionResult> GetByDoctorId([FromRoute] Guid doctorId, CancellationToken ct = default)
    {
        var schedules = await _mediator.Send(new GetSchedulesByDoctorIdQuery(doctorId), ct);
        return Ok(schedules);
    }
}