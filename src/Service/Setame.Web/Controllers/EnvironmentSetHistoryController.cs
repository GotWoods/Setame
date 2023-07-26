using Marten;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Setame.Data.Projections;
using Setame.Web.Models;

namespace Setame.Web.Controllers;

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
    public async Task<ActionResult<IEnumerable<ChangeHistory>>> GetAll(Guid id)
    {
        //TODO: paging and search
        var changeHistories = await _querySession.Query<EnvironmentSetChangeHistory>().Where(x => x.EnvironmentSetId == id).ToListAsync();
        var results = new List<ChangeHistory>();
        foreach (var history in changeHistories)
        {
            var user = _querySession.Query<UserSummary>().FirstOrDefault(x => x.Id == history.User);
            if (user == null)
                throw new NullReferenceException("User could not be found!");
                
            results.Add(new ChangeHistory(history.timestamp, history.EnvironmentActionType.ToString(), history.Description, user.Username));
        }
        return Ok(results);
    }
}