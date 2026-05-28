using Application.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class PersonsController : ControllerBase
{
    private readonly ISender _sender;
    public PersonsController(ISender sender) { _sender = sender; }

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] int page = 1, [FromQuery] int size = 10)
    {
        return Ok(await _sender.Send(new GetPersonsQuery(page, size)));
    }
}