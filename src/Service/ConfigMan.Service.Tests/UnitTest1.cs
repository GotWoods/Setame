// using System;
// using System.Threading;
// using System.Threading.Tasks;
// using ConfigMan.Data.Handlers.Applications;
// using ConfigMan.Data.Models;
// using Marten;
// using MediatR;
// using Moq;
// using Kekiri.Xunit;
//
// namespace ConfigMan.Data.Tests.Handlers.Applications
// {
//     public class CreateApplicationHandlerTests2 : Scenarios
//     {
//         private CreateApplicationHandler _handler;
//         private Mock<IDocumentSession> _documentSessionMock;
//         private Mock<IQuerySession> _querySessionMock;
//         private Mock<IUserInfo> _userInfoMock;
//         private CreateApplication _command;
//         private EnvironmentSet _environmentSet;
//
//         [Scenario]
//         public void CallingHandle()
//         {
//             Given(Given_a_CreateApplicationHandler)
//                 .And(Given_a_CreateApplicationCommand)
//                 .AndAsync(Given_an_EnvironmentSet);
//
//             WhenAsync(When_Handle_is_called);
//
//             Then(Then_the_Application_should_be_created)
//                 .And(Then_the_changes_should_be_saved)
//                 .And(Then_the_Application_should_be_associated_with_environments);
//         }
//
//         public void Given_a_CreateApplicationHandler()
//         {
//             _documentSessionMock = new Mock<IDocumentSession>();
//             _querySessionMock = new Mock<IQuerySession>();
//             _userInfoMock = new Mock<IUserInfo>();
//             _handler = new CreateApplicationHandler(_userInfoMock.Object);
//         }
//
//         public void Given_a_CreateApplicationCommand()
//         {
//             var applicationId = Guid.NewGuid();
//             var environmentSetId = Guid.NewGuid();
//             _command = new CreateApplication(applicationId, "Test App", "Token123", environmentSetId);
//         }
//
//         public async Task Given_an_EnvironmentSet()
//         {
//             _environmentSet = new EnvironmentSet
//             {
//                 DeploymentEnvironments = new List<DeploymentEnvironment>() { new DeploymentEnvironment { Name = "Production" }, new DeploymentEnvironment { Name = "Staging" }, new DeploymentEnvironment { Name = "Development" } }
//             };
//
//             _querySessionMock.Setup(x => x.Events.AggregateStreamAsync<EnvironmentSet>(_command.EnvironmentSetId, 0, null, null, 0, It.IsAny<CancellationToken>()))
//                 .ReturnsAsync(_environmentSet);
//         }
//
//         
//         public async Task When_Handle_is_called()
//         {
//             await _handler.Handle(_command, CancellationToken.None);
//         }
//
//         public void Then_the_Application_should_be_created()
//         {
//             _documentSessionMock.Verify(x => x.Events.StartStream<Application>(
//                 _command.ApplicationId,
//                 It.Is<ApplicationCreated>(e =>
//                     e.Id == _command.ApplicationId &&
//                     e.Name == _command.Name &&
//                     e.Token == _command.Token &&
//                     e.EnvironmentSet == _command.EnvironmentSetId)), Times.Once);
//         }
//
//         public void Then_the_changes_should_be_saved()
//         {
//             _documentSessionMock.Verify(x => x.SaveChangesAsync(CancellationToken.None), Times.Once);
//         }
//
//         public void Then_the_Application_should_be_associated_with_environments()
//         {
//             foreach (var deploymentEnvironment in _environmentSet.DeploymentEnvironments)
//             {
//                 _documentSessionMock.Verify(x => x.AppendToStreamAndSave<Application>(
//                     _command.ApplicationId,
//                     It.Is<ApplicationEnvironmentAdded>(e => e.Name == deploymentEnvironment.Name),
//                     It.IsAny<Guid>()), Times.Once);
//
//                 _documentSessionMock.Verify(x => x.AppendToStreamAndSave<EnvironmentSet>(
//                     _command.EnvironmentSetId,
//                     It.Is<ApplicationAssociatedToEnvironmentSet>(e =>
//                         e.ApplicationId == _command.ApplicationId &&
//                         e.EnvironmentSetId == _command.EnvironmentSetId),
//                     It.IsAny<Guid>()), Times.Once);
//             }
//         }
//     }
// }
