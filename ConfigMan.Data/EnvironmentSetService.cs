using System.Data;
using ConfigMan.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace ConfigMan.Data;

public interface IEnvironmentSetService
{
    // Task<IEnumerable<EnvironmentSet>> GetAllAsync();
    // Task<EnvironmentSet> GetOneAsync(string name);
    // Task<EnvironmentSet> CreateAsync(EnvironmentSet environment);
    // Task UpdateAsync(EnvironmentSet environment);
    // Task DeleteAsync(string name);
    // Task RenameAsync(string oldName, string newName);
}

//public record CreateEnvironmentSet(Guid Id, string Name);
//public record RenameEnvironmentSet(string OldName, string NewName);

public class EnvironmentSetService : IEnvironmentSetService
{
    // private readonly IApplicationService _applicationService;
    //
    // public EnvironmentSetService(IApplicationService applicationService)
    // {
    //     _applicationService = applicationService;
    // }

    // public static EnvironmentSetCreated Handle(CreateEnvironmentSet command)
    // {
    //     return new EnvironmentSetCreated(command.Id, command.Name);
    // }

    // public static EnvironmentSetRenamed Handle(EnvironmentSet current, RenameEnvironmentSet command)
    // {
    //     //TODO: make sure no naming conflicts
    //     return new EnvironmentSetRenamed(command.OldName, command.NewName);
    // }


    // public async Task<IEnumerable<EnvironmentSet>> GetAllAsync()
    // {
    //
    //     return await _dbContext.EnvironmentSets.ToListAsync();
    // }
    //
    // public async Task<EnvironmentSet> GetOneAsync(string name)
    // {
    //     var environmentSet = await _dbContext.EnvironmentSets.FindAsync(name);
    //     if (environmentSet == null)
    //         throw new NullReferenceException("Could not find environment set by name " + name);
    //     return environmentSet;
    // }
    //
    // public async Task<EnvironmentSet> CreateAsync(EnvironmentSet environment)
    // {
    //
    //     if (environment == null) throw new ArgumentNullException(nameof(environment));
    //
    //     var existing = await _dbContext.EnvironmentSets.FirstOrDefaultAsync(x => x.Name == environment.Name);
    //     if (existing != null)
    //         throw new DuplicateNameException("Can not have two Environment Sets of the same name");
    //
    //     await _dbContext.EnvironmentSets.AddAsync(environment);
    //     await _dbContext.SaveChangesAsync();
    //
    //     return environment;
    // }
    //
    // public async Task UpdateAsync(EnvironmentSet environment)
    // {
    //     if (environment == null) throw new ArgumentNullException(nameof(environment));
    //
    //     _dbContext.EnvironmentSets.Update(environment);
    //     await _dbContext.SaveChangesAsync();
    // }
    //
    // public async Task DeleteAsync(string name)
    // {
    //     var environment = await GetOneAsync(name);
    //
    //     var applications = await _applicationService.GetApplicationsAsync();
    //     foreach (var application in applications)
    //         if (application.EnvironmentSet == name)
    //             throw new InvalidOperationException("Can not delete an Environment Set when an application is associated to the environment set");
    //
    //     _dbContext.EnvironmentSets.Remove(environment);
    //     await _dbContext.SaveChangesAsync();
    // }
    //
    // public async Task RenameAsync(string oldName, string newName)
    // {
    //     var environmentSet = await GetOneAsync(oldName);
    //     var newSet = environmentSet.Copy(); //have to copy as we can not change the key of an object 
    //     newSet.Name = newName;
    //
    //     var allApps = await _applicationService.GetApplicationsAsync();
    //     foreach (var application in allApps)
    //         if (application.EnvironmentSet == oldName)
    //         {
    //             application.EnvironmentSet = newName;
    //             await _applicationService.UpdateApplicationAsync(application);
    //         }
    //
    //     _dbContext.EnvironmentSets.Remove(environmentSet); //delete the original
    //     await _dbContext.EnvironmentSets.AddAsync(newSet);
    //     await _dbContext.SaveChangesAsync();
    // }
}