using Microsoft.AspNetCore.Mvc;
using PersonFinder.Application.Common;
using PersonFinder.Application.Features.Persons.Commands;
using PersonFinder.Application.Features.Persons.Queries;
using PersonFinder.API.Contracts.Requests;

namespace PersonFinder.API.Controllers;

[ApiController]
[Route("api/v1/persons")]
public sealed class PersonsController : ControllerBase
{
    private readonly CreatePersonCommandHandler _handler;
    private readonly UpdatePersonLocationCommandHandler _updateLocationHandler;
    private readonly GetPersonByIdQueryHandler _getByIdHandler;
    private readonly GetNearbyPersonsQueryHandler _getNearbyHandler;
    private readonly ILogger<PersonsController> _logger;

    public PersonsController(
        CreatePersonCommandHandler handler,
        UpdatePersonLocationCommandHandler updateLocationHandler,
        GetPersonByIdQueryHandler getByIdHandler,
        GetNearbyPersonsQueryHandler getNearbyHandler,
        ILogger<PersonsController> logger)
    {
        _handler = handler;
        _updateLocationHandler = updateLocationHandler;
        _getByIdHandler = getByIdHandler;
        _getNearbyHandler = getNearbyHandler;
        _logger = logger;
    }

    /// <summary>
    /// Gets one specific person by id (including their location point when available).
    /// </summary>
    [HttpGet("/api/v1/person/{id:long}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetPersonById(
        [FromRoute] long id,
        CancellationToken cancellationToken)
    {
        var query = new GetPersonByIdQuery(id);
        var result = await _getByIdHandler.Handle(query, cancellationToken);

        if (!result.IsSuccess)
        {
            return NotFound(new { error = result.Error });
        }

        return Ok(result.Value);
    }

    /// <summary>
    /// Creates a new person profile. A bio is generated asynchronously via Gemini AI.
    /// </summary>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreatePerson(
        [FromBody] CreatePersonRequest request,
        CancellationToken cancellationToken)
    {
        // Layer 1 defense: reject prompt-injection keywords before reaching the LLM
        if (!InputValidator.IsInputSafe(request.JobTitle, request.Hobbies))
        {
            _logger.LogWarning("event=createPerson reason=validation_failed invalid_job_or_hobbies");
            return BadRequest(new { error = "Input contains forbidden keywords." });
        }

        // Validate coordinates when provided
        if (request.Latitude.HasValue && request.Longitude.HasValue)
        {
            if (!InputValidator.IsLocationValid(request.Latitude.Value, request.Longitude.Value))
            {
                _logger.LogWarning("event=createPerson reason=validation_failed invalid_location");
                return BadRequest(new { error = "Invalid latitude or longitude values." });
            }
        }

        var command = new CreatePersonCommand(
            request.Name,
            request.JobTitle,
            request.Hobbies,
            request.Latitude,
            request.Longitude);

        var result = await _handler.Handle(command, cancellationToken);

        if (!result.IsSuccess)
        {
            _logger.LogError("event=createPerson error={Error}", result.Error);
            return StatusCode(StatusCodes.Status500InternalServerError);
        }

        return CreatedAtAction(
            actionName: null,
            routeValues: new { id = result.Value!.Id },
            value: result.Value);
    }

    /// <summary>
    /// Updates an existing person's location.
    /// </summary>
    [HttpPut("{id:long}/location")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdatePersonLocation(
        [FromRoute] long id,
        [FromBody] UpdatePersonLocationRequest request,
        CancellationToken cancellationToken)
    {
        if (!InputValidator.IsLocationValid(request.Latitude, request.Longitude))
        {
            _logger.LogWarning("event=updateLocation reason=validation_failed invalid_location personId={PersonId}", id);
            return BadRequest(new { error = "Invalid latitude or longitude values." });
        }

        var command = new UpdatePersonLocationCommand(id, request.Latitude, request.Longitude);
        var result = await _updateLocationHandler.Handle(command, cancellationToken);

        if (!result.IsSuccess)
        {
            if (string.Equals(result.Error, "Person not found.", StringComparison.Ordinal))
            {
                return NotFound(new { error = result.Error });
            }

            _logger.LogError("event=updateLocation error={Error} personId={PersonId}", result.Error, id);
            return StatusCode(StatusCodes.Status500InternalServerError);
        }

        return NoContent();
    }

    /// <summary>
    /// Finds all persons within a given radius (metres) of a coordinate point.
    /// </summary>
    [HttpGet("nearby")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> FindNearby(
        [FromQuery] double latitude,
        [FromQuery] double longitude,
        [FromQuery] double radius,
        CancellationToken cancellationToken)
    {
        if (!InputValidator.IsLocationValid(latitude, longitude, radius))
        {
            _logger.LogWarning(
                "event=findNearby reason=validation_failed lat={Lat} lon={Lon} radius={Radius}",
                latitude, longitude, radius);
            return BadRequest(new { error = "Invalid latitude, longitude, or radius values." });
        }

        var query = new GetNearbyPersonsQuery(latitude, longitude, radius);
        var result = await _getNearbyHandler.Handle(query, cancellationToken);

        if (!result.IsSuccess)
        {
            _logger.LogError("event=findNearby error={Error}", result.Error);
            return StatusCode(StatusCodes.Status500InternalServerError);
        }

        return Ok(result.Value);
    }
}
