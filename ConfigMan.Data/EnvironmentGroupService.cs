// using ConfigMan.Data.Models;
// using Microsoft.EntityFrameworkCore;
// using System.Data;
//
// namespace ConfigMan.Data;
//
//
// public interface IEnvironmentGroupService
// {
//     Task<List<EnvironmentGroup>> GetAllAsync();
//     Task<EnvironmentGroup> GetByNameAsync(string name);
//     Task<EnvironmentGroup> CreateAsync(EnvironmentGroup environment);
//     //Task UpdateEnvironmentAsync(DeploymentEnvironment environment);
//     //Task DeleteEnvironmentAsync(string name);
//     //Task AddSettingToEnvironment(string environmentName, Setting settings);
// }
//
// public class EnvironmentGroupService : IEnvironmentGroupService
// {
//     private readonly AppDbContext _dbContext;
//
//     public EnvironmentGroupService(AppDbContext dbContext)
//     {
//         _dbContext = dbContext;
//     }
//
//     public async Task<List<EnvironmentGroup>> GetAllAsync()
//     {
//         return await _dbContext.EnvironmentGroups.ToListAsync();
//     }
//
//     public async Task<EnvironmentGroup> GetByNameAsync(string name)
//     {
//         return await _dbContext.EnvironmentGroups.FindAsync(name);
//     }
//
//     public async Task<EnvironmentGroup> CreateAsync(EnvironmentGroup environmentGroup)
//     {
//         if (environmentGroup == null) throw new ArgumentNullException(nameof(environmentGroup));
//
//         var existing = await _dbContext.EnvironmentGroups.FirstOrDefaultAsync(x => x.Name == environmentGroup.Name);
//         if (existing != null)
//             throw new DuplicateNameException("Can not have two environment groups of the same name");
//
//         await _dbContext.EnvironmentGroups.AddAsync(environmentGroup);
//         await _dbContext.SaveChangesAsync();
//
//         return environmentGroup;
//     }
// }