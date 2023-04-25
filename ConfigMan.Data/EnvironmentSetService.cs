using System.Data;
using System.Runtime.InteropServices;
using System.Xml.Linq;
using ConfigMan.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace ConfigMan.Data;

public interface IEnvironmentService
{
    Task<IEnumerable<EnvironmentSet>> GetAllAsync();
    Task<EnvironmentSet> GetOneAsync(string name);
    Task<EnvironmentSet> Create(EnvironmentSet environment);
    Task UpdateAsync(EnvironmentSet environment);
    // Task DeleteEnvironmentAsync(string name);
    // Task AddSettingToEnvironment(string environmentName, Setting settings);
}

public class EnvironmentSetService : IEnvironmentService
{
    private readonly AppDbContext _dbContext;

    public EnvironmentSetService(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IEnumerable<EnvironmentSet>> GetAllAsync()
    {
        return await _dbContext.Environments.ToListAsync();
    }

    public async Task<EnvironmentSet> GetOneAsync(string name)
    {
        return await _dbContext.Environments.FindAsync(name);
    }

    public async Task<EnvironmentSet> Create(EnvironmentSet environment)
    {
        if (environment == null) throw new ArgumentNullException(nameof(environment));

        var existing = await _dbContext.Environments.FirstOrDefaultAsync(x => x.Name == environment.Name);
        if (existing!= null)
            throw new DuplicateNameException("Can not have two environments of the same name");

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
    //
    // public async Task DeleteEnvironmentAsync(string name)
    // {
    //     var environment = await _dbContext.Environments.FirstAsync(x=>x.Name == name);
    //     if (environment == null) throw new InvalidOperationException("Environment not found.");
    //
    //     _dbContext.Environments.Remove(environment);
    //     await _dbContext.SaveChangesAsync();
    // }
    //
    // public async Task AddSettingToEnvironment(string environmentName, Setting setting)
    // {
    //     var environment = _dbContext.Environments.First(x=>x.Name == environmentName);
    //     if (environment.Settings == null)
    //         environment.Settings  = new List<Setting>();
    //
    //     environment.Settings.Add(setting);
    //
    //     _dbContext.Entry(environment).Property(x => x.Settings).IsModified = true; //EF does not do change detection on jsonb objects
    //     await _dbContext.SaveChangesAsync();
    // }
}