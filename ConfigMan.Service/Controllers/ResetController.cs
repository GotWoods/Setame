using Marten;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Data;


//TODO: Remove this before prod
namespace ConfigMan.Service.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    

    public class ResetController : ControllerBase
    {
        private readonly IDocumentStore _documentStore;

        public ResetController(IDocumentStore documentStore)
        {
            _documentStore = documentStore;
        }

        [HttpPost("login")]
        public async Task Reset()
        {
            await _documentStore.Advanced.Clean.CompletelyRemoveAllAsync();
        }
    }
}
