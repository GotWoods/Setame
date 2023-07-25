using ConfigMan.Data.Data;
using ConfigMan.Data.Handlers.Applications;
using ConfigMan.Data.Models;
using ConfigMan.Data.Projections;
using MediatR;
using Microsoft.Extensions.Logging;

namespace ConfigMan.Data.Handlers.EnvironmentSets;

public record DeleteEnvironmentFromEnvironmentSet
    (Guid EnvironmentSetId, int ExpectedVersion, string EnvironmentName) : IRequest<CommandResponse>;

public class
    DeleteEnvironmentFromEnvironmentSetHandler : IRequestHandler<DeleteEnvironmentFromEnvironmentSet, CommandResponse>
{
    private readonly IDocumentSessionHelper<Application> _applicationDocumentSessionHelper;
    private readonly IDocumentSessionHelper<EnvironmentSet> _documentSession;
    private readonly IEnvironmentSetApplicationAssociationRepository _environmentSetApplicationAssociationRepository;
    private readonly ILogger<DeleteEnvironmentFromEnvironmentSetHandler> _logger;


    public DeleteEnvironmentFromEnvironmentSetHandler(IDocumentSessionHelper<EnvironmentSet> documentSession, IDocumentSessionHelper<Application> applicationDocumentSessionHelper, IEnvironmentSetApplicationAssociationRepository environmentSetApplicationAssociationRepository, ILogger<DeleteEnvironmentFromEnvironmentSetHandler> logger)
    {
        _documentSession = documentSession;
        _applicationDocumentSessionHelper = applicationDocumentSessionHelper;
        _environmentSetApplicationAssociationRepository = environmentSetApplicationAssociationRepository;
        _logger = logger;
    }


    public async Task<CommandResponse> Handle(DeleteEnvironmentFromEnvironmentSet command, CancellationToken cancellationToken)
    {
        var environmentRemoved = new EnvironmentRemoved(command.EnvironmentName);

        await _documentSession.AppendToStream(command.EnvironmentSetId, command.ExpectedVersion, environmentRemoved);

        var associations = _environmentSetApplicationAssociationRepository.Get(command.EnvironmentSetId);
        foreach (var application in associations.Applications)
            await _applicationDocumentSessionHelper.AppendToStream(application.Id, environmentRemoved);

        await _documentSession.SaveChangesAsync();
        await _applicationDocumentSessionHelper.SaveChangesAsync();
        return CommandResponse.FromSuccess(command.ExpectedVersion + 1);
    }
}