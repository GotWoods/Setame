using ConfigMan.Data.Models;
using Microsoft.EntityFrameworkCore;
using System;
using static System.Net.Mime.MediaTypeNames;
using Application = ConfigMan.Data.Models.Application;

namespace ConfigMan.Data;

public interface IApplicationService
{
    // Task<IEnumerable<Application>> GetApplicationsAsync();
    // Task<Application?> GetApplicationByIdAsync(string name);
    // Task<Application> CreateApplicationAsync(Application application);
    // Task UpdateApplicationAsync(Application application);
    // Task DeleteApplicationAsync(string name);

}

public class ApplicationService : IApplicationService
{
    // private readonly AppDbContext _dbContext;
    //
    // public ApplicationService(AppDbContext dbContext)
    // {
    //     _dbContext = dbContext;
    // }

    // public async Task<IEnumerable<Application>> GetApplicationsAsync()
    // {
    //     return await _dbContext.Applications.ToListAsync();
    // }
    //
    // public async Task<Application?> GetApplicationByIdAsync(string name)
    // {
    //     var app = await _dbContext.Applications.FirstOrDefaultAsync(x => x.Name == name);
    //     if (app == null)
    //         throw new NullReferenceException("Could not find application by name " + name);
    //     return app;
    // }
    //
    // public async Task<Application> CreateApplicationAsync(Application application)
    // {
    //     if (application == null) throw new ArgumentNullException(nameof(application));
    //
    //     await _dbContext.Applications.AddAsync(application);
    //     await _dbContext.SaveChangesAsync();
    //
    //     return application;
    // }
    //
    // public async Task UpdateApplicationAsync(Application application)
    // {
    //     if (application == null) throw new ArgumentNullException(nameof(application));
    //
    //     _dbContext.Applications.Update(application);
    //     await _dbContext.SaveChangesAsync();
    // }
    //
    // public async Task DeleteApplicationAsync(string name)
    // {
    //     var application = await _dbContext.Applications.FirstOrDefaultAsync(x=>x.Name == name);
    //     if (application == null) throw new InvalidOperationException("Application not found.");
    //
    //     _dbContext.Applications.Remove(application);
    //     await _dbContext.SaveChangesAsync();
    // }
}