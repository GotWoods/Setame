using ConfigMan.Data.Models;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace ConfigMan.Data;


public interface IVariableGroupService
{
    Task<List<VariableGroup>> GetAllAsync();
    Task<VariableGroup> GetByNameAsync(string name);
    Task<VariableGroup> CreateAsync(VariableGroup Variable);
    //Task UpdateVariableAsync(DeploymentVariable Variable);
    //Task DeleteVariableAsync(string name);
    //Task AddSettingToVariable(string VariableName, Setting settings);
}

public class VariableGroupService : IVariableGroupService
{
    private readonly AppDbContext _dbContext;

    public VariableGroupService(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<List<VariableGroup>> GetAllAsync()
    {
        return await _dbContext.VariableGroups.ToListAsync();
    }

    public async Task<VariableGroup> GetByNameAsync(string name)
    {
        return await _dbContext.VariableGroups.FindAsync(name);
    }

    public async Task<VariableGroup> CreateAsync(VariableGroup variableGroup)
    {
        if (variableGroup == null) throw new ArgumentNullException(nameof(variableGroup));

        var existing = await _dbContext.VariableGroups.FirstOrDefaultAsync(x => x.Name == variableGroup.Name);
        if (existing != null)
            throw new DuplicateNameException("Can not have two Variable groups of the same name");

        await _dbContext.VariableGroups.AddAsync(variableGroup);
        await _dbContext.SaveChangesAsync();

        return variableGroup;
    }
}