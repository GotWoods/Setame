using Marten;
using Microsoft.AspNetCore.Mvc;


//TODO: Remove this before prod
namespace ConfigMan.Service.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ResetController : ControllerBase
{
    private readonly IDocumentStore _documentStore;

    public ResetController(IDocumentStore documentStore)
    {
        _documentStore = documentStore;
    }

    [HttpGet]
    public async Task Reset()
    {
        //TODO: remove this before prod
        await _documentStore.Advanced.Clean.CompletelyRemoveAllAsync();
    }
}