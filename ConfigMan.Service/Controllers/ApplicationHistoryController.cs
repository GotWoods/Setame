using ConfigMan.Data.Models.Projections;
using Marten;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ConfigMan.Service.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize(Roles = "Administrator")]
public class ApplicationHistoryController : ControllerBase
{
    private readonly IQuerySession _querySession;


    public ApplicationHistoryController(IQuerySession querySession)
    {
        _querySession = querySession;
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<IEnumerable<ApplicationChangeHistory>>> GetAll(Guid id)
    {
        //TODO: paging and search
        return Ok(await _querySession.Query<ApplicationChangeHistory>().Where(x => x.Id == id).ToListAsync());
    }
}