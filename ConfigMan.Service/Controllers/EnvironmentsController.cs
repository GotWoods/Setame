using ConfigMan.Data;
using ConfigMan.Data.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ConfigMan.Service.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize] 
    public class EnvironmentsController : ControllerBase
    {
        private readonly IEnvironmentService _environmentService;

        public EnvironmentsController(IEnvironmentService environmentService)
        {
            _environmentService = environmentService;
        }

        // GET: api/environments
        [HttpGet]
        public async Task<ActionResult<IEnumerable<DeploymentEnvironment>>> GetEnvironments()
        {
            var deploymentEnvironments = await _environmentService.GetEnvironmentsAsync();
            var sorted = deploymentEnvironments.ToList().OrderBy(x=>x.Order);
            return Ok(sorted);
        }

        // POST: api/environments
        [HttpPost]
        public async Task<ActionResult<DeploymentEnvironment>> CreateEnvironment(DeploymentEnvironment environment)
        {
            await _environmentService.CreateEnvironmentAsync(environment);
            return CreatedAtAction(nameof(CreateEnvironment), new { id = environment.Name }, environment);
        }

        // // GET: api/environments/guid
        // [HttpGet("{id}")]
        // public async Task<ActionResult<DeploymentEnvironment>> GetEnvironment(Guid id)
        // {
        //     var environment = await _environmentService.GetEnvironmentByIdAsync(id);
        //
        //     if (environment == null)
        //     {
        //         return NotFound();
        //     }
        //
        //     return Ok(environment);
        // }

        // PUT: api/environments/guid
        // [HttpPut("{id}")]
        // public async Task<IActionResult> UpdateEnvironment(DeploymentEnvironment environment)
        // {
        //     await _environmentService.UpdateEnvironmentAsync(environment);
        //     return NoContent();
        // }

        // DELETE: api/environments/guid
        [HttpDelete("{name}")]
        public async Task<IActionResult> DeleteEnvironment(string name)
        {
            await _environmentService.DeleteEnvironmentAsync(name);
            return NoContent();
        }
    }

}
