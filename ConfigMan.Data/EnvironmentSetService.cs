using System.Data;
using System.Runtime.InteropServices;
using System.Xml.Linq;
using ConfigMan.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace ConfigMan.Data;

public interface IEnvironmentSetService
{
    Task<IEnumerable<EnvironmentSet>> GetAllAsync();
    Task<EnvironmentSet> GetOneAsync(string name);
    Task<EnvironmentSet> Create(EnvironmentSet environment);
    Task Update(EnvironmentSet environment);
    // Task DeleteEnvironmentAsync(string name);
    // Task AddSettingToEnvironment(string environmentName, Setting settings);
    Task Delete(string name);
}

public class EnvironmentSetService : IEnvironmentSetService
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
            throw new DuplicateNameException("Can not have two Environment Sets of the same name");

        await _dbContext.Environments.AddAsync(environment);
        await _dbContext.SaveChangesAsync();

        return environment;
    }

    public async Task Update(EnvironmentSet environment)
    {
        if (environment == null) throw new ArgumentNullException(nameof(environment));
    
        _dbContext.Environments.Update(environment);
        await _dbContext.SaveChangesAsync();
    }
    
    public async Task Delete(string name)
    {
        var environment = await _dbContext.Environments.FirstAsync(x=>x.Name == name);
        if (environment == null) throw new InvalidOperationException("Environment Set not found.");
    
        _dbContext.Environments.Remove(environment);
        await _dbContext.SaveChangesAsync();
    }
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