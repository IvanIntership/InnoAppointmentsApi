using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using MediatR;
using Swashbuckle.AspNetCore.Annotations;
using InnoAppointmentsApi.Dtos;
using InnoAppointmentsApi.Features.Results;
using InnoAppointmentsApi.Constants;

namespace InnoAppointmentsApi.Controllers;

[ApiController]
[Route("[controller]")]
[Consumes("application/json")]
[Authorize(Policy = AuthPolicies.RequireStaff)]
public sealed class ResultsController : ControllerBase
{
    private readonly IMediator _mediator;

    public ResultsController(IMediator mediator)
    {
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
    }
    
    [HttpPost]
    [SwaggerOperation(
        Summary = "Adds a new result",
        Description = "Creates a new appointment result with the specified details",
        OperationId = "CreateResult"
    )]
    [SwaggerResponse(StatusCodes.Status201Created, "Result was created successfully", typeof(Guid))]
    [SwaggerResponse(StatusCodes.Status400BadRequest, "Invalid request body or parameters")]
    [SwaggerResponse(StatusCodes.Status401Unauthorized, "Unauthorized")]
    [SwaggerResponse(StatusCodes.Status403Forbidden, "Forbidden")]
    [SwaggerResponse(StatusCodes.Status500InternalServerError, "Internal service error")]
    public async Task<IActionResult> CreateResult([FromBody] CreateResultCommand command, CancellationToken ct = default)
    {
        var result = await _mediator.Send(command, ct);
        return Created($"/results/{result}", result);
    }

    [HttpDelete("{id:guid}")]
    [SwaggerOperation(
        Summary = "Deletes a result",
        Description = "Permanently removes an appointment result by its unique identifier.",
        OperationId = "DeleteResult"
    )]
    [SwaggerResponse(StatusCodes.Status204NoContent, "Result was successfully deleted")]
    [SwaggerResponse(StatusCodes.Status401Unauthorized, "Unauthorized")]
    [SwaggerResponse(StatusCodes.Status403Forbidden, "Forbidden")]
    [SwaggerResponse(StatusCodes.Status500InternalServerError, "Internal service error")]
    public async Task<IActionResult> DeleteResult([FromRoute] Guid id, CancellationToken ct = default)
    {
        await _mediator.Send(new DeleteResultCommand(id), ct);
        return NoContent();
    }
    
    [HttpPut]
    [SwaggerOperation(
        Summary = "Edits a result",
        Description = "Edits appointment result specified details",
        OperationId = "EditResult"
    )]
    [SwaggerResponse(StatusCodes.Status204NoContent, "Result was successfully edited")]
    [SwaggerResponse(StatusCodes.Status400BadRequest, "Invalid request body or parameters")]
    [SwaggerResponse(StatusCodes.Status401Unauthorized, "Unauthorized")]
    [SwaggerResponse(StatusCodes.Status403Forbidden, "Forbidden")]
    [SwaggerResponse(StatusCodes.Status500InternalServerError, "Internal service error")]
    public async Task<IActionResult> UpdateResult([FromBody] UpdateResultCommand command, CancellationToken ct = default)
    {
        await _mediator.Send(command, ct);
        return NoContent();
    }
    
    [HttpGet("{id:guid}")]
    [Authorize(Policy = AuthPolicies.RequireAllRoles)]
    [SwaggerOperation(
        Summary = "Gets a result by ID",
        Description = "Retrieves detailed information for a specific result using its unique identifier.",
        OperationId = "GetResultById"
    )]
    [SwaggerResponse(StatusCodes.Status200OK, "Result retrieved successfully", typeof(ResultDto))]
    [SwaggerResponse(StatusCodes.Status404NotFound, "Result with specified ID was not found")]
    [SwaggerResponse(StatusCodes.Status401Unauthorized, "Unauthorized")]
    [SwaggerResponse(StatusCodes.Status500InternalServerError, "Internal service error")]
    public async Task<IActionResult> GetResult([FromRoute] Guid id, CancellationToken ct = default)
    {
        var result = await _mediator.Send(new GetResultByIdQuery(id), ct);
        return result is not null ? Ok(result) : NotFound();
    }
    
    [HttpGet("appointment/{appointmentId:guid}")]
    [Authorize(Policy = AuthPolicies.RequireAllRoles)]
    [SwaggerOperation(
        Summary = "Gets a result by appointment ID",
        Description = "Retrieves result details associated with a specific appointment.",
        OperationId = "GetResultByAppointmentId"
    )]
    [SwaggerResponse(StatusCodes.Status200OK, "Result retrieved successfully", typeof(ResultDto))]
    [SwaggerResponse(StatusCodes.Status404NotFound, "Result with specified appointment ID was not found")]
    [SwaggerResponse(StatusCodes.Status401Unauthorized, "Unauthorized")]
    [SwaggerResponse(StatusCodes.Status500InternalServerError, "Internal service error")]
    public async Task<IActionResult> GetByAppointmentId([FromRoute] Guid appointmentId, CancellationToken ct = default)
    {
        var result = await _mediator.Send(new GetResultByAppointmentIdQuery(appointmentId), ct);
        return result is not null ? Ok(result) : NotFound();
    }
}