using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using MediatR;
using Swashbuckle.AspNetCore.Annotations;
using InnoAppointmentsApi.Dtos;
using InnoAppointmentsApi.Features.Appointments;
using InnoAppointmentsApi.Constants;

namespace InnoAppointmentsApi.Controllers;

[ApiController]
[Route("[controller]")]
[Consumes("application/json")]
[Authorize(Policy = AuthPolicies.RequireAllRoles)]
public sealed class AppointmentsController : ControllerBase
{
    private readonly IMediator _mediator;

    public AppointmentsController(IMediator mediator)
    {
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
    }
    
    [HttpPost]
    [Authorize(Policy = AuthPolicies.RequirePatientOrAdmin)]
    [SwaggerOperation(
        Summary = "Adds a new appointment",
        Description = "Creates a new appointment with the specified details",
        OperationId = "CreateAppointment"
    )]
    [SwaggerResponse(StatusCodes.Status201Created, "Appointment was created successfully", typeof(Guid))]
    [SwaggerResponse(StatusCodes.Status400BadRequest, "Invalid request body or parameters")]
    [SwaggerResponse(StatusCodes.Status401Unauthorized, "Unauthorized")]
    [SwaggerResponse(StatusCodes.Status403Forbidden, "Forbidden")]
    [SwaggerResponse(StatusCodes.Status500InternalServerError, "Internal service error")]
    public async Task<IActionResult> CreateAppointment([FromBody] CreateAppointmentCommand command, CancellationToken ct = default)
    {
        var result = await _mediator.Send(command, ct);
        return Created($"/appointments/{result}", result);
    }

    [HttpDelete("{id:guid}")]
    [Authorize(Policy = AuthPolicies.RequireStaff)]
    [SwaggerOperation(
        Summary = "Deletes an appointment",
        Description = "Permanently removes an appointment by its unique identifier.",
        OperationId = "DeleteAppointment"
    )]
    [SwaggerResponse(StatusCodes.Status204NoContent, "Appointment was successfully deleted")]
    [SwaggerResponse(StatusCodes.Status401Unauthorized, "Unauthorized")]
    [SwaggerResponse(StatusCodes.Status403Forbidden, "Forbidden")]
    [SwaggerResponse(StatusCodes.Status500InternalServerError, "Internal service error")]
    public async Task<IActionResult> DeleteAppointment([FromRoute] Guid id, CancellationToken ct = default)
    {
        await _mediator.Send(new DeleteAppointmentCommand(id), ct);
        return NoContent();
    }
    
    [HttpPut]
    [Authorize(Policy = AuthPolicies.RequireStaff)]
    [SwaggerOperation(
        Summary = "Edits an appointment",
        Description = "Edits appointment specified details",
        OperationId = "EditAppointment"
    )]
    [SwaggerResponse(StatusCodes.Status204NoContent, "Appointment was successfully edited")]
    [SwaggerResponse(StatusCodes.Status400BadRequest, "Invalid request body or parameters")]
    [SwaggerResponse(StatusCodes.Status401Unauthorized, "Unauthorized")]
    [SwaggerResponse(StatusCodes.Status403Forbidden, "Forbidden")]
    [SwaggerResponse(StatusCodes.Status500InternalServerError, "Internal service error")]
    public async Task<IActionResult> UpdateAppointment([FromBody] UpdateAppointmentCommand command, CancellationToken ct = default)
    {
        await _mediator.Send(command, ct);
        return NoContent();
    }
    
    [HttpGet("{id:guid}")]
    [SwaggerOperation(
        Summary = "Gets an appointment by ID",
        Description = "Retrieves detailed information for a specific appointment using its unique identifier.",
        OperationId = "GetAppointmentById"
    )]
    [SwaggerResponse(StatusCodes.Status200OK, "Appointment retrieved successfully", typeof(AppointmentDto))]
    [SwaggerResponse(StatusCodes.Status404NotFound, "Appointment with specified ID was not found")]
    [SwaggerResponse(StatusCodes.Status401Unauthorized, "Unauthorized")]
    [SwaggerResponse(StatusCodes.Status500InternalServerError, "Internal service error")]
    public async Task<IActionResult> GetAppointment([FromRoute] Guid id, CancellationToken ct = default)
    {
        var appointment = await _mediator.Send(new GetAppointmentByIdQuery(id), ct);
        return appointment is not null ? Ok(appointment) : NotFound();
    }
    
    [HttpGet("doctor/{doctorId:guid}")]
    [SwaggerOperation(
        Summary = "Gets appointments by doctor ID",
        Description = "Retrieves a list of appointments associated with a specific doctor.",
        OperationId = "GetAppointmentsByDoctorId"
    )]
    [SwaggerResponse(StatusCodes.Status200OK, "Appointments retrieved successfully", typeof(IEnumerable<AppointmentDto>))]
    [SwaggerResponse(StatusCodes.Status401Unauthorized, "Unauthorized")]
    [SwaggerResponse(StatusCodes.Status500InternalServerError, "Internal service error")]
    public async Task<IActionResult> GetByDoctorId([FromRoute] Guid doctorId, CancellationToken ct = default)
    {
        var appointments = await _mediator.Send(new GetAppointmentsByDoctorIdQuery(doctorId), ct);
        return Ok(appointments);
    }

    [HttpGet("patient/{patientId:guid}")]
    [SwaggerOperation(
        Summary = "Gets appointments by patient ID",
        Description = "Retrieves a list of appointments associated with a specific patient.",
        OperationId = "GetAppointmentsByPatientId"
    )]
    [SwaggerResponse(StatusCodes.Status200OK, "Appointments retrieved successfully", typeof(IEnumerable<AppointmentDto>))]
    [SwaggerResponse(StatusCodes.Status401Unauthorized, "Unauthorized")]
    [SwaggerResponse(StatusCodes.Status500InternalServerError, "Internal service error")]
    public async Task<IActionResult> GetByPatientId([FromRoute] Guid patientId, CancellationToken ct = default)
    {
        var appointments = await _mediator.Send(new GetAppointmentsByPatientIdQuery(patientId), ct);
        return Ok(appointments);
    }
}