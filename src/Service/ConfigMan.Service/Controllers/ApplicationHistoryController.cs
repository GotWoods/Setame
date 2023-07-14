using ConfigMan.Data.Models;
using ConfigMan.Data.Projections;
using ConfigMan.Service.Models;
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
    public async Task<ActionResult<IEnumerable<ChangeHistory>>> GetAll(Guid id)
    {
        //TODO: paging and search
        var changeHistories = await _querySession.Query<ApplicationChangeHistory>().Where(x => x.ApplicationId == id).ToListAsync();
        var results = new List<ChangeHistory>();
        foreach (var history in changeHistories)
        {
            var user = _querySession.Query<UserSummary>().FirstOrDefault(x => x.Id == history.User);
            if (user == null)
                throw new NullReferenceException("User could not be found!");

            results.Add(new ChangeHistory(history.EventTime, history.ApplicationActionType.ToString(), history.Description, user.Username));
        }

        return Ok(results);
    }
}