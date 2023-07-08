using ConfigMan.Data.Models.Projections;
using Marten;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ConfigMan.Service.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Administrator")]
    public class EnvironmentSetApplicationAssociationController : ControllerBase
    {
        private readonly IQuerySession _querySession;

        public EnvironmentSetApplicationAssociationController(IQuerySession querySession)
        {
            _querySession = querySession;
        }

        [HttpGet("{environmentId}")]
        public Task<ActionResult<EnvironmentSetApplicationAssociation>> GetAll(Guid environmentId)
        {
            var data = _querySession.Query<EnvironmentSetApplicationAssociation>().First(x => x.Id == environmentId);
            return Task.FromResult<ActionResult<EnvironmentSetApplicationAssociation>>(Ok(data));
        }
    }
}
