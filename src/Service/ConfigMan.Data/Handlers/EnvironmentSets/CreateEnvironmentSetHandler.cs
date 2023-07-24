using ConfigMan.Data.Data;
using ConfigMan.Data.Models;
using JasperFx.Core;
using Marten;
using MediatR;

namespace ConfigMan.Data.Handlers.EnvironmentSets;

public record CreateEnvironmentSet(string Name) : IRequest<CommandResponse>;

public class CreateEnvironmentSetHandler : IRequestHandler<CreateEnvironmentSet, CommandResponse>
{
    private readonly IDocumentSessionHelper<EnvironmentSet> _documentSession;
    private readonly IEnvironmentSetRepository _environmentSetRepository;


    public CreateEnvironmentSetHandler(IDocumentSessionHelper<EnvironmentSet> documentSession, IEnvironmentSetRepository environmentSetRepository)
    {
        _documentSession = documentSession;
        _environmentSetRepository = environmentSetRepository;
    }

    public async Task<CommandResponse> Handle(CreateEnvironmentSet command, CancellationToken cancellationToken)
    {
      var matchingName = _environmentSetRepository.GetByName(command.Name);
        if (matchingName != null)
        {
            return CommandResponse.FromError(Errors.DuplicateName(command.Name));
        }

        var id = CombGuidIdGeneration.NewGuid();
        _documentSession.Start(id, new EnvironmentSetCreated(id, command.Name));
        await _documentSession.SaveChangesAsync();
        return CommandResponse.FromSuccess(1);
    }
}