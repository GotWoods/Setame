using System.Data;
using ConfigMan.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace ConfigMan.Data;

public interface IEnvironmentSetService
{
    Task<IEnumerable<EnvironmentSet>> GetAllAsync();
    Task<EnvironmentSet> GetOneAsync(string name);
    Task<EnvironmentSet> CreateAsync(EnvironmentSet environment);
    Task UpdateAsync(EnvironmentSet environment);
    Task DeleteAsync(string name);
    Task RenameAsync(string oldName, string newName);
}

public class EnvironmentSetService : IEnvironmentSetService
{
    private readonly IApplicationService _applicationService;
    private readonly AppDbContext _dbContext;

    public EnvironmentSetService(AppDbContext dbContext, IApplicationService applicationService)
    {
        _dbContext = dbContext;
        _applicationService = applicationService;
    }

    public async Task<IEnumerable<EnvironmentSet>> GetAllAsync()
    {
        return await _dbContext.Environments.ToListAsync();
    }

    public async Task<EnvironmentSet> GetOneAsync(string name)
    {
        var environmentSet = await _dbContext.Environments.FindAsync(name);
        if (environmentSet == null)
            throw new NullReferenceException("Could not find environment set by name " + name);
        return environmentSet;
    }

    public async Task<EnvironmentSet> CreateAsync(EnvironmentSet environment)
    {
        if (environment == null) throw new ArgumentNullException(nameof(environment));

        var existing = await _dbContext.Environments.FirstOrDefaultAsync(x => x.Name == environment.Name);
        if (existing != null)
            throw new DuplicateNameException("Can not have two Environment Sets of the same name");

        await _dbContext.Environments.AddAsync(environment);
        await _dbContext.SaveChangesAsync();

        return environment;
    }

    public async Task UpdateAsync(EnvironmentSet environment)
    {
        if (environment == null) throw new ArgumentNullException(nameof(environment));

        _dbContext.Environments.Update(environment);
        await _dbContext.SaveChangesAsync();
    }

    public async Task DeleteAsync(string name)
    {
        var environment = await GetOneAsync(name);

        var applications = await _applicationService.GetApplicationsAsync();
        foreach (var application in applications)
            if (application.EnvironmentSet == name)
                throw new InvalidOperationException("Can not delete an Environment Set when an application is associated to the environment set");

        _dbContext.Environments.Remove(environment);
        await _dbContext.SaveChangesAsync();
    }

    public async Task RenameAsync(string oldName, string newName)
    {
        var environmentSet = await GetOneAsync(oldName);
        var newSet = environmentSet.Copy(); //have to copy as we can not change the key of an object 
        newSet.Name = newName;

        var allApps = await _applicationService.GetApplicationsAsync();
        foreach (var application in allApps)
            if (application.EnvironmentSet == oldName)
            {
                application.EnvironmentSet = newName;
                await _applicationService.UpdateApplicationAsync(application);
            }

        _dbContext.Environments.Remove(environmentSet); //delete the original
        await _dbContext.Environments.AddAsync(newSet);
        await _dbContext.SaveChangesAsync();
    }
}