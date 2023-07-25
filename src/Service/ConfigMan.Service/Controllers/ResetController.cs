using Marten;
using Microsoft.AspNetCore.Mvc;

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
        //this is only for testing
        if (!(HttpContext.RequestServices.GetService<IWebHostEnvironment>() ?? throw new InvalidOperationException()).IsDevelopment())
        {
            throw new InvalidOperationException("Reset endpoint is only available in development environment.");
        }

        await _documentStore.Advanced.Clean.CompletelyRemoveAllAsync();
    }
}