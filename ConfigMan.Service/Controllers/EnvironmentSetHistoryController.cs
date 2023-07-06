using ConfigMan.Data.Models.Projections;
using Marten;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ConfigMan.Service.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize(Roles = "Administrator")]
public class EnvironmentSetHistoryController : ControllerBase
{
    private readonly IQuerySession _querySession;


    public EnvironmentSetHistoryController(IQuerySession querySession)
    {
        _querySession = querySession;
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<IEnumerable<EnvironmentSetChangeHistory>>> GetAll(Guid id)
    {
        //TODO: paging and search
        return Ok(await _querySession.Query<EnvironmentSetChangeHistory>().Where(x => x.EnvironmentSetId == id).ToListAsync());
    }
}