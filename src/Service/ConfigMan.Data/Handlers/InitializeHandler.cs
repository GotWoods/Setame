using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using ConfigMan.Data.Handlers.EnvironmentSets;
using ConfigMan.Data.Models;
using Marten;
using MediatR;
using Microsoft.Extensions.Primitives;

namespace ConfigMan.Data.Handlers
{
    public record InitializeApplication(string AdminEmailAddress, string Password) : IRequest;
    public class InitializeHandler : IRequestHandler<InitializeApplication>
    {
        private readonly IUserService _userService;
        private readonly IDocumentSession _documentSession;
        private readonly IQuerySession _querySession;

        public InitializeHandler(IUserService userService, IDocumentSession documentSession, IQuerySession querySession)
        {
            _userService = userService;
            _documentSession = documentSession;
            _querySession = querySession;
        }

        public async Task Handle(InitializeApplication request, CancellationToken cancellationToken)
        {
            
            var currentStatus = await _querySession.Events.AggregateStreamAsync<ServiceStatus>(ServiceStatus.ServiceId, token: cancellationToken);
            if (CheckIfInitialized(currentStatus)) 
                throw new SecurityException("Application can not be initialized twice");

            var newAdminUser = new User
            {
                Username = request.AdminEmailAddress,
                Id = Guid.NewGuid()
            };
            await _userService.CreateUserAsync(newAdminUser, request.Password); 

            _documentSession.Events.StartStream<ServiceStatus>(ServiceStatus.ServiceId, new ApplicationInitialized());
            await _documentSession.SaveChangesAsync(cancellationToken);

        }

        private bool CheckIfInitialized(ServiceStatus? currentStatus)
        {
            if (currentStatus == null)
            {
                // Code continues execution if currentStatus is null
                //Console.WriteLine("Current Status is null. Code execution continues.");
                return false;
            }

            if (currentStatus.IsInitialized)
            {
                // Do not allow code execution if IsInitialized is true
                //Console.WriteLine("Current Status is initialized. Code execution not allowed.");
                return true;
            }

            // Code continues execution if IsInitialized is false
            //Console.WriteLine("Current Status is not initialized. Code execution continues.");
            return false;
        }
    }
}
